using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace MetroRadiance.UI.Controls
{
    public interface IFlyout
    {
        bool IsOverlay { get; }

        event EventHandler Close;
    }

    public class FlyoutManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        internal ObservableCollection<IFlyout> Items { get; }

        #region ThroughInput notification property

        private bool _ThroughInput = true;

        public bool ThroughInput
        {
            get => this._ThroughInput;
            private set
            {
                if (this._ThroughInput == value) return;
                this._ThroughInput = value;
                this.RaisePropertyChanged();
            }
        }

        #endregion

        internal FlyoutManager()
        {
            this.Items = new ObservableCollection<IFlyout>();
            this.Items.CollectionChanged += this.HandleItemsChanged;
        }

        public void Show(IFlyout flyout)
        {
            this.Items.Add(flyout);

            flyout.Close += HandleClose;

            void HandleClose(object? sender, EventArgs args)
            {
                this.Items.Remove(flyout);
                flyout.Close -= HandleClose;
            }
        }

        private void HandleItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.ThroughInput = this.Items.Any(x => x.IsOverlay) == false;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
