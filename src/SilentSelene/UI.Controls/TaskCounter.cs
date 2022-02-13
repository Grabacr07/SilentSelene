using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SilentSelene.UI.Controls.Primitives;

namespace SilentSelene.UI.Controls;

[TemplatePart(Name = PART_ItemsHost, Type = typeof(ItemsControl))]
public class TaskCounter : Control
{
    private const string PART_ItemsHost = nameof(PART_ItemsHost);

    static TaskCounter()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(TaskCounter), new FrameworkPropertyMetadata(typeof(TaskCounter)));
    }

    private ItemsControl? _host;

    #region Total dependency property

    public static readonly DependencyProperty TotalProperty
        = DependencyProperty.Register(
            nameof(Total),
            typeof(int),
            typeof(TaskCounter),
            new PropertyMetadata(4, HandlePropertyChanged));

    public int Total
    {
        get => (int)this.GetValue(TotalProperty);
        set => this.SetValue(TotalProperty, value);
    }

    #endregion

    #region Finished dependency property

    public static readonly DependencyProperty FinishedProperty
        = DependencyProperty.Register(
            nameof(Finished),
            typeof(int),
            typeof(TaskCounter),
            new PropertyMetadata(default(int), HandlePropertyChanged));

    public int Finished
    {
        get => (int)this.GetValue(FinishedProperty);
        set => this.SetValue(FinishedProperty, value);
    }

    private static void HandlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TaskCounter counter) counter.UpdateChildren();
    }

    #endregion

    #region IsRewardReceived dependency property

    public static readonly DependencyProperty IsRewardReceivedProperty
        = DependencyProperty.Register(
            nameof(IsRewardReceived),
            typeof(bool?),
            typeof(TaskCounter),
            new PropertyMetadata(default(bool?)));

    public bool? IsRewardReceived
    {
        get => (bool?)this.GetValue(IsRewardReceivedProperty);
        set => this.SetValue(IsRewardReceivedProperty, value);
    }

    #endregion

    #region RewardDescription dependency property

    public static readonly DependencyProperty RewardDescriptionProperty
        = DependencyProperty.Register(
            nameof(RewardDescription),
            typeof(string),
            typeof(TaskCounter),
            new PropertyMetadata(default(string)));

    public string RewardDescription
    {
        get => (string)this.GetValue(RewardDescriptionProperty);
        set => this.SetValue(RewardDescriptionProperty, value);
    }

    #endregion

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (this.GetTemplateChild(PART_ItemsHost) is not ItemsControl panel) return;

        this._host = panel;
        this.UpdateChildren();
    }

    private void UpdateChildren()
    {
        if (this._host == null)
        {
            return;
        }

        if (this.Total <= 0)
        {
            this._host.Items.Clear();
            return;
        }

        var count = this._host.Items.Count;
        if (count < this.Total)
        {
            for (; count < this.Total; count++)
            {
                this._host.Items.Add(new CheckItem());
            }
        }
        else if (count > this.Total)
        {
            for (; count > this.Total; count--)
            {
                this._host.Items.RemoveAt(this.Total - 1);
            }
        }

        foreach (var (item, index) in this._host.Items
                     .OfType<CheckItem>()
                     .Select((item, index) => (item, index)))
        {
            item.IsChecked = index < this.Finished;
        }
    }
}
