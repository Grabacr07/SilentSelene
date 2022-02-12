using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Livet.Messaging.Windows;
using Reactive.Bindings;
using SilentSelene.Mvvm;

namespace SilentSelene.UI.Timer.Bindings
{
    public class Window : WindowBase
    {
        private readonly ReactiveProperty<object> _content = new();

        public IReadOnlyReactiveProperty<object> Content
            => this._content;

        public Window()
        {
            this._content.Value = new Manual();
        }

        public void OpenPreferences()
        {
        }

        public void Minimize()
            => this.SendWindowAction(WindowAction.Minimize);
    }
}
