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
    private WriteableBitmap _bitmap;
    private static int _width = 960;
    private static int _height = 400;

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

        _vm.MediaPlayer.SetVideoFormat("RV32", (uint)_width, (uint)_height, (uint)_width * 4);
        _vm.MediaPlayer.SetVideoCallbacks(LockCallback, UnlockCallback, DisplayCallback);
        _vm.MediaPlayer.Mute = true;
        _vm.MediaPlayer.EncounteredError += (_, _) => {
            if (string.IsNullOrEmpty(MediaPath)) return;
            _vm.Play(MediaPath);
        };
        _vm.MediaPlayer.EndReached += async (_, _) => {
            if (string.IsNullOrEmpty(MediaPath)) return;
            await Task.Delay(1000);
            _vm.Play(MediaPath);
        };


    }

    private IntPtr LockCallback(IntPtr opaque, IntPtr planes)
    {
        Marshal.WriteIntPtr(planes, 0, _buff);
        return _buff;
    }

    private unsafe void UnlockCallback(IntPtr opaque, IntPtr picture, IntPtr planes)
    {
        using (var locked = _bitmap.Lock())
        {
            var size = _height * _width * 4;
            Buffer.MemoryCopy(_buff.ToPointer(), locked.Address.ToPointer(), size, size);
        }
        Dispatcher.UIThread.Post(() => VideoFrame.InvalidateVisual(), DispatcherPriority.Render);
    }

    private unsafe void DisplayCallback(IntPtr opaque, IntPtr picture)
    {
        long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        if (currentTime - _lastFrameTime < _frameInterval) return; // skip frame to maintain target FPS
        _lastFrameTime = currentTime;

        Dispatcher.UIThread.Post(() => {
            if (_buff == IntPtr.Zero || _bitmap == null) return;

            try
            {
                using (var locked = _bitmap.Lock())
                {
                    var size = _height * _width * 4;
                    Buffer.MemoryCopy(_buff.ToPointer(),
                        locked.Address.ToPointer(),
                        size,
                        size);
                }
            }
            catch { }
        }, DispatcherPriority.Render);
    }

    public void SetMediaPath(string path)
    {
        MediaPath = path;
        _vm.Play(path);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (_vm.MediaPlayer.IsPlaying)
        {
            _vm.MediaPlayer.Stop();
        }

        if (_buff != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(_buff);
            _buff = IntPtr.Zero;
        }

        VideoFrame.Source = null;
        _bitmap = null!;
    }
}