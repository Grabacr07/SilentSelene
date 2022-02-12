#if DEBUG

using System.ComponentModel;
using System.Windows;

namespace SilentSelene;

public class DebugFeatures
{
    public static bool IsInDesignMode
        => DesignerProperties.GetIsInDesignMode(new DependencyObject());
}


#endif
