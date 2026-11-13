using DriveNow.Domain.Interfaces;

namespace DriveNow.Infrastructure.External;

public class SmsSender : ISmsSender
{
    readonly Action<TimeSpan> _delay;
    readonly TextWriter _output;

    public SmsSender(Action<TimeSpan> delay, TextWriter output)
    {
        _delay = delay;
        _output = output;
    }

    public void Send(string phone, string text)
    {
        _delay(TimeSpan.FromMilliseconds(500));
        _output.WriteLine($"[SMS to {phone}] {text}");
    }
}
