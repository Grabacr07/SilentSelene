﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;

namespace SilentSelene.Properties;

internal static class SettingsUtil
{
    public static void Update()
    {
        if (UserSettings.Default.AssemblyVersion < AssemblyInfo.Version)
        {
            UserSettings.Default.Upgrade();
            UserSettings.Default.AssemblyVersion = AssemblyInfo.Version;
            UserSettings.Default.Save();
        }
    }
}

internal static class SettingsUtil<TSettings>
    where TSettings : ApplicationSettingsBase
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly Subject<Unit> _trigger = new();
    private static TSettings? _instance;

    public static TSettings Instance
    {
        set
        {
            if (_instance == null)
            {
                _instance = value;
                _trigger.Subscribe(_ => _instance.Save());
            }
        }
    }

    public static void Save()
        => _trigger.OnNext(Unit.Default);
}

internal partial class UserSettings
{
    [UserScopedSetting]
    [DefaultSettingValue("0.0")]
    public Version AssemblyVersion
    {
        get => (Version)this[nameof(this.AssemblyVersion)];
        set => this[nameof(this.AssemblyVersion)] = value;
    }
}
