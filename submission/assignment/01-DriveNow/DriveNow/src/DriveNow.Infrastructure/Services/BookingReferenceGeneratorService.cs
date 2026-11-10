using DriveNow.Domain.Interfaces;

namespace DriveNow.Infrastructure.Services;

public class BookingReferenceGeneratorService : IBookingReferenceGenerator
{
    public string Generate(DateTime pickupAt)
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        var chars = new char[4];
        for (var i = 0; i < chars.Length; i++)
            chars[i] = alphabet[random.Next(alphabet.Length)];

        return "DN-" + pickupAt.ToString("yyyyMMdd") + "-" + new string(chars);
    }
}
