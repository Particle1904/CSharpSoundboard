using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soundboard.Lib.Services
{
    public interface IAudioManagerService
    {
        public string[] FileNames { get; }
        public bool CanAudioOverlap { get; set; }
        public void PlayAudio(string audioKeyName, float virtualCableVolume = 0.5f, float localVolume = 0.5f);
        public void StopWave();
        public void DisposeWave();
        public string GetDefaultSoundsFolder();
    }
}
