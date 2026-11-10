using DriveNow.Domain.Interfaces;

namespace DriveNow.Infrastructure.External;

public class PaymentGateway : IPaymentGateway
{
    readonly Random _random;

    public PaymentGateway(Random random)
    {
        _random = random;
    }

    public string Charge(int customerId, decimal amount, string description)
    {
        PauseAndMaybeFail();
        Console.WriteLine($"[Payment] Charged {amount:N2} EGP to customer #{customerId}");
        return "PAY-" + RandomCode();
    }

    public void Refund(int customerId, decimal amount, string description)
    {
        PauseAndMaybeFail();
        Console.WriteLine($"[Payment] Refunded {amount:N2} EGP to customer #{customerId}");
    }

    void PauseAndMaybeFail()
    {
        Thread.Sleep(1500);
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
