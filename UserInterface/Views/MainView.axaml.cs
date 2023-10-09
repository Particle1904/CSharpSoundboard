using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using Avalonia.VisualTree;

using Soundboard.Lib.Services;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using UserInterface.ViewModels;

namespace UserInterface.Views;

/// <summary>
/// The main view that displays buttons for audio playback.
/// </summary>
public partial class MainView : UserControl
{
    private const int _columns = 7;
    private const string _stackPanelsBaseName = "StackPanel_Buttons_";

    private readonly IAudioManagerService _audioManagerService;
    private readonly MainViewModel _viewModel;
    private IStorageProvider _storageProvider;
    private TopLevel _topLevel;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainView"/> class.
    /// </summary>
    /// <param name="audioManagerService">The audio manager service used for audio playback.</param>
    public MainView(IAudioManagerService audioManagerService)
    {
        _audioManagerService = audioManagerService;
        _viewModel = new MainViewModel(_audioManagerService);
        DataContext = _viewModel;
        InitializeComponent();
    }

    /// <summary>
    /// Default constructor required for the IDE Designer to work.
    /// </summary>
    public MainView()
    {
        _audioManagerService = new AudioManagerService();
        _viewModel = new MainViewModel(_audioManagerService);
        DataContext = _viewModel;

        InitializeComponent();
    }

    /// <summary>
    /// Handles the initialization of the view.
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        this.SizeChanged += OnSizeChanged;
        this.Loaded += OnLoaded;
        _viewModel.FolderPathChanged += OnFolderPathChanged;

        if (_audioManagerService != null)
        {
            InitializeButtons();
        }
    }

    /// <summary>
    /// Handles the event when the folder path changes.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">Event arguments.</param>
    private void OnFolderPathChanged(object sender, EventArgs e)
    {
        RemoveButtons();
        InitializeButtons();
    }

    /// <summary>
    /// Handles the event when the view is loaded.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">Event arguments.</param>
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _topLevel = TopLevel.GetTopLevel(this);
        _storageProvider = _topLevel.StorageProvider;
        _viewModel.SetStorageProvider(_storageProvider);
    }

    /// <summary>
    /// Handles the event when the view size changes.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">Event arguments.</param>
    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        IEnumerable<Button> buttons = this.FindControl<Grid>("Grid_Buttons").GetVisualDescendants().OfType<Button>();

        foreach (Button button in buttons)
        {
            FormattedText formattedText = new FormattedText(button.Content.ToString(), CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight, Typeface.Default, button.FontSize, null);

            double desiredWidth = button.Bounds.Width - button.Padding.Left - button.Padding.Right;
            double scaleFactor = desiredWidth / formattedText.Width;

            scaleFactor = Math.Max(0.5, Math.Min(2.0, scaleFactor));

            button.FontSize = Math.Clamp(button.FontSize * scaleFactor, 10, 20) - 2.0f;
        }
    }

    /// <summary>
    /// Initializes the buttons based on audio files.
    /// </summary>
    private void InitializeButtons()
    {
        string[] fileNames = _audioManagerService.FileNames;

        int currentColumn = 0;
        for (int i = 0; i < fileNames.Length; i++)
        {
            StackPanel currentStackPanel = this.FindControl<StackPanel>($"{_stackPanelsBaseName}{currentColumn}");
            Button button = new Button()
            {
                Name = GetButtonNameFromFileName(fileNames[i]),
                Content = fileNames[i],
                MaxHeight = 40,
                CommandParameter = fileNames[i],
                Command = _viewModel.ButtonClickCommand
            };

            currentStackPanel.Children.Add(button);

            currentColumn++;

            if (currentColumn >= _columns)
            {
                currentColumn = 0;
            }
        }
    }

    /// <summary>
    /// Removes all buttons from the view that are attached to a StackPanel.
    /// </summary>
    private void RemoveButtons()
    {
        for (int i = 0; i < _columns; i++)
        {
            StackPanel currentStackPanel = this.FindControl<StackPanel>($"{_stackPanelsBaseName}{i}");
            IEnumerable<Button> buttons = currentStackPanel.GetVisualDescendants().OfType<Button>();
            currentStackPanel.Children.RemoveAll(buttons);
        }
    }

    /// <summary>
    /// Generates a button name from a file name.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <returns>The generated button name.</returns>
    private string GetButtonNameFromFileName(string fileName)
    {
        return $"Button_{fileName.Replace(' ', '_')}";
    }

    /// <summary>
    /// Handles cleanup when the view is unloaded.
    /// </summary>
    /// <param name="e">Event arguments.</param>s
    protected override void OnUnloaded(RoutedEventArgs e)
    {
        _viewModel.FolderPathChanged -= OnFolderPathChanged;
        _audioManagerService.FreeResources();
        base.OnUnloaded(e);
    }
}