namespace DriveNow.Domain.Interfaces;

public interface ISmsSender
{
    void Send(string phone, string text);
}
