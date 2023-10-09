using ManagedBass;

namespace Soundboard.Lib.Services
{
    /// <summary>
    /// Provides audio management services for playing sounds on different devices.
    /// </summary>
    public class AudioManagerService : IAudioManagerService
    {
        private readonly string _defaultFolderPath = Path.Combine(Environment.CurrentDirectory, "Sounds");
        public string DefaultFolderPath
        {
            get => _defaultFolderPath;
        }
        private string _soundsFolderPath;
        public string SoundsFolderPath
        {
            get => _soundsFolderPath;
            set
            {
                _soundsFolderPath = value;
                if (!Directory.Exists(SoundsFolderPath))
                {
                    Directory.CreateDirectory(SoundsFolderPath);
                }
                LoadAudioFilesFromFolder(SoundsFolderPath);
            }
        }

        private const string _cableDeviceString = "Cable";
        private const string _localDeviceString = "Speakers";
        Dictionary<string, int> _devices = new Dictionary<string, int>(2);

        /// <summary>
        /// Gets or sets a value indicating whether audio playback can overlap.
        /// </summary>
        public bool CanAudioOverlap { get; set; }

        private Dictionary<string, int> _cableFilesStream;
        private Dictionary<string, int> _localFilesStream;

        /// <summary>
        /// Gets an array of file names for loaded audio streams.
        /// </summary>
        public string[] FileNames => _localFilesStream.Keys.ToArray();

        /// <summary>
        /// Initializes a new instance of the AudioManagerService class.
        /// </summary>
        public AudioManagerService()
        {
            SoundsFolderPath = _defaultFolderPath;

            for (int i = 0; i < Bass.DeviceCount; i++)
            {
                DeviceInfo device = Bass.GetDeviceInfo(i);
                if (device.Name.Contains(_cableDeviceString, StringComparison.InvariantCultureIgnoreCase))
                {
                    _devices.Add(_cableDeviceString, i);
                    Bass.Init(i);
                }
                else if (device.Name.Contains(_localDeviceString, StringComparison.InvariantCultureIgnoreCase))
                {
                    _devices.Add(_localDeviceString, i);
                    Bass.Init(i);
                }
            }
        }

        /// <summary>
        /// Play an audio file on specified output devices with custom volumes.
        /// </summary>
        /// <param name="audioKeyName">The key name of the audio file.</param>
        /// <param name="cableVolume">Volume for the virtual cable device.</param>
        /// <param name="localVolume">Volume for the local device.</param>
        public void PlayAudio(string audioKeyName, float cableVolume = 1.0f, float localVolume = 1.0f)
        {
            if (!CanAudioOverlap)
            {
                Stop();
            }

            if (_localFilesStream.ContainsKey(audioKeyName))
            {
                PlayAudioFromStream(_cableFilesStream[audioKeyName], _devices[_cableDeviceString], cableVolume);
                PlayAudioFromStream(_localFilesStream[audioKeyName], _devices[_localDeviceString], localVolume);
            }
        }

        /// <summary>
        /// Stops both the Local and Virtual audio outputs.
        /// </summary>
        public void Stop()
        {
            List<int> cableStreamFiles = _cableFilesStream.Values.ToList();
            List<int> localStreamFiles = _localFilesStream.Values.ToList();
            for (int i = 0; i < cableStreamFiles.Count; i++)
            {
                if (Bass.ChannelIsActive(cableStreamFiles[i]) == PlaybackState.Playing)
                {
                    Bass.ChannelStop(cableStreamFiles[i]);
                    Bass.ChannelStop(localStreamFiles[i]);
                    return;
                }
            }
        }

        /// <summary>
        /// Free up resources.
        /// </summary>
        public void FreeResources()
        {
            Stop();
            FreeAudioStreams();
            Bass.Free();
        }

        /// <summary>
        /// Frees the audio streams associated with the virtual cable and local devices,
        /// releasing resources and stopping playback for both devices.
        /// </summary>
        private void FreeAudioStreams()
        {
            List<int> cableAudioStreams = _cableFilesStream.Values.ToList();
            for (int i = 0; i < cableAudioStreams.Count; i++)
            {
                Bass.StreamFree(cableAudioStreams[i]);
            }
            _localFilesStream.Clear();

            List<int> localAudioStreams = _localFilesStream.Values.ToList();
            for (int i = 0; i < localAudioStreams.Count; i++)
            {
                Bass.StreamFree(localAudioStreams[i]);
            }
            _localFilesStream.Clear();
        }

        /// <summary>
        /// Play an MP3 audio file on a specified device with custom volume.
        /// </summary>
        /// <param name="audioStream">The audio stream handle.</param>
        /// <param name="deviceId">The device ID of the output device.</param>
        /// <param name="volume">Volume for the audio.</param>
        private void PlayAudioFromStream(int audioStream, int deviceId, float volume)
        {
            Bass.ChannelSetDevice(audioStream, deviceId);
            Bass.ChannelSetAttribute(audioStream, ChannelAttribute.Volume, volume);
            Bass.ChannelPlay(audioStream, true);
        }

        /// <summary>
        /// Load MP3 and WAV files from a specified folder into memory streams.
        /// </summary>
        /// <param name="inputFolderPath">Path to the folder containing audio files.</param>
        private void LoadAudioFilesFromFolder(string inputFolderPath)
        {
            string[] soundFiles = Directory.GetFiles(inputFolderPath, "*.mp3");

            if (_cableFilesStream == null || _localFilesStream == null)
            {
                _cableFilesStream = new Dictionary<string, int>(soundFiles.Length);
                _localFilesStream = new Dictionary<string, int>(soundFiles.Length);
            }
            if (_cableFilesStream.Count > 0 || _localFilesStream.Count > 0)
            {
                _cableFilesStream.Clear();
                _localFilesStream.Clear();
            }

            Bass.Init(-1);
            for (int i = 0; i < soundFiles.Length; i++)
            {
                if (!File.Exists(soundFiles[i]))
                {
                    continue;
                }

                int cableAudioStream = 0;
                int localAudioStream = 0;
                string fileExtension = Path.GetExtension(soundFiles[i]).ToLower();
                if (fileExtension == ".mp3" || fileExtension == ".wav")
                {
                    cableAudioStream = Bass.CreateStream(soundFiles[i]);
                    localAudioStream = Bass.CreateStream(soundFiles[i]);
                }

                if (cableAudioStream != 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(soundFiles[i]);
                    _cableFilesStream.Add(fileName, cableAudioStream);
                    _localFilesStream.Add(fileName, localAudioStream);
                }
            }
        }
    }
}
