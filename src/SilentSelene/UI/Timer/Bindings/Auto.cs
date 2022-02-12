using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Livet;
using Reactive.Bindings;
using SilentSelene.Core;

namespace SilentSelene.UI.Timer.Bindings;

public class Auto : ViewModel
{
    public RealtimeNote RealtimeNote { get; }

    public IReadOnlyReactiveProperty<bool> IsActive { get; }

    public Auto(RealtimeNote realtimeNote)
    {
        this.RealtimeNote = realtimeNote;
        this.IsActive = realtimeNote.HasError
            .Select(x => x == false)
            .ToReadOnlyReactiveProperty();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            this.RealtimeNote.Dispose();
        }
    }
}
