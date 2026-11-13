namespace DriveNow.Domain.Interfaces;

public interface IEmailSender
{
    void Send(string to, string subject, string body);
}
