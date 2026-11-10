using DriveNow.Domain.Interfaces;

namespace DriveNow.Infrastructure.Logging;

public class AuditLog : IAuditLog
{
    readonly TimeProvider _timeProvider;

    public AuditLog(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public void Write(string message)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "audit.log");
        File.AppendAllText(path, $"[{_timeProvider.GetLocalNow():O}] {message}{Environment.NewLine}");
    }
}
