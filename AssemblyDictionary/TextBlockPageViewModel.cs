using CommunityToolkit.Mvvm.ComponentModel;

namespace AssemblyDictionary;

public partial class TextBlockPageViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotEditing))]
    private bool _isEditing = true;// TODO: markdown显示不好看

    public bool IsNotEditing => !IsEditing;

    [ObservableProperty]
    private bool _isWrapping;
}
