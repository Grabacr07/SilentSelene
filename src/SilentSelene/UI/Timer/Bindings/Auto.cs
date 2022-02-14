using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Livet;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SilentSelene.Core;

namespace SilentSelene.UI.Timer.Bindings;

public class Auto : ViewModel
{
    public RealtimeNote RealtimeNote { get; }

    public IReadOnlyReactiveProperty<bool> IsActive { get; }

    public Auto(RealtimeNote realtimeNote)
    {
        this.RealtimeNote = realtimeNote;
        this.IsActive = realtimeNote.Status
            .Select(x => x == RealtimeNoteStatus.Active)
            .ToReadOnlyReactiveProperty()
            .AddTo(this.CompositeDisposable);
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
