namespace DriveNow.Domain.Interfaces;

public interface IBookingReferenceGenerator
{
    string Generate(DateTime pickupAt);
}
