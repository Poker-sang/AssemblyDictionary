using Microsoft.UI.Xaml;
using WinUI3Utilities;

namespace AssemblyDictionary;

public partial class App : Application
{
    public const string Title = "指令字典";

    public App() => InitializeComponent();

    private static MainWindow _mainWindow = null!;

    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {
        _mainWindow = new();
        _mainWindow.Initialize(new()
        {
            Title = Title,
            IconPath = DocResources.IconIcoPath,
            Size = WindowHelper.EstimatedWindowSize()
        });
        _mainWindow.Activate();
    }
}
