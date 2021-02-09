using System;
using System.Windows.Input;

namespace MetroRadiance.UI.Controls
{
    public enum CommandBehavior
    {
        Normal,

        Default, // Button.IsDefault
        Cancel, // Button.IsCancel
    }

    public class ModalFlyoutMessage
    {
        public string Header { get; }

        public string Body { get; }

        public ModalFlyoutMessageCommand[] Commands { get; }

        public ModalFlyoutMessage(string header, string body, params ModalFlyoutMessageCommand[] commands)
        {
            this.Header = header;
            this.Body = body;
            this.Commands = commands;
        }
    }

    public class ModalFlyoutMessageCommand : ICommand
    {
        private readonly Action? _action;
        private readonly CommandBehavior _behavior;

        internal event Action? Executed;
        public event EventHandler? CanExecuteChanged;

        public string Label { get; }

        public bool IsDefault
            => this._behavior == CommandBehavior.Default;

        public bool IsCancel
            => this._behavior == CommandBehavior.Cancel;

        public ModalFlyoutMessageCommand(string label, Action? action, CommandBehavior behavior)
        {
            this.Label = label;
            this._action = action;
            this._behavior = behavior;
        }

        public bool CanExecute(object parameter)
            => true;

        public void Execute(object parameter)
        {
            this._action?.Invoke();
            this.Executed?.Invoke();
        }

        protected virtual void RaiseCanExecuteChanged()
            => this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);


        public static ModalFlyoutMessageCommand CreateDefault(Action? action, string label = "OK")
            => new ModalFlyoutMessageCommand(label, action, CommandBehavior.Default);

        public static ModalFlyoutMessageCommand CreateCancel(Action? action = null, string label = "キャンセル")
            => new ModalFlyoutMessageCommand(label, action, CommandBehavior.Cancel);
    }
}
