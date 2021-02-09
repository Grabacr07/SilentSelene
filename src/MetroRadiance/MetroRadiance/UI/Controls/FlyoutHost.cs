using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MetroRadiance.UI.Controls
{
    public class FlyoutHost : ItemsControl
    {
        static FlyoutHost()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlyoutHost), new FrameworkPropertyMetadata(typeof(FlyoutHost)));
        }
    }
}
