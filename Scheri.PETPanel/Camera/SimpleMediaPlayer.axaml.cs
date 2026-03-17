using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using LibVLCSharp.Shared;
using System;
using System.Runtime.InteropServices;

namespace Scheri.PETPanel.Camera;

public partial class SimpleMediaPlayer : UserControl
{
    private readonly VLCVideoRenderer _videoRender = new();
    public string? MediaPath { get; private set; }
    private IntPtr _buff = IntPtr.Zero;
    private static int _width = 960;
    private static int _height = 400;
    public SimpleMediaPlayer()
    {
        InitializeComponent();
        _videoRender.MediaPlayer.SetVideoFormat("BGRA", (uint)_width, (uint)_height, (uint)_width * 4);
        _videoRender.MediaPlayer.SetVideoCallbacks(LockCallback, UnLockCallback, DisplayCallback);
        _videoRender.MediaPlayer.Mute = true;
        _videoRender.MediaPlayer.EncounteredError += (_, _) => {
            if (string.IsNullOrEmpty(MediaPath))
                return;
            _videoRender.Play(MediaPath);
        };
        _videoRender.MediaPlayer.EndReached += (_, _) => {
            if (string.IsNullOrEmpty(MediaPath))
                return;
            _videoRender.Play(MediaPath);
        };
        _videoRender.MediaPlayer.Playing += (_, _) => {
        };
        _videoRender.MediaPlayer.Buffering += (_, _) => {
        };
        _buff = Marshal.AllocHGlobal(1920 * 1080 * 4);
    }

    private void SetVlcState(string state)
    {
        Dispatcher.UIThread.Invoke(() => {
          //  LabelConnectInfo.Text = state;
        });
    }


    private IntPtr LockCallback(IntPtr opaque, IntPtr planes)
    {
        Marshal.WriteIntPtr(planes, _buff);
        return IntPtr.Zero;
    }

    private void UnLockCallback(IntPtr opaque, IntPtr picture, IntPtr planes)
    {
        var writeableBitmap = new WriteableBitmap(PixelFormat.Bgra8888,
            AlphaFormat.Opaque, _buff,
            new PixelSize(_width, _height), new Vector(96, 96), _width * 4);
        Dispatcher.UIThread.Post(() => {
            SetVlcState(_videoRender.MediaPlayer.State == VLCState.Playing ? "" : "加载中");
            ImgVideo.Source = writeableBitmap;
        });
    }

    private void DisplayCallback(IntPtr opaque, IntPtr picture)
    {

    }


    public void SetMediaPath(string mediaPath)
    {
        MediaPath = mediaPath;
        _videoRender.Play(MediaPath);
    }


    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        // if (Design.IsDesignMode)
        //     return;
        // if (string.IsNullOrEmpty(MediaPath))
        //     return;
        // if (_viewModel.MediaPlayer.IsPlaying)
        //     _viewModel.Resume();
        // else
        //     _viewModel.Play(MediaPath);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        // base.OnUnloaded(e);
        // _viewModel.Pause();
    }
}