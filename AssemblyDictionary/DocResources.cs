using System;
using System.IO;
using Windows.ApplicationModel;
using WinUI3Utilities;

namespace AssemblyDictionary;

public static class DocResources
{
    public static readonly string ResourcePath = GetAssetsItem("AsslanDAT");

    public static readonly string IconPath = GetAssetsItem("icon.targetsize-32.png");

    public static readonly string IconIcoPath = GetAssetsItem("icon.targetsize-32.ico");

    public static string GetAssetsItem(string name) => ApplicationUriToPath(new("ms-appx:///Assets/" + name));

    public static string ApplicationUriToPath(Uri uri)
    {
        if (uri.Scheme is not "ms-appx")
        {
            // ms-appdata is handled by the caller.
            ThrowHelper.InvalidOperation("Uri is not using the ms-appx scheme");
        }

        var path = Uri.UnescapeDataString(uri.PathAndQuery).TrimStart('/');

        return Path.Combine(Package.Current.InstalledPath, uri.Host, path);
    }
}
