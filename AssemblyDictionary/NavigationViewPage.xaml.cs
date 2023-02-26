using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using WinUI3Utilities;

namespace AssemblyDictionary;

public sealed partial class NavigationViewPage : Page
{
    public NavigationViewPage() => InitializeComponent();

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        NavigationView.MenuItemsSource = Directory.GetFileSystemEntries(e.Parameter.To<string>())
            .Select(t => new FileItem(Path.GetFileNameWithoutExtension(t).Replace('_', '/').Replace('!', '?'), t));
    }

    private FileItem? _currentSelection;

    private void ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs e)
    {
        if (e.InvokedItem is not FileItem fi)
            return;
        TryNavigate(fi);
    }

    private void OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e)
    {
        if (sender.Text is "")
            return;
        if (NavigationView.MenuItemsSource.To<IEnumerable<FileItem>>().FirstOrDefault(t => t.Name.StartsWith(sender.Text, StringComparison.OrdinalIgnoreCase)) is not { } fi)
            return;
        TryNavigate(fi);
        // TODO: 无法自动切换Pane选项
        NavigationView.SelectedItem = _currentSelection;
    }
    private void OnKeyUp(object sender, KeyRoutedEventArgs e)
    {
        if (!NavigationView.IsBackEnabled)
            return;
        // TODO: 接受不到事件
        if (e.Key is VirtualKey.GoBack)
            BackRequested(null!, null!);
    }

    private void BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs e)
    {
        Frame.GoBack();
        _currentSelection = NavigationView.MenuItemsSource.To<IEnumerable<FileItem>>().First(t => t.Fullname == Frame.Content.To<TextBlockPage>().Parameter);
        NavigationView.Header = _currentSelection.Name;
    }

    private void TryNavigate(FileItem item)
    {
        if (item.Fullname == _currentSelection?.Fullname)
            return;
        _currentSelection = item;
        if (Directory.Exists(item.Fullname))
            _ = Frame.Navigate(typeof(TextBlockPage), item.Fullname);
        else if (File.Exists(path: item.Fullname))
            _ = Frame.Navigate(typeof(TextBlockPage), item.Fullname);
        NavigationView.Header = item.Name;
    }
}
