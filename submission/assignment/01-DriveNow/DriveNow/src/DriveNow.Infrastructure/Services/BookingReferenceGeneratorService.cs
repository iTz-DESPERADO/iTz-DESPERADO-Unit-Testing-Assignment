using DriveNow.Domain.Interfaces;

namespace DriveNow.Infrastructure.Services;

public class BookingReferenceGeneratorService : IBookingReferenceGenerator
{
    readonly Random _random;

    public BookingReferenceGeneratorService(Random random)
    {
        _random = random;
    }

    public string Generate(DateTime pickupAt)
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var chars = new char[4];
        for (var i = 0; i < chars.Length; i++)
            chars[i] = alphabet[_random.Next(alphabet.Length)];

        return "DN-" + pickupAt.ToString("yyyyMMdd") + "-" + new string(chars);
    }
}
