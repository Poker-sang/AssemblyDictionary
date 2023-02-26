using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using WinUI3Utilities;

namespace AssemblyDictionary;

public sealed partial class TextBlockPage : Page
{
    public TextBlockPage() => InitializeComponent();

    public string Parameter { get; private set; } = "";

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        Parameter = e.Parameter.To<string>();
        TextBox.Text = File.ReadAllText(Parameter);
    }

    private void RefreshTapped(object sender, TappedRoutedEventArgs e)
    {
        TextBox.Text = File.ReadAllText(Parameter);
        FadeOut("已从硬盘重新读取", Parameter);
    }

    private void SaveTapped(object sender, TappedRoutedEventArgs e)
    {
        File.WriteAllText(Parameter, TextBox.Text);
        FadeOut("已保存到路径", Parameter);
    }

    #region SnackBar功能

    private DateTime _closeSnakeBarTime;

    private async void FadeOut(string title, string subtitle, int mSec = 3000)
    {
        _closeSnakeBarTime = DateTime.Now + TimeSpan.FromMicroseconds(mSec - 100);

        SnackBar.Title = title;
        SnackBar.Subtitle = subtitle;

        _ = SnackBar.IsOpen = true;
        await Task.Delay(mSec);
        if (DateTime.Now > _closeSnakeBarTime)
            _ = SnackBar.IsOpen = false;
    }

    #endregion
}
