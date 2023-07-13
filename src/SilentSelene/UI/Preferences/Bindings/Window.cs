using System;
using System.Collections.Generic;
using System.Linq;
using MetroTrilithon.Mvvm;
using SilentSelene.Properties;

namespace SilentSelene.UI.Preferences.Bindings;

public class Window : WindowBase
{
    private readonly UserSettings _settings;

    public Generals Generals { get; }

    public Accounts Accounts { get; }

    public VersionInfo VersionInfo { get; }

    internal Window(UserSettings settings)
    {
        this.Title.Value = "Preferences";

        this._settings = settings;
        this.Generals = new Generals(settings);
        this.Accounts = new Accounts(settings);
        this.VersionInfo = new VersionInfo();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        this._settings.Save();
    }
}
