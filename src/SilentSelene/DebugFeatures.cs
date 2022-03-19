#if DEBUG

using SilentSelene.Core;

namespace SilentSelene;

public class DebugFeatures
{
    public static void Break()
    {
    }
}

partial class App
{
    partial void Break()
    {
        //new UI.Preferences.Window().Show();
    }
}

#endif
