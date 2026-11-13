namespace DriveNow.Domain.Interfaces;

public interface IAuditLog
{
    void Write(string message);
}
