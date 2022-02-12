using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ResinTimer.UI.Hosting;

namespace ResinTimer.UI
{
    [MainContent("アプリ設定", "\xE713", MainContent.Categories.Option, 1000), UsedImplicitly]
    public partial class SettingsView
    {
        public SettingsView()
        {
            this.InitializeComponent();
        }
    }
}
