using LibVLCSharp.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Scheri.PETPanel.Camera;

public class VLCVideoRenderer
{
    private readonly LibVLC _libVlc = new();

    public MediaPlayer MediaPlayer { get; }

    private string _rtspUrl = string.Empty;

    public VLCVideoRenderer()
    {
        MediaPlayer = new MediaPlayer(_libVlc);
        MediaPlayer.EncounteredError += (s, e) => {
        };
        MediaPlayer.EndReached += (s, e) => {
        };
        MediaPlayer.TimeChanged += (s, e) => {
        };
    }

    public void Play(string url)
    {
        //url = "rtsp://127.0.0.1/live";
        _rtspUrl = url;
        Task.Factory.StartNew(() => {
            while (true)
            {
                if (MediaPlayer.Media == null)
                {
                    try
                    {
                        Play();
                    }
                    catch (Exception e)
                    {
                        //Debug.WriteLine(e);
                    }
                    Thread.Sleep(2000);
                }
                else
                {
                    if (MediaPlayer.State != VLCState.Playing)
                    {
                        MediaPlayer.Stop();
                        MediaPlayer.Media?.Dispose();
                        MediaPlayer.Media = null;
                    }
                    //Debug.WriteLine(MediaPlayer.State);
                }
                Thread.Sleep(1000);
            }
        });
    }

    private void Play()
    {
        MediaPlayer.Stop();
        MediaPlayer.Media?.Dispose();
        var media = new LibVLCSharp.Shared.Media(_libVlc, new Uri(mUrl));
        media.AddOption(":rtsp-timeout=5000");       // 网络流缓冲（毫秒）
        media.AddOption(":network-caching=100");       // 网络流缓冲（毫秒）
        media.AddOption(":live-caching=0");          // 实时流缓冲（毫秒）
        media.AddOption(":file-caching=0");
        media.AddOption(":clock-jitter=0");
        media.AddOption(":clock-synchro=0");
        media.AddOption(":rtsp-frame-buffer-size=50");
        media.AddOption(":drop-late-frames");
        MediaPlayer.Play(media);
    }

    public void Stop()
    {
        MediaPlayer.Stop();
    }

    public void Pause()
    {
        if (MediaPlayer.IsPlaying)
            MediaPlayer.Pause();
    }

    public void Resume()
    {
        MediaPlayer.SetPause(false);
    }

    public void Dispose()
    {
        MediaPlayer?.Dispose();
        _libVlc?.Dispose();
    }
}


