using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
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
            .SelectMany(t => Path.GetFileNameWithoutExtension(t)
                .Replace("%2F", "/", StringComparison.OrdinalIgnoreCase)
                .Replace("%3F", "?", StringComparison.OrdinalIgnoreCase)
                .Replace("%26", "&")
                .Split('&')
                .Select(s => new FileItem(s, t)))
            .OrderBy(t => t.Name);
        _firstTime = true;
    }

    private bool _firstTime;

    private Dictionary<FileItem, NavigationViewItem> _items = null!;

    private FileItem? _currentSelection;

    private void ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs e)
    {
        if (_firstTime)
            _items = NavigationView.FindDescendants().OfType<NavigationViewItem>().ToDictionary(t => t.GetTag<FileItem>(), t => t);
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
        TryNavigateAndInvoke(fi);
    }

    private void BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs e)
    {
        NavFrame.GoBack();
        _currentSelection = NavFrame.Content.To<TextBlockPage>().FileItem;
        NavigationView.SelectedItem = _items[_currentSelection];
        NavigationView.Header = _currentSelection.Name;
    }

    #region 快捷键

    private void PageUp(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs e)
    {
        if (_items.Count is 0)
            return;
        var last = _items.Last();
        foreach (var item in _items)
            if (item.Key == NavigationView.SelectedItem.To<FileItem>())
            {
                TryNavigateAndInvoke(last.Key, last.Value);
                return;
            }
            else
                last = item;
    }

    private void PageDown(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs e)
    {
        if (_items.Count is 0)
            return;
        var found = false;
        foreach (var (fi, nvi) in _items)
            if (found)
            {
                TryNavigateAndInvoke(fi, nvi);
                return;
            }
            else if (fi == NavigationView.SelectedItem.To<FileItem>())
                found = true;

        var item = _items.First();
        TryNavigateAndInvoke(item.Key, item.Value);
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

    #region 操作

    [MemberNotNull(nameof(_currentSelection))]
    private void TryNavigate(FileItem item)
    {
        NavigationView.Header = item.Name;
        if (item.Fullname == _currentSelection?.Fullname)
            return;
        _currentSelection = item;
        if (Directory.Exists(item.Fullname))
            _ = NavFrame.Navigate(typeof(TextBlockPage), item);
        else if (File.Exists(item.Fullname))
            _ = NavFrame.Navigate(typeof(TextBlockPage), item);
    }

    [MemberNotNull(nameof(_currentSelection))]
    private void TryNavigateAndInvoke(FileItem fi, NavigationViewItem? nvi = null)
    {
        NavigationView.SelectedItem = nvi ?? _items[fi];
        TryNavigate(fi);
    }

    #endregion
}
