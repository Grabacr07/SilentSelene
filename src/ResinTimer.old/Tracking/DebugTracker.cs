#if DEBUG

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ResinTimer.Tracking
{
    public class DebugTracker : ITracker
    {
        public void TrackException(
            object sender,
            Exception exception,
            [CallerFilePath] string callerFilePath = default!,
            [CallerLineNumber] int callerLineNumber = default)
        {
            var senderType = sender is Type t ? t : sender.GetType();

            Debug.WriteLine("--- Exception Tracked ---");
            Debug.WriteLine($"Sender: {senderType.FullName ?? senderType.Name}");
            Debug.WriteLine($"Caller: {callerFilePath}:{callerLineNumber}");
            Debug.WriteLine(exception);
        }
    }
}

#endif
