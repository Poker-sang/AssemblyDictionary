using System.IO;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using WinUI3Utilities;

namespace AssemblyDictionary;

public sealed partial class TextBlockPage : Page
{
    public TextBlockPage() => InitializeComponent();

    public TextBox TextBlockContent => Content.To<ScrollViewer>().Content.To<TextBox>();

    protected override void OnNavigatedTo(NavigationEventArgs e) => TextBlockContent.Text = File.ReadAllText(e.Parameter.To<string>());
}
