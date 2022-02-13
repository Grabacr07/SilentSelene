#if DEBUG

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SilentSelene.Reporting;

public class DebugReporter : IReporter
{
    public void ReportException(
        object sender,
        Exception exception,
        [CallerFilePath] string? callerFilePath = default,
        [CallerLineNumber] int callerLineNumber = default)
    {
        var senderType = sender is Type t ? t : sender.GetType();

        Debug.WriteLine("--- Exception Reported ---");
        Debug.WriteLine($"Sender: {senderType.FullName ?? senderType.Name}");
        Debug.WriteLine($"Caller: {callerFilePath}:{callerLineNumber}");
        Debug.WriteLine(exception);
    }
}

#endif
