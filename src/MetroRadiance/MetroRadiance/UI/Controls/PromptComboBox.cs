using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MetroRadiance.UI.Controls
{
    /// <summary>
    /// 未入力時にプロンプトを表示できる <see cref="ComboBox"/> を表します。
    /// </summary>
    [TemplateVisualState(Name = "Empty", GroupName = "TextStates")]
    [TemplateVisualState(Name = "NotEmpty", GroupName = "TextStates")]
    public class PromptComboBox : ComboBox
    {
        static PromptComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PromptComboBox), new FrameworkPropertyMetadata(typeof(PromptComboBox)));
        }

        public PromptComboBox()
        {
            this.UpdateTextStates(true);
            this.SelectionChanged += (sender, e) => this.UpdateTextStates(true);
            this.GotKeyboardFocus += (sender, e) => this.UpdateTextStates(true);
            //this.KeyDown += (sender, e) => this.UpdateTextStates(true);
            //this.KeyUp += (sender, e) => this.UpdateTextStates(true);
        }


        #region Prompt dependency property

        public static readonly DependencyProperty PromptProperty
            = DependencyProperty.Register(
                nameof(Prompt),
                typeof(string),
                typeof(PromptComboBox),
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
                typeof(PromptComboBox),
                new UIPropertyMetadata(Brushes.Gray));

        public Brush PromptBrush
        {
            get => (Brush)this.GetValue(PromptBrushProperty);
            set => this.SetValue(PromptBrushProperty, value);
        }

        #endregion

        #region EditableText dependency property

        public static readonly DependencyProperty EditableTextProperty
            = DependencyProperty.Register(
                nameof(EditableText),
                typeof(string),
                typeof(PromptComboBox),
                new UIPropertyMetadata("", EditableTextChangedCallback));

        private static void EditableTextChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (PromptComboBox)d;
            instance.UpdateTextStates(true);
        }

        public string EditableText
        {
            get => (string)this.GetValue(EditableTextProperty);
            set => this.SetValue(EditableTextProperty, value);
        }

        #endregion


        private void UpdateTextStates(bool useTransitions)
        {
            VisualStateManager.GoToState(this, string.IsNullOrEmpty(this.EditableText) ? "Empty" : "NotEmpty", useTransitions);
        }
    }
}
