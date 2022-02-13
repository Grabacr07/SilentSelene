#if DEBUG

using System;
using JetBrains.Annotations;
using SilentSelene.Core;
using SilentSelene.Properties;

namespace SilentSelene.UI.Timer.Bindings;

public class DesignTimeWindow : Window
{
    [UsedImplicitly]
    public DesignTimeWindow()
        : base(INotifier.Default, UserSettings.Default) { }
}

#endif
