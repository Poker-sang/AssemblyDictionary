using Microsoft.UI.Xaml;
using WinUI3Utilities;

namespace AssemblyDictionary;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        CurrentContext.Title = "指令字典";
    }

    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {
        _ = new MainWindow();
        AppHelper.Initialize(new()
        {
            Size = WindowHelper.EstimatedWindowSize()
        });
        CurrentContext.App.Resources["NavigationViewContentMargin"] = new Thickness(0);
    }
}
