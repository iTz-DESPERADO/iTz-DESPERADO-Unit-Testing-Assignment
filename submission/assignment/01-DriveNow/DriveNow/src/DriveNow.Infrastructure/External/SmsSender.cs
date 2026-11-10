using DriveNow.Domain.Interfaces;

namespace DriveNow.Infrastructure.External;

public class SmsSender : ISmsSender
{
    readonly Action<TimeSpan> _delay;

    public SmsSender(Action<TimeSpan> delay)
    {
        _delay = delay;
    }

    public void Send(string phone, string text)
    {
        _delay(TimeSpan.FromMilliseconds(500));
        Console.WriteLine($"[SMS to {phone}] {text}");
    }
}
