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
using System.Threading.Tasks;

namespace Scheri.PETPanel.Camera;

public partial class SimpleMediaPlayer : UserControl
{
    //private readonly VLCVideoRenderer _videoRender = new();
    public string? MediaPath { get; private set; }
    private readonly LibVLC? _libVlc;
    private readonly MediaPlayer? _mediaPlayer;
    public SimpleMediaPlayer()
    {
        InitializeComponent();
        _libVlc = new LibVLC(enableDebugLogs: true);
        _libVlc.Log += (sender, args) => {
            System.Diagnostics.Debug.WriteLine($"[VLC Log]: {args.Message}");
        };
        _mediaPlayer = new MediaPlayer(_libVlc);
        VideoPlayer.MediaPlayer = _mediaPlayer;
    }

    public void Play(string url)
    {
        if (_libVlc == null || _mediaPlayer == null)
        {
            System.Diagnostics.Debug.WriteLine("Error: VLC not initialized!");
            return;
        }

        try
        {
            // 确保使用正确的 Uri 格式
            using var media = new Media(_libVlc, url, FromType.FromLocation);

            // 关键：强制开启 TCP 模式（针对 RTSP）
            media.AddOption(":rtsp-tcp");
            media.AddOption(":network-caching=500");

            System.Diagnostics.Debug.WriteLine($"Attempting to play: {url}");

            bool success = _mediaPlayer.Play(media);
            System.Diagnostics.Debug.WriteLine($"Play command sent. Success: {success}");
        }
        catch (Exception ex)
        {
            // 在 Android Logcat 中可以看到这个报错
            System.Diagnostics.Debug.WriteLine($"VLC Init Failed: {ex.Message}");
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _mediaPlayer?.Stop();
        _mediaPlayer?.Dispose();
        _libVlc?.Dispose();
    }
}