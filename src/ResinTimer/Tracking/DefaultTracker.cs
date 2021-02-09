using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using ResinTimer.Properties;

namespace ResinTimer.Tracking
{
    public partial class DefaultTracker : ITracker
    {
        private readonly TelemetryClient _telemetryClient;

        public DefaultTracker()
        {
            this._telemetryClient = new TelemetryClient(TelemetryConfiguration.CreateDefault());
            this._telemetryClient.Context.Session.Id = Guid.NewGuid().ToString();
            this._telemetryClient.Context.Device.OperatingSystem = RuntimeInformation.OSDescription;
            this._telemetryClient.Context.Component.Version = AssemblyInfo.VersionString;
            this.SetInstrumentationKey();
        }

        public void TrackException(
            object sender,
            Exception exception,
            [CallerFilePath] string callerPath = default!,
            [CallerLineNumber] int callerLineNumber = default)
        {
            var senderType = sender is Type t ? t : sender.GetType();
            var properties = new Dictionary<string, string>()
            {
                { "sender", senderType.FullName ?? senderType.Name },
                { "caller", $"{callerPath}:{callerLineNumber}" }
            };

            this._telemetryClient.TrackException(exception, properties);
        }

        partial void SetInstrumentationKey();
    }
}
