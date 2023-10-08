using Avalonia;
using Avalonia.Controls;

using Soundboard.Lib.Services;

namespace UserInterface.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

#if DEBUG
        this.AttachDevTools();
#endif
    }
}
