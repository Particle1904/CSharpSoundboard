using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio;
using NAudio.Wave;

namespace ParticleSoundboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string _soundsFolderPath = @"E:\Stuff\Sounds\";

        private List<WaveOutCapabilities> _outputDevices = new List<WaveOutCapabilities>();

        private WaveOut _output;
        private WaveOut _outputLocal;
        BlockAlignReductionStream stream;
        BlockAlignReductionStream stream2;

        private List<Button> _buttons = new List<Button>();
        private string[] _soundFiles;

        public MainWindow()
        {
            InitializeComponent();
            #region Find buttons, get files from directory and set buttons content.
            // Find all the buttons that are children of the Grid Named ButtonsGrid.
            for (int i = 0; i < (7 * 6); i++)
            {
                Button? buttonFound = ButtonsGrid.Children.OfType<Button>().FirstOrDefault(button => button.Name == $"Button{i + 1}");
                if (buttonFound != null)
                {
                    _buttons.Add(buttonFound);
                }
            }

            // Get a list of files in the Directory.
            _soundFiles = Directory.GetFiles(_soundsFolderPath, "*", SearchOption.TopDirectoryOnly);

            // Change the Buttons Content to be the sound file name.
            for (int i = 0; i < _soundFiles.Length; i++)
            {
                string[] soundFile = _soundFiles[i].Split(@"\");
                _buttons[i].Content = soundFile.Last().Replace(".mp3", "");
            }
            #endregion

            for (int i = -1; i < WaveOut.DeviceCount; i++)
            {
                _outputDevices.Add(WaveOut.GetCapabilities(i));
            }
        }

        private void Button_Click(object sender, MouseButtonEventArgs e)
        {
            ButtonStop_Click(sender, e);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Button senderButton = (Button)sender;
                if (senderButton.Content == null)
                {
                    return;
                }

                string filePath = $"{_soundsFolderPath}{senderButton.Content}.mp3";
                #region Output sound stream(virtual cable).
                WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(new Mp3FileReader(filePath));
                stream = new BlockAlignReductionStream(pcmStream);

                _output = new WaveOut();
                _output.DeviceNumber = _outputDevices.IndexOf(_outputDevices.FirstOrDefault(
                    device => device.ProductName.Contains("Cable")));

                AudioFileReader audioFile = new AudioFileReader(filePath);

                audioFile.Volume = 0.65f;

                _output.Init(audioFile);
                _output.Play();
                #endregion
                #region Local sound stream.
                WaveStream pcmStream2 = WaveFormatConversionStream.CreatePcmStream(new Mp3FileReader(filePath));
                stream2 = new BlockAlignReductionStream(pcmStream2);

                _outputLocal = new WaveOut();
                _outputLocal.DeviceNumber = _outputDevices.IndexOf(_outputDevices.FirstOrDefault(
                    device => device.ProductName.Contains("Speakers")));

                AudioFileReader audioFileLocal = new AudioFileReader(filePath);

                audioFileLocal.Volume = 0.65f;

                _outputLocal.Init(audioFileLocal);
                _outputLocal.Play();
                #endregion
            }
        }

        private void ButtonStop_Click(object sender, MouseButtonEventArgs e)
        {
            if (_outputLocal != null)
            {
                _outputLocal.Stop();
                _outputLocal.Dispose();
            }
            if (_output != null)
            {
                _output.Stop();
                _output.Dispose();
            }
        }
    }
}
