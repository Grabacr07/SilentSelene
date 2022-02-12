// Original => https://github.com/windows-toolkit/WindowsCommunityToolkit/blob/ff113729d8e954239eeaf058d4dac28663c28002/Microsoft.Toolkit.Uwp.UI.Controls/RadialProgressBar/RadialProgressBar.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

// ReSharper disable InconsistentNaming

namespace SilentSelene.UI.Controls;

/// <summary>
/// An alternative implementation of a progress bar.
/// Progression is represented by a loop filling up in a clockwise fashion.
/// Like the traditional progress bar, it inherits from RangeBase, so Minimum, Maximum and Value properties work the same way.
/// </summary>
[TemplatePart(Name = OutlineFigurePartName, Type = typeof(PathFigure))]
[TemplatePart(Name = OutlineArcPartName, Type = typeof(ArcSegment))]
[TemplatePart(Name = BarFigurePartName, Type = typeof(PathFigure))]
[TemplatePart(Name = BarArcPartName, Type = typeof(ArcSegment))]
public class RadialProgressBar : ProgressBar
{
    private const string OutlineFigurePartName = "OutlineFigurePart";
    private const string OutlineArcPartName = "OutlineArcPart";
    private const string BarFigurePartName = "BarFigurePart";
    private const string BarArcPartName = "BarArcPart";

    static RadialProgressBar()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(RadialProgressBar), new FrameworkPropertyMetadata(typeof(RadialProgressBar)));
    }

    private PathFigure? outlineFigure;
    private PathFigure? barFigure;
    private ArcSegment? outlineArc;
    private ArcSegment? barArc;

    private bool allTemplatePartsDefined;

    /// <summary>
    /// Called when the Minimum property changes.
    /// </summary>
    /// <param name="oldMinimum">Old value of the Minimum property.</param>
    /// <param name="newMinimum">New value of the Minimum property.</param>
    protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
    {
        base.OnMinimumChanged(oldMinimum, newMinimum);
        this.RenderSegment();
    }

    /// <summary>
    /// Called when the Maximum property changes.
    /// </summary>
    /// <param name="oldMaximum">Old value of the Maximum property.</param>
    /// <param name="newMaximum">New value of the Maximum property.</param>
    protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
    {
        base.OnMaximumChanged(oldMaximum, newMaximum);
        this.RenderSegment();
    }

    /// <summary>
    /// Called when the Value property changes.
    /// </summary>
    /// <param name="oldValue">Old value of the Value property.</param>
    /// <param name="newValue">New value of the Value property.</param>
    protected override void OnValueChanged(double oldValue, double newValue)
    {
        base.OnValueChanged(oldValue, newValue);
        this.RenderSegment();
    }

    /// <summary>
    /// Update the visual state of the control when its template is changed.
    /// </summary>
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        this.outlineFigure = this.GetTemplateChild(OutlineFigurePartName) as PathFigure ?? throw new InvalidOperationException();
        this.outlineArc = this.GetTemplateChild(OutlineArcPartName) as ArcSegment ?? throw new InvalidOperationException();
        this.barFigure = this.GetTemplateChild(BarFigurePartName) as PathFigure ?? throw new InvalidOperationException();
        this.barArc = this.GetTemplateChild(BarArcPartName) as ArcSegment ?? throw new InvalidOperationException();

        this.allTemplatePartsDefined = this.outlineFigure != null && this.outlineArc != null && this.barFigure != null && this.barArc != null;

        this.RenderAll();
    }

    /// <summary>
    /// Gets or sets the thickness of the circular outline and segment
    /// </summary>
    public double Thickness
    {
        get { return (double)this.GetValue(ThicknessProperty); }
        set { this.SetValue(ThicknessProperty, value); }
    }

    /// <summary>
    /// Identifies the Thickness dependency property
    /// </summary>
    public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(nameof(Thickness), typeof(double), typeof(RadialProgressBar), new PropertyMetadata(0.0, ThicknessChangedHandler));

    /// <summary>
    /// Gets or sets the color of the circular outline on which the segment is drawn
    /// </summary>
    public Brush Outline
    {
        get { return (Brush)this.GetValue(OutlineProperty); }
        set { this.SetValue(OutlineProperty, value); }
    }

    /// <summary>
    /// Identifies the Outline dependency property
    /// </summary>
    public static readonly DependencyProperty OutlineProperty = DependencyProperty.Register(nameof(Outline), typeof(Brush), typeof(RadialProgressBar), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

    /// <summary>
    /// Initializes a new instance of the <see cref="RadialProgressBar"/> class.
    /// Create a default circular progress bar
    /// </summary>
    public RadialProgressBar()
    {
        this.DefaultStyleKey = typeof(RadialProgressBar);
        this.SizeChanged += this.SizeChangedHandler;
    }

    // Render outline and progress segment when thickness is changed
    private static void ThicknessChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RadialProgressBar sender) sender.RenderAll();
    }

    // Render outline and progress segment when control is resized.
    private void SizeChangedHandler(object sender, SizeChangedEventArgs e)
    {
        if (sender is RadialProgressBar self) self.RenderAll();
    }

    private double ComputeNormalizedRange()
    {
        var range = this.Maximum - this.Minimum;
        var delta = this.Value - this.Minimum;
        var output = range == 0.0 ? 0.0 : delta / range;
        output = Math.Min(Math.Max(0.0, output), 0.9999);
        return output;
    }

    // Compute size of ellipse so that the outer edge touches the bounding rectangle
    private Size ComputeEllipseSize()
    {
        var safeThickness = Math.Max(this.Thickness, 0.0);
        var width = Math.Max((this.ActualWidth - safeThickness) / 2.0, 0.0);
        var height = Math.Max((this.ActualHeight - safeThickness) / 2.0, 0.0);
        return new Size(width, height);
    }

    // Render the segment representing progress ratio.
    private void RenderSegment()
    {
        if (!this.allTemplatePartsDefined)
        {
            return;
        }

        var normalizedRange = this.ComputeNormalizedRange();

        var angle = 2 * Math.PI * normalizedRange;
        var size = this.ComputeEllipseSize();
        var translationFactor = Math.Max(this.Thickness / 2.0, 0.0);

        var x = (Math.Sin(angle) * size.Width) + size.Width + translationFactor;
        var y = (((Math.Cos(angle) * size.Height) - size.Height) * -1) + translationFactor;

        this.barArc!.IsLargeArc = angle >= Math.PI;
        this.barArc.Point = new Point(x, y);
    }

    // Render the progress segment and the loop outline. Needs to run when control is resized or retemplated
    private void RenderAll()
    {
        if (!this.allTemplatePartsDefined)
        {
            return;
        }

        var size = this.ComputeEllipseSize();
        var segmentWidth = size.Width;
        var translationFactor = Math.Max(this.Thickness / 2.0, 0.0);

        this.outlineFigure!.StartPoint = this.barFigure!.StartPoint = new Point(segmentWidth + translationFactor, translationFactor);
        this.outlineArc!.Size = this.barArc!.Size = new Size(segmentWidth, size.Height);
        this.outlineArc.Point = new Point(segmentWidth + translationFactor - 0.05, translationFactor);

        this.RenderSegment();
    }
}
