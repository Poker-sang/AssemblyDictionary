using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using WinUI3Utilities;
using Microsoft.UI.Xaml;

namespace AssemblyDictionary;

public sealed partial class NavigationViewPage : Page
{
    public NavigationViewPage()
    {
        InitializeComponent();
    }

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

    private void TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e)
    {
        if (sender.Text is "")
            return;
        if (NavigationView.MenuItemsSource.To<IEnumerable<FileItem>>()
                .FirstOrDefault(t => t.Name.StartsWith(sender.Text, StringComparison.OrdinalIgnoreCase)) is not { } fi)
            return;
        TryNavigate(fi);
        // TODO: 无法自动切换Pane选项
        NavigationView.SelectedItem = _currentSelection;
    }

    private void BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs e)
    {
        Frame.GoBack();
        _currentSelection = NavigationView.MenuItemsSource.To<IEnumerable<FileItem>>()
            .First(t => t.Fullname == Frame.Content.To<TextBlockPage>().Parameter);
        NavigationView.Header = _currentSelection.Name;
    }

    private void TryNavigate(FileItem item)
    {
        if (item.Fullname == _currentSelection?.Fullname)
            return;
        _currentSelection = item;
        if (Directory.Exists(item.Fullname))
            _ = Frame.Navigate(typeof(TextBlockPage), item.Fullname);
        else if (File.Exists(item.Fullname))
            _ = Frame.Navigate(typeof(TextBlockPage), item.Fullname);
        NavigationView.Header = item.Name;
    }

    #region 快捷键

    private void PageUp(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs e)
    {
        var items = NavigationView.MenuItemsSource.To<IEnumerable<FileItem>>().ToArray();
        if (items.Length is 0)
            return;
        var last = items[^1];
        foreach (var fi in items)
            if (fi == NavigationView.SelectedItem.To<FileItem>())
            {
                NavigationView.SelectedItem = last;
                TryNavigate(last);
                return;
            }
            else
                last = fi;
    }

    private void PageDown(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs e)
    {
        var items = NavigationView.MenuItemsSource.To<IEnumerable<FileItem>>().ToArray();
        if (items.Length is 0)
            return;
        var found = false;
        foreach (var fi in items)
            if (found)
            {
                NavigationView.SelectedItem = fi;
                TryNavigate(fi);
                return;
            }
            else if (fi == NavigationView.SelectedItem.To<FileItem>())
                found = true;

        NavigationView.SelectedItem = items[0];
        TryNavigate(items[0]);
    }

    private void GoBack(object sender, PointerRoutedEventArgs e)
    {
        var currentPoint = e.GetCurrentPoint(sender.To<UIElement>());
        if (currentPoint.PointerDeviceType is not Microsoft.UI.Input.PointerDeviceType.Mouse)
            return;

        if (currentPoint.Properties.IsXButton1Pressed)
        {
            if (!NavigationView.IsBackEnabled)
                return;
            BackRequested(null!, null!);
        }
    }

    #endregion
}
