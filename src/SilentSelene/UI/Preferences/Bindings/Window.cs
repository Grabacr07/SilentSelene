using System;
using System.Collections.Generic;
using System.Linq;
using MetroTrilithon.Mvvm;

namespace SilentSelene.UI.Preferences.Bindings;

public class Window : WindowBase
{
    public Generals Generals { get; }
        = new();

    public Accounts Accounts { get; }
        = new();
}
