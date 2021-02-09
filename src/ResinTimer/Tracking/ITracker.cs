using System;
using System.Runtime.CompilerServices;

namespace ResinTimer.Tracking
{
    public interface ITracker
    {
        void TrackException(
            object sender,
            Exception exception,
            [CallerFilePath] string callerFilePath = default!,
            [CallerLineNumber] int callerLineNumber = default);
    }
}
