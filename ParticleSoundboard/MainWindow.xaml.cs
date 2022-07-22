using NAudio.Wave;

using ParticleSoundboard.src;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ParticleSoundboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string soundsFolderPath = @".\Sounds\";
        private string[] soundFiles;

        private List<Button> buttons;

        private AudioEngine audioEngine;

        public float localVolume;
        public float cableVolume;

        public MainWindow()
        {
            InitializeComponent();
            audioEngine = new AudioEngine();

            SetupSoundsFolder(soundsFolderPath);
            soundFiles = Directory.GetFiles(soundsFolderPath, "*", SearchOption.TopDirectoryOnly);

            // TODO: Make buttons dynamically based on amount of files in the sounds folder.
            buttons = FindButtons(7, 8);

            SetupButtonsTextContent(soundFiles, buttons);

            LocalVolumeSlider.Value = 25;
            CableVolumeSlider.Value = 50;
        }

        private void Button_Click(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Button senderButton = (Button)sender;
                if (senderButton.Content == null)
                {
                    return;
                }

                string filePath = $"{soundsFolderPath}{senderButton.Content}.mp3";

                audioEngine.PlayAudio(filePath, cableVolume, localVolume);
            }
        }

        private void ButtonStop_Click(object sender, MouseButtonEventArgs e)
        {
            audioEngine.StopWaveOut();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Find all the instances typeof(Button) and return a list with them.
        /// Buttons must be named: "Button{i}" for this function to find them starting with "Button1"
        /// </summary>
        /// <param name="columns"> Number of Columns. </param>
        /// <param name="rows"> Number of Rows. </param>
        /// <returns></returns>
        private List<Button> FindButtons(int columns, int rows)
        {
            List<Button> buttons = new List<Button>(columns * rows);

            for (int i = 0; i < (columns * rows); i++)
            {
                Button? buttonFound = ButtonsGrid.Children.OfType<Button>().FirstOrDefault(button => button.Name == $"Button{i + 1}");
                if (buttonFound != null)
                {
                    buttons.Add(buttonFound);
                }
            }

            return buttons;
        }

        /// <summary>
        /// Setup Buttons Text Content to be the same as the Sound File name.
        /// </summary>
        /// <param name="soundFilesArray"> Array of Sound Files found in a directory. </param>
        /// <param name="buttonsList"> List of Buttons to setup the name. </param>
        private void SetupButtonsTextContent(string[] soundFilesArray, List<Button> buttonsList)
        {
            for (int i = 0; i < soundFilesArray.Length; i++)
            {
                string[] soundFile = soundFilesArray[i].Split(@"\");
                buttonsList[i].Content = soundFile.Last().Replace(".mp3", "");
            }
        }

        /// <summary>
        /// Try to find the Sounds folder inside the application root folder.
        /// If it doens't exist, create a new one.
        /// </summary>
        /// <param name="folderName"> The folder String Path to create. ".\folderName" can be used to create a folder in the application root folder.</param>
        private void SetupSoundsFolder(string folderName)
        {
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }
        }

        /// <summary>
        /// Event when window is closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            audioEngine.DisposeWaveOut();
        }

        /// <summary>
        /// Event when CheckBox is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AudioOverlapCheckBox_Click(object sender, RoutedEventArgs e)
        {
            audioEngine.CanAudioOverlap = (bool)AudioOverlapCheckBox.IsChecked;
        }

        /// <summary>
        /// Event when Cable Volume Slider changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CableVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            cableVolume = (float)CableVolumeSlider.Value / 100.0f;
        }

        /// <summary>
        /// Event when Local Volume Slider changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LocalVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            localVolume = (float)LocalVolumeSlider.Value / 100.0f;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void ExitButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Close();
            }
        }

        private void MinimizeButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                WindowState = WindowState.Minimized;
            }
        }
    }
}
