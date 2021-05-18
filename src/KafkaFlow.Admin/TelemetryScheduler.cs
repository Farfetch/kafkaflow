namespace KafkaFlow.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    internal class TelemetryScheduler
    {
        private static readonly Lazy<Dictionary<string, Timer>> timers = new (() => new Dictionary<string, Timer>());

        public static void Set(string key, TimerCallback callback, TimeSpan dueTime, TimeSpan period)
        {
            Unset(key);

            timers.Value[key] = new Timer(callback, null, dueTime, period);
        }

        public static void Unset(string key)
        {
            if (timers.Value.TryGetValue(key, out var timer))
            {
                timer.Dispose();
                timers.Value.Remove(key);
            }
        }
    }
}
