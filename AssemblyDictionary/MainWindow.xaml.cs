using System.IO;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI3Utilities;

namespace AssemblyDictionary;

public record FileItem(string Name, string Fullname);

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        CurrentContext.Window = this;
        InitializeComponent();
        CurrentContext.TitleBar = TitleBar;
        CurrentContext.TitleTextBlock = TitleTextBlock;
        CurrentContext.NavigationView = NavigationView;
        CurrentContext.Frame = NavigationView.Content.To<Frame>();
        //CurrentContext.AppTitleBar.SetDragRectangles();

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
            NavigationHelper.GotoPage<NavigationViewPage>(_lastPath);
        else if (File.Exists(_lastPath))
            NavigationHelper.GotoPage<TextBlockPage>(_lastPath);
    }
}
