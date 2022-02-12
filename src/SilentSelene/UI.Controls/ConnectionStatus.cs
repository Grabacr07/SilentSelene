using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SilentSelene.UI.Controls;

public class ConnectionStatus : Control
{
    static ConnectionStatus()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ConnectionStatus), new FrameworkPropertyMetadata(typeof(ConnectionStatus)));
    }

    #region IsActive dependency property

    public static readonly DependencyProperty IsActiveProperty
        = DependencyProperty.Register(
            nameof(IsActive),
            typeof(bool),
            typeof(ConnectionStatus),
            new PropertyMetadata(default(bool)));

    public bool IsActive
    {
        get => (bool)this.GetValue(IsActiveProperty);
        set => this.SetValue(IsActiveProperty, value);
    }

    #endregion
}
