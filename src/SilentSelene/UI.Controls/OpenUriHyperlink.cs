﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace SilentSelene.UI.Controls;

public class OpenUriHyperlink : Hyperlink
{
    #region Uri dependency property

    public static readonly DependencyProperty UriProperty
        = DependencyProperty.Register(
            nameof(Uri),
            typeof(Uri),
            typeof(OpenUriHyperlink),
            new PropertyMetadata(default(Uri)));

    public Uri? Uri
    {
        get => (Uri?)this.GetValue(UriProperty);
        set => this.SetValue(UriProperty, value);
    }

    #endregion

    protected override void OnClick()
    {
        base.OnClick();

        if (this.Uri == null) return;

        var psi = new ProcessStartInfo()
        {
            FileName = this.Uri.ToString(),
            UseShellExecute = true,
        };

        try
        {
            Process.Start(psi);
        }
        catch (Exception ex)
        {
            // ToDo:
        }
    }
}
