namespace Soundboard.Lib.Services
{
    public interface IAudioManagerService
    {
        public string DefaultFolderPath { get; }
        public string[] FileNames { get; }
        public bool CanAudioOverlap { get; set; }
        public void PlayAudio(string audioKeyName, float virtualCableVolume = 0.5f, float localVolume = 0.5f);
        public void Stop();
        public void FreeResources();
    }
}
