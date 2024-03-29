﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Livet;
using Reactive.Bindings;
using SilentSelene.Core;

namespace SilentSelene.UI.Timer.Bindings;

public class Manual : ViewModel
{
    public ResinTimer ResinTimer { get; }

    public IReactiveProperty<string> NewResin { get; }
    
    public Manual(ResinTimer resinTimer)
    {
        this.ResinTimer = resinTimer;
        this.NewResin = new ReactiveProperty<string>();
    }

    [UsedImplicitly]
    public void Update()
    {
        if (int.TryParse(this.NewResin.Value, out var resin))
        {
            this.ResinTimer.Reset(resin);
        }

        this.NewResin.Value = "";
    }

    [UsedImplicitly]
    public void Increase(string value)
    {
        if (int.TryParse(value, out var i)) this.ResinTimer.Increase(i);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            this.ResinTimer.Dispose();
        }
    }
}
