using ParkSmart.Domain;

namespace ParkSmart.Application;

public class ParkingFeeCalculator
{
    public decimal Calculate(ParkingSession session)
    {
        if (session.ExitTime < session.EntryTime)
            throw new ArgumentException("Exit time must be after entry time.");

        var duration = session.ExitTime - session.EntryTime;

        if (duration <= TimeSpan.FromMinutes(15))
            return 0m;

        return CalculateFee(duration, session.Vehicle);
    }

    private static decimal CalculateFee(TimeSpan duration, VehicleType vehicleType)
    {
        var hourlyRate = vehicleType == VehicleType.Car ? 10m : 5m;

        var dailyCap = vehicleType == VehicleType.Car ? 80 : 40;

        var fullDays = (int)(duration.TotalHours / 24);

        var remainingDuration = duration - TimeSpan.FromDays(fullDays);

        var remainingFee = Math.Min((decimal)Math.Ceiling(remainingDuration.TotalHours) * hourlyRate, dailyCap);

        return (fullDays * dailyCap) + remainingFee;
    }
}
