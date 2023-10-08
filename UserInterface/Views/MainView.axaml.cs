using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;

using Soundboard.Lib.Services;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using UserInterface.ViewModels;

namespace UserInterface.Views;

public partial class MainView : UserControl
{
    private const int _columns = 7;
    private const string _stackPanelsBaseName = "StackPanel_Buttons_";

    private readonly IAudioManagerService _audioManagerService;
    private readonly MainViewModel _viewModel;

    public MainView(IAudioManagerService audioManagerService)
    {
        _audioManagerService = audioManagerService;
        _viewModel = new MainViewModel(_audioManagerService);
        DataContext = _viewModel;

        InitializeComponent();
    }

    public MainView()
    {
        _audioManagerService = new AudioManagerService();
        _viewModel = new MainViewModel(_audioManagerService);
        DataContext = _viewModel;

        InitializeComponent();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        this.SizeChanged += OnSizeChanged;

        if(_audioManagerService != null)
        {            
            InitializeButtons();
            return;
        }

        Debug.WriteLine("Couldn't find the Grid_Buttons grid.");
    }

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

    private string GetButtonNameFromFileName(string fileName)
    {
        return $"Button_{fileName.Replace(' ', '_')}";
    }
}
