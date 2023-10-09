using Avalonia.Platform.Storage;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Soundboard.Lib.Services;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UserInterface.ViewModels;

/// <summary>
/// ViewModel for the main application view.
/// </summary>
public partial class MainViewModel : ViewModelBase
{
    private readonly IAudioManagerService _audioManagerService;
    private IStorageProvider _storageProvider;
    private FolderPickerOpenOptions _folderPickerOptions;

    /// <summary>
    /// Event raised when the folder path changes.
    /// </summary>
    public EventHandler FolderPathChanged;

    [ObservableProperty]
    private string _soundsFolderPath;

    private float _localVolume;
    /// <summary>
    /// Gets or sets the local audio volume as a percentage.
    /// </summary>
    public int LocalVolume
    {
        get => (int)(_localVolume * 100.0f);

        set
        {
            _localVolume = (float)(value / 100.0f);
            OnPropertyChanged(nameof(LocalVolume));
        }
    }

    private float _cableVolume;
    /// <summary>
    /// Gets or sets the cable audio volume as a percentage.
    /// </summary>
    public int CableVolume
    {
        get => (int)(_cableVolume * 100.0f);

        set
        {
            _cableVolume = (float)(value / 100.0f);
            OnPropertyChanged(nameof(CableVolume));
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether audio can overlap.
    /// </summary>
    [ObservableProperty]
    private bool _canAudioOverlap;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    /// <param name="audioManagerService">The audio manager service.</param>
    public MainViewModel(IAudioManagerService audioManagerService)
    {
        _audioManagerService = audioManagerService;
        _folderPickerOptions = new FolderPickerOpenOptions()
        {
            AllowMultiple = false,
            Title = "Select folder with MP3 and WAV files"
        };

        SoundsFolderPath = _audioManagerService.DefaultFolderPath;

        LocalVolume = 50;
        CableVolume = 50;
        CanAudioOverlap = false;
        _audioManagerService.CanAudioOverlap = CanAudioOverlap;
    }

    /// <summary>
    /// Default Constructor is REQUIRED for the IDE Designer to work!
    /// </summary>
    public MainViewModel()
    {
        LocalVolume = 50;
        CableVolume = 50;
        CanAudioOverlap = false;
    }

    /// <summary>
    /// Sets the storage provider for the ViewModel.
    /// </summary>
    /// <param name="storageProvider">The storage provider to set.</param>
    public void SetStorageProvider(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
    }

    /// <summary>
    /// Handles the button click event.
    /// </summary>
    /// <param name="parameter">The parameter passed from the button click.</param>
    [RelayCommand]
    public void ButtonClick(object parameter)
    {
        _audioManagerService.PlayAudio((string)parameter, _cableVolume, _localVolume);
    }

    /// <summary>
    /// Stops the currently playing sound asynchronously.
    /// </summary>
    [RelayCommand]
    public void StopSoundAsync()
    {
        _audioManagerService.Stop();
    }

    /// <summary>
    /// Toggles the ability to overlap audio playback.
    /// </summary>
    [RelayCommand]
    public void ToggleCanAudioOverlap()
    {
        _audioManagerService.CanAudioOverlap = CanAudioOverlap;
    }

    /// <summary>
    /// Opens a folder picker dialog to select the sounds folder.
    /// </summary>
    [RelayCommand]
    public async Task SelectSoundsFolder()
    {
        IReadOnlyList<IStorageFolder> result = await _storageProvider.OpenFolderPickerAsync(_folderPickerOptions);
        if (result.Count > 0)
        {
            SoundsFolderPath = result[0].Path.LocalPath;
            _audioManagerService.SoundsFolderPath = SoundsFolderPath;
        }

        FolderPathChanged?.Invoke(this, EventArgs.Empty);
    }
}