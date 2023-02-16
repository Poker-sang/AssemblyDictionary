using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using WinUI3Utilities;

namespace AssemblyDictionary;

public sealed partial class NavigationViewPage : Page
{
    public NavigationViewPage() => InitializeComponent();

    protected override void OnNavigatedTo(NavigationEventArgs e) =>
        NavigationView.MenuItemsSource = Directory.GetFileSystemEntries(e.Parameter.To<string>())
            .Select(t => new FileItem(Path.GetFileNameWithoutExtension(t).Replace('_', '/').Replace('!', '?'), t));

    private FileItem? _lastFileItem;

    private void ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs e)
    {
        if (e.InvokedItem is FileItem fi && fi.Fullname != _lastFileItem?.Fullname)
            _lastFileItem = fi;
        else
            return;
        if (Directory.Exists(_lastFileItem.Fullname))
            _ = Frame.Navigate(typeof(TextBlockPage), _lastFileItem.Fullname);
        else if (File.Exists(_lastFileItem.Fullname))
            _ = Frame.Navigate(typeof(TextBlockPage), _lastFileItem.Fullname);
        sender.Header = _lastFileItem.Name;
    }
}
