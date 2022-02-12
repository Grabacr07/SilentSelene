using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SilentSelene.UI.Controls;

[TemplateVisualState(Name = "Empty", GroupName = "TextStates")]
[TemplateVisualState(Name = "NotEmpty", GroupName = "TextStates")]
public class PromptTextBox : TextBox
{
    static PromptTextBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(PromptTextBox), new FrameworkPropertyMetadata(typeof(PromptTextBox)));
    }

    public PromptTextBox()
    {
        this.UpdateTextStates(true);
        this.TextChanged += (sender, e) => this.UpdateTextStates(true);
        this.GotKeyboardFocus += (sender, e) => this.UpdateTextStates(true);
    }

    #region Prompt dependency property

    public static readonly DependencyProperty PromptProperty
        = DependencyProperty.Register(
            nameof(Prompt),
            typeof(string),
            typeof(PromptTextBox),
            new UIPropertyMetadata(""));

    public string Prompt
    {
        get => (string)this.GetValue(PromptProperty);
        set => this.SetValue(PromptProperty, value);
    }

    #endregion

    #region PromptBrush dependency property

    public static readonly DependencyProperty PromptBrushProperty
        = DependencyProperty.Register(
            nameof(PromptBrush),
            typeof(Brush),
            typeof(PromptTextBox),
            new UIPropertyMetadata(Brushes.Gray));

    public Brush PromptBrush
    {
        get => (Brush)this.GetValue(PromptBrushProperty);
        set => this.SetValue(PromptBrushProperty, value);
    }

    #endregion


    private void UpdateTextStates(bool useTransitions)
    {
        VisualStateManager.GoToState(this, string.IsNullOrEmpty(this.Text) ? "Empty" : "NotEmpty", useTransitions);
    }
}
