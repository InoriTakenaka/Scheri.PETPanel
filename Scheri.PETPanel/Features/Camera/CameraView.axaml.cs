using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using LibVLCSharp.Shared;
using Scheri.PETPanel.Features.Camera;
using Scheri.PETPanel.Utils;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Scheri.PETPanel.Features;

public partial class CameraView : UserControl
{
    private readonly CameraViewModel _vm = new();
    public string? MediaPath { get; private set; }
    private IntPtr _buff = IntPtr.Zero;
    private WriteableBitmap? _bitmap;
    private static int _width = 960;
    private static int _height = 400;
    private readonly object _syncLock = new();
    private long _lastFrameTime = 0;
    private const int TargetFPS = 30;
    private readonly long _frameInterval = 1000 / TargetFPS;

    public CameraView()
    {
        InitializeComponent();
        _buff = Marshal.AllocHGlobal(1920 * 1080 * 4);
        _bitmap = new WriteableBitmap(
          new PixelSize(_width, _height),
          new Vector(96, 96),
          PixelFormat.Bgra8888,
          AlphaFormat.Premul);
        VideoFrame.Source = _bitmap;

        if (_vm.MediaPlayer != null)
        {
            _vm.MediaPlayer.SetVideoFormat("RV32", (uint)_width, (uint)_height, (uint)_width * 4);
            _vm.MediaPlayer.SetVideoCallbacks(LockCallback, UnlockCallback, DisplayCallback); 
            _vm.MediaPlayer.Mute = true;
        }
    }

    private IntPtr LockCallback(IntPtr opaque, IntPtr planes)
    {
        if (_buff == IntPtr.Zero) return IntPtr.Zero;

        Marshal.WriteIntPtr(planes, 0, _buff);
        return _buff;
    }

    private unsafe void UnlockCallback(IntPtr opaque, IntPtr picture, IntPtr planes)
    {
        lock (_syncLock)
        {
            if (_buff == IntPtr.Zero || _bitmap == null) return;
            try
            {
                using (var locked = _bitmap.Lock())
                {
                    var size = _height * _width * 4;

                    System.Runtime.CompilerServices.Unsafe.CopyBlock(
                        locked.Address.ToPointer(),
                        _buff.ToPointer(),
                        (uint)size);
                }
                Dispatcher.UIThread.Post(
                    () => VideoFrame.InvalidateVisual(),
                    DispatcherPriority.Render);
            }
            catch { }
        }
    }

    private void DisplayCallback(IntPtr opaque, IntPtr picture)
    {
        long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        if (currentTime - _lastFrameTime < _frameInterval) return; // skip frame to maintain target FPS
        _lastFrameTime = currentTime;
    }

    public void SetMediaPath(string path)
    {
        MediaPath = path;
        _vm.Play(path);
    }

    /// <summary>
    /// Handles cleanup when the control is detached from the visual tree.
    /// </summary>
    /// <remarks>This method releases resources associated with video playback and disposes of unmanaged
    /// memory. It should be called when the control is no longer part of the visual tree to prevent resource
    /// leaks.</remarks>
    /// <param name="e">The event data associated with the visual tree detachment.</param>
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        VideoFrame.Source = null;
        if (_vm.MediaPlayer != null)
        {
            _vm.Stop();
            _vm.MediaPlayer.SetVideoCallbacks(null, null, null);
        }
        lock (_syncLock)
        {
            _vm.Dispose();
            if (_buff != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_buff);
                _buff = IntPtr.Zero;
            }
            VideoFrame.Source = null;
            _bitmap?.Dispose();
            _bitmap = null;
        }
        base.OnDetachedFromVisualTree(e);
    }
}