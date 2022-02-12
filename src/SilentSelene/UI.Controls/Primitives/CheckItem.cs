using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MetroTrilithon.UI;

namespace SilentSelene.UI.Controls.Primitives;

public class CheckItem : Control
{
    static CheckItem()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckItem), new FrameworkPropertyMetadata(typeof(CheckItem)));
    }

    #region IsChecked dependency property

    public static readonly DependencyProperty IsCheckedProperty
        = DependencyProperty.Register(
            nameof(IsChecked),
            typeof(bool),
            typeof(CheckItem),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    public bool IsChecked
    {
        get => (bool)this.GetValue(IsCheckedProperty);
        set => this.SetValue(IsCheckedProperty, value);
    }

    #endregion
}
