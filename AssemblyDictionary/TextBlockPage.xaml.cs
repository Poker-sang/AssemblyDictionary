using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using WinUI3Utilities;

namespace AssemblyDictionary;

public sealed partial class TextBlockPage : Page
{
    public TextBlockPage() => InitializeComponent();

    private readonly TextBlockPageViewModel _vm = new();

    public string Parameter { get; private set; } = "";

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        Parameter = e.Parameter.To<string>();
        TextBox.Text = await File.ReadAllTextAsync(Parameter);
    }

    private async void RefreshTapped(object sender, ICustomQueryInterface e)
    {
        TextBox.Text = await File.ReadAllTextAsync(Parameter);
        FadeOut("已从硬盘重新读取", Parameter);
    }

    private async void SaveTapped(object sender, ICustomQueryInterface e)
    {
        await File.WriteAllLinesAsync(Parameter, TextBox.Text.Split("\r\n").Select(s => s.TrimEnd()));
        TextBox.Text = await File.ReadAllTextAsync(Parameter);
        FadeOut("已保存到路径", Parameter);
    }

    #region SnackBar功能

    private DateTime _closeSnakeBarTime;

    private async void FadeOut(string title, string subtitle, int mSec = 3000)
    {
        _closeSnakeBarTime = DateTime.Now + TimeSpan.FromMicroseconds(mSec - 100);

        SnackBar.Title = title;
        SnackBar.Subtitle = subtitle;

        SnackBar.IsOpen = true;
        await Task.Delay(mSec);
        if (DateTime.Now > _closeSnakeBarTime)
            SnackBar.IsOpen = false;
    }

    #endregion
}
