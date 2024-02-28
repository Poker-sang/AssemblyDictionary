using System.IO;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AssemblyDictionary;

public record FileItem(string Name, string Fullname);

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        NavigationView.MenuItemsSource = Directory.GetFileSystemEntries(DocResources.ResourcePath).Select(t => new FileItem(Path.GetFileNameWithoutExtension(t), t));
    }

    private string _lastPath = "";

    private void ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs e)
    {
        if (e.InvokedItem is FileItem fi && fi.Fullname != _lastPath)
            _lastPath = fi.Fullname;
        else
            return;
        if (Directory.Exists(_lastPath))
            _ = NavigateFrame.Navigate(typeof(NavigationViewPage), _lastPath);
        else if (File.Exists(_lastPath))
            _ = NavigateFrame.Navigate(typeof(TextBlockPage), fi);
    }
}
