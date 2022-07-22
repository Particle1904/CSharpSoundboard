using NAudio.Wave;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParticleSoundboard.src
{
    public class AudioEngine
    {
        private List<WaveOutCapabilities> outputDevices;

        // The Virtual cable output
        private WaveOut virtualCableOutput;
        // The Local audio output
        private WaveOut localCableOutput;

        public bool CanAudioOverlap { get; set; }

        public AudioEngine()
        {
            outputDevices = GetAudioDevices();

            virtualCableOutput = new WaveOut();
            localCableOutput = new WaveOut();
        }

        /// <summary>
        /// Play the audio in both the Virtual Cable and Local Devices.
        /// </summary>
        /// <param name="filePath"> The path of the Mp3 File. </param>
        /// <param name="virtualCableVolume"> The Volume of the Virtual Cable device. </param>
        /// <param name="localVolume"> The Volume of the Local device. </param>
        public void PlayAudio(string filePath, float virtualCableVolume = 0.5f, float localVolume = 0.5f)
        {
            if (!CanAudioOverlap)
            {
                // I dont understand why I cant just STOP both WaveOuts... I have to Stop,
                // Dispose and Create new instances for it to do the behaviour I want... ?!?
                StopWaveOut();
                DisposeWaveOut();
                virtualCableOutput = new WaveOut();
                localCableOutput = new WaveOut();
            }

            // Volume must be swapped here cus of how VBCable is coffigured to stream audios to microphone output.
            PlayMp3Audio(filePath, "Cable", localVolume);
            PlayMp3Audio(filePath, "Speakers", virtualCableVolume);
        }

        /// <summary>
        /// Play a Mp3 audio form source.
        /// </summary>
        /// <param name="filePath"> The path of the Mp3 File. </param>
        /// <param name="deviceName"> The name of the Device to use. </param>
        /// <param name="volume"> The volume of the audio between 0.0f and 1.0f</param>
        private void PlayMp3Audio(string filePath, string deviceName, float volume)
        {

            //WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(new Mp3FileReader(filePath));
            //BlockAlignReductionStream? stream = new BlockAlignReductionStream(pcmStream);

            virtualCableOutput.DeviceNumber = outputDevices.IndexOf(outputDevices.FirstOrDefault(
                device => device.ProductName.Contains(deviceName)));

            AudioFileReader audioFile = new AudioFileReader(filePath);

            audioFile.Volume = volume;

            virtualCableOutput.Init(audioFile);
            virtualCableOutput.Play();
        }

        /// <summary>
        /// Get a List of available Audio Devices.
        /// </summary>
        /// <returns></returns>
        private List<WaveOutCapabilities> GetAudioDevices()
        {
            List<WaveOutCapabilities> devices = new List<WaveOutCapabilities>();
            // -1 to get all devices.
            for (int i = -1; i < WaveOut.DeviceCount; i++)
            {
                devices.Add(WaveOut.GetCapabilities(i));
            }

            return devices;
        }

        /// <summary>
        /// Stops both the Local and Virtual WaveOut audio outputs.
        /// </summary>
        public void StopWaveOut()
        {
            virtualCableOutput?.Stop();
            localCableOutput?.Stop();
        }

        /// <summary>
        /// Dispose the WaveOuts to free up resources.
        /// </summary>
        public void DisposeWaveOut()
        {
            StopWaveOut();
            virtualCableOutput.Dispose();
            localCableOutput.Dispose();
        }
    }
}
