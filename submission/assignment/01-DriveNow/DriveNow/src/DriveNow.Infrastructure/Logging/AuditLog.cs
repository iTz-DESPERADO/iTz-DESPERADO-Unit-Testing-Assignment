using DriveNow.Domain.Interfaces;
using DriveNow.Infrastructure.Interfaces;

namespace DriveNow.Infrastructure.Logging;

public class AuditLog : IAuditLog
{
    readonly TimeProvider _timeProvider;
    readonly ITextFileWriter _files;
    readonly string _path;

    public AuditLog(TimeProvider timeProvider, ITextFileWriter files, string path)
    {
        _timeProvider = timeProvider;
        _files = files;
        _path = path;
    }

    public void Write(string message)
    {
        _files.AppendAllText(_path, $"[{_timeProvider.GetLocalNow():O}] {message}{Environment.NewLine}");
    }
}
