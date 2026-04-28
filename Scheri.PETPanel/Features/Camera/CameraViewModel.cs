using LibVLCSharp.Shared;
using Scheri.PETPanel.Utils;
using Splat;
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
        private LibVLC? _libVlc;
        public MediaPlayer? MediaPlayer { get; private set; }

        private string _url = string.Empty;
        private bool _isMonitoring = false;

        public CameraViewModel()
        {
            _libVlc = new LibVLC("--rtsp-tcp", "--avcodec-hw=none", "--no-audio", "--verbose=2");
            _libVlc.Log += (s, e) => AppLogger.Info($"[VLC] {e.Message}", nameof(CameraViewModel));
            MediaPlayer = new MediaPlayer(_libVlc);
        }

        public void Play(string url)
        {
            if (string.IsNullOrEmpty(url)) return;
            _url = url;
            if (_isMonitoring) return;
            _isMonitoring = true;

            Task.Run(async () => {
                while (_isMonitoring)
                {
                    var state = MediaPlayer?.State;
                    if (state == VLCState.Error || state == VLCState.Ended ||
                    state == VLCState.Stopped || state == VLCState.NothingSpecial)
                    {
                        await Task.Delay(1000);
                        ExecutePlay();
                    }
                    await Task.Delay(2000);
                }
            });

        }

        private void ExecutePlay()
        {
            if(_libVlc == null || MediaPlayer ==null ||string.IsNullOrEmpty(_url)) return;
            try
            {
                using var media = new Media(_libVlc, new Uri(_url), ":rtsp-tcp");
                media.AddOption(":network-caching=1000");
                media.AddOption(":avcodec-hw=none");
                MediaPlayer?.Play(media);
            }
            catch (Exception ex)
            {
                AppLogger.Error(ex.Message, nameof(CameraViewModel));
            }
        }

        public void Stop()
        {
            _isMonitoring = false;
            MediaPlayer?.Stop();
        }

        public void Dispose()
        {
            AppLogger.Info("[VLC] processing dispose...",nameof(Dispose));
            _isMonitoring = false;
            if (MediaPlayer != null)
            {
                MediaPlayer.Stop();
                MediaPlayer.Dispose();
                MediaPlayer = null;
            }
            _libVlc?.Dispose();
            _libVlc = null;
        }
    }
}
