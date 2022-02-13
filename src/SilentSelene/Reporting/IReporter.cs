using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SilentSelene.Reporting;

public interface IReporter
{
    void ReportException(
        object sender,
        Exception exception,
        [CallerFilePath] string? callerFilePath = default,
        [CallerLineNumber] int callerLineNumber = default);

    public static IReporter Instance { get; }
#if DEBUG
        = new DebugReporter();
#else
        = new ApplicationInsightsReporter();
#endif
}
