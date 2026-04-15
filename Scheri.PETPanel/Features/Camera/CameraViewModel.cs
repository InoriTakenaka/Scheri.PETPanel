using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Scheri.PETPanel.Features.Camera
{
    public class CameraViewModel
    {
        private readonly LibVLC _libVlc;

        public MediaPlayer MediaPlayer { get; }

        private string mUrl=string.Empty;

        public CameraViewModel()
        {
            _libVlc = new LibVLC("--rtsp-tcp","--avcodec-hw=none","--network-caching=300", "--no-audio", "--verbose=2");
            _libVlc.Log += (s, e) =>
            {
                Debug.WriteLine($"[{e.Level}] {e.Message}");
            };
            MediaPlayer = new MediaPlayer(_libVlc);
            MediaPlayer.EncounteredError += (s, e) =>
            {
            };
            MediaPlayer.EndReached += (s, e) =>
            {
            };
            MediaPlayer.TimeChanged += (s, e) =>
            {
            };
        }

        public void Play(string url)
        {
            mUrl = url;
            Task.Factory.StartNew(() =>
            {
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
            var media = new Media(_libVlc, new Uri(mUrl), ":rtsp-mcast=0");
            media.AddOption(":rtsp-timeout=5000");       // 网络流缓冲（毫秒）
            media.AddOption(":network-caching=2000");       // 网络流缓冲（毫秒）
            media.AddOption(":live-caching=0");          // 实时流缓冲（毫秒）
            media.AddOption(":file-caching=0");
            media.AddOption(":clock-jitter=0");
            media.AddOption(":clock-synchro=0");
            media.AddOption(":rtsp-frame-buffer-size=50");
            media.AddOption(":drop-late-frames");
            media.AddOption("--verbose=2");
            media.AddOption("--rtsp-tcp");
            media.AddOption("：avcodec-hw=none");
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
            MediaPlayer.Stop();
            MediaPlayer?.Dispose();
            _libVlc?.Dispose();
        }
    }
}
