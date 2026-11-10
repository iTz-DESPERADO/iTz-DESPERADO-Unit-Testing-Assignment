using DriveNow.Domain.Interfaces;

namespace DriveNow.Infrastructure.External;

public class PaymentGateway : IPaymentGateway
{
    readonly Random _random;
    readonly Action<TimeSpan> _delay;
    readonly TextWriter _output;

    public PaymentGateway(Random random, Action<TimeSpan> delay, TextWriter output)
    {
        _random = random;
        _delay = delay;
        _output = output;
    }

    public string Charge(int customerId, decimal amount, string description)
    {
        PauseAndMaybeFail();
        _output.WriteLine($"[Payment] Charged {amount:N2} EGP to customer #{customerId}");
        return "PAY-" + RandomCode();
    }

    public void Refund(int customerId, decimal amount, string description)
    {
        PauseAndMaybeFail();
        _output.WriteLine($"[Payment] Refunded {amount:N2} EGP to customer #{customerId}");
    }

    void PauseAndMaybeFail()
    {
        _delay(TimeSpan.FromMilliseconds(1500));
        if (_random.Next(10) == 0)
            throw new PaymentFailedException("Gateway timeout");
    }

    string RandomCode()
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var chars = new char[8];
        for (var i = 0; i < chars.Length; i++)
            chars[i] = alphabet[_random.Next(alphabet.Length)];

        return new string(chars);
    }
}
