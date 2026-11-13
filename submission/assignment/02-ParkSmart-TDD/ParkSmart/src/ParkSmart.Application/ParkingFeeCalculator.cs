using ParkSmart.Domain;

namespace ParkSmart.Application;

public class ParkingFeeCalculator
{
    public decimal Calculate(ParkingSession session)
    {
        if (session.ExitTime < session.EntryTime)
            throw new ArgumentException(
                "Exit time must be after entry time.");

        var duration = session.ExitTime - session.EntryTime;

        if (duration <= TimeSpan.FromMinutes(15))
            return 0m;

        var normalFee = CalculateFee(duration, session.Vehicle);

        var nightStart = session.EntryTime.Date.AddHours(22);

        var nightEnd = nightStart.AddHours(10);

        var isEligibleNight = session.EntryTime >= nightStart && session.ExitTime <= nightEnd;

        if (!isEligibleNight)
            return normalFee;


        return Math.Min(normalFee, NightFlatRate(session.Vehicle));
    }

    private static decimal CalculateFee(TimeSpan duration, VehicleType vehicleType)
    {
        var hourlyRate = HourlyRate(vehicleType);

        var dailyCap = DailyCap(vehicleType);

        var fullDays = (int)(duration.TotalHours / 24);

        var remainingDuration = duration - TimeSpan.FromDays(fullDays);

        var remainingFee = Math.Min((decimal)Math.Ceiling(remainingDuration.TotalHours) * hourlyRate, dailyCap);

        return (fullDays * dailyCap) + remainingFee;
    }

    private static decimal HourlyRate(VehicleType vehicleType) =>
        vehicleType switch
        {
            VehicleType.Car => 10m,
            VehicleType.Motorcycle => 5m,
            VehicleType.Truck => 25m,
            _ => throw new ArgumentOutOfRangeException(nameof(vehicleType))
        };

    private static decimal DailyCap(VehicleType vehicleType) =>
        vehicleType switch
        {
            VehicleType.Car => 80m,
            VehicleType.Motorcycle => 40m,
            VehicleType.Truck => 200m,
            _ => throw new ArgumentOutOfRangeException(nameof(vehicleType))
        };

    private static decimal NightFlatRate(VehicleType vehicleType) =>
        vehicleType switch
        {
            VehicleType.Car => 30m,
            VehicleType.Motorcycle => 15m,
            VehicleType.Truck => 75m,
            _ => throw new ArgumentOutOfRangeException(
                nameof(vehicleType))
        };
}
