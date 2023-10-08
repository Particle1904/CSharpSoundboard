using NAudio.Wave;
using NAudio.Wave.SampleProviders;

using System.Diagnostics;
using System.Reflection.PortableExecutable;

namespace Soundboard.Lib.Services
{
    public class AudioManagerService : IAudioManagerService
    {
        private readonly string _defaultFolderPath = Path.Combine(Environment.CurrentDirectory, "Sounds");

        private List<DirectSoundDeviceInfo> _outputDevices;
        private WaveOutEvent _virtualCableOutput;
        private WaveOutEvent _localOutput;

        public bool CanAudioOverlap { get; set; }

        public Dictionary<string, WaveStream> _audioFilesStream;
        public string[] FileNames => _audioFilesStream.Keys.ToArray();

        public AudioManagerService()
        { 
            if (!Directory.Exists(_defaultFolderPath))
            {
                Directory.CreateDirectory(_defaultFolderPath);
            }

            _outputDevices = DirectSoundOut.Devices.ToList();
            for (int i = 0; i < _outputDevices.Count; i++)
            {
                if (_outputDevices[i].Description.Contains("Cable", StringComparison.InvariantCultureIgnoreCase))
                {                   
                    _virtualCableOutput = new WaveOutEvent();
                    _virtualCableOutput.DeviceNumber = i - 1;
                    _virtualCableOutput.DesiredLatency = 200;
                }
                else if (_outputDevices[i].Description.Contains("Speakers", StringComparison.InvariantCultureIgnoreCase))
                {
                    _localOutput = new WaveOutEvent();
                    _localOutput.DeviceNumber = i - 1;
                    _localOutput.DesiredLatency = 200;
                }
            }

            LoadAudioFilesFromFolder(_defaultFolderPath);
        }

        /// <summary>
        /// Play an audio file on specified output devices with custom volumes.
        /// </summary>
        /// <param name="filePath">Path to the audio file.</param>
        /// <param name="virtualCableVolume">Volume for the virtual cable device.</param>
        /// <param name="localVolume">Volume for the local device.</param>
        public void PlayAudio(string audioKeyName, float virtualCableVolume = 1.0f, float localVolume = 1.0f)
        {
            if (!CanAudioOverlap)
            {
                StopWave();
            }            

            // Volume must be swapped here cus of how VBCable is coffigured to stream audios to microphone output.
            if (_audioFilesStream.ContainsKey(audioKeyName))
            {
                WaveStream stream = _audioFilesStream[audioKeyName];
                PlayAudioFromStream(stream, _localOutput, localVolume);
                PlayAudioFromStream(stream, _virtualCableOutput, virtualCableVolume);
            }
        }

        /// <summary>
        /// Stops both the Local and Virtual DirectSound audio outputs.
        /// </summary>
        public void StopWave()
        {
            _virtualCableOutput?.Stop();
            _localOutput?.Stop();
        }

        /// <summary>
        /// Dispose the DirectSound outputs to free up resources.
        /// </summary>
        public void DisposeWave()
        {
            StopWave();
            _virtualCableOutput.Dispose();
            _localOutput.Dispose();
        }

        public string GetDefaultSoundsFolder()
        {
            return _defaultFolderPath;
        }

        /// <summary>
        /// Play an MP3 audio file on a specified device with custom volume.
        /// </summary>
        /// <param name="filePath">Path to the MP3 audio file.</param>
        /// <param name="deviceName">Name of the output device.</param>
        /// <param name="volume">Volume for the audio.</param>
        private void PlayAudioFromStream(WaveStream audioStream, WaveOutEvent device, float volume)
        {
            audioStream.Position = 0;
            VolumeSampleProvider volumeStream = new VolumeSampleProvider(audioStream.ToSampleProvider());
            volumeStream.Volume = Math.Clamp(volume, 0.0f, 1.0f);
            volumeStream.ToWaveProvider16();

            device.Init(volumeStream);
            device.Play();
            Debug.WriteLine("Sound played");
        }

        /// <summary>
        /// Load MP3 and WAV files from a specified folder into memory streams.
        /// </summary>
        /// <param name="inputFolderPath">Path to the folder containing audio files.</param>
        private void LoadAudioFilesFromFolder(string inputFolderPath)
        {
            string[] soundFiles = Directory.GetFiles(inputFolderPath, "*.mp3");

            if (_audioFilesStream == null)
            {
                _audioFilesStream = new Dictionary<string, WaveStream>(soundFiles.Length);
            }
            if (_audioFilesStream.Count > 0)
            {
                _audioFilesStream.Clear();
            }
            
            for (int i = 0; i < soundFiles.Length; i++)
            {
                if (!File.Exists(soundFiles[i]))
                {
                    continue;
                }

                WaveStream audioStream = null;

                string fileExtension = Path.GetExtension(soundFiles[i]).ToLower();
                if (fileExtension == ".mp3")
                {
                    audioStream = new AudioFileReader(soundFiles[i]);
                }
                else if (fileExtension == ".wav")
                {
                    audioStream = new WaveFileReader(soundFiles[i]);
                }

                if (audioStream != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(soundFiles[i]);
                    _audioFilesStream.Add(fileName, audioStream);
                }
            }
        }
    }
}
