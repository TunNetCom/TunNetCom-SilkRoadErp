namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests;

public class TestLogger<T> : ILogger<T>
{
    public List<string> Logs = new List<string>();

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (formatter != null)
        {
            var message = formatter(state, exception);
            Logs.Add(message);
        }
    }
}
