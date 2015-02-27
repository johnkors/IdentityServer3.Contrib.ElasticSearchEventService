using Serilog.Events;

namespace Unittests.Extensions
{
    public static class LogEventExtensions
    {
        public static T GetProperty<T>(this LogEvent evt, string key) where T : LogEventPropertyValue
        {
            var value = evt.Properties[key];
            return (T)value;
        }
    }
}