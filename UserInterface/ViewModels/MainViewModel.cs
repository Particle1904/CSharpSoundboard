using Avalonia.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Soundboard.Lib.Services;

using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace UserInterface.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IAudioManagerService _audioManagerService;

    [ObservableProperty]
    private string _soundsFolderPath;

    private float _localVolume;
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
    public int CableVolume
    {
        get => (int)(_cableVolume * 100.0f);

        set
        {
            _cableVolume = (float)(value / 100.0f);
            OnPropertyChanged(nameof(CableVolume));
        }
    }

    [ObservableProperty]
    private bool _canAudioOverlap;

    public MainViewModel(IAudioManagerService audioManagerService)
    {
        _audioManagerService = audioManagerService;
        SoundsFolderPath = _audioManagerService.GetDefaultSoundsFolder();

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
    
    [RelayCommand]
    public void ButtonClick(object parameter)
    {
        _audioManagerService.PlayAudio((string)parameter, _cableVolume, _cableVolume);
    }

    [RelayCommand]
    public void StopSoundAsync()
    {
        _audioManagerService.StopWave();
    }

    [RelayCommand]
    public void ToggleCanAudioOverlap()
    { 
        _audioManagerService.CanAudioOverlap = CanAudioOverlap;
    }
}
