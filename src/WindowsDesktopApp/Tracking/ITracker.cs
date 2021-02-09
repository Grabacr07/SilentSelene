using System;
using System.Runtime.CompilerServices;

namespace WindowsDesktopApp.Tracking
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
