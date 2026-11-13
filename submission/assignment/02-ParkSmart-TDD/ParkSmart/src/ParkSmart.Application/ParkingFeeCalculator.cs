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

        var normalFee = CalculateFee(duration, session.Vehicle);

        if (session.TicketLost)
            return Math.Max(normalFee, LostTicketFee(session.Vehicle));

        if (IsSubscriberFree(session))
            return 0m;

        var freePeriod = session.EntryTime.DayOfWeek == DayOfWeek.Friday
            ? TimeSpan.FromHours(2)
            : TimeSpan.FromMinutes(15);

        if (duration <= freePeriod)
            return 0m;

        var nightStart = session.EntryTime.Date.AddHours(22);

        var nightEnd = nightStart.AddHours(10);

        var isEligibleNight = session.EntryTime >= nightStart && session.ExitTime <= nightEnd;

        if (!isEligibleNight)
            return normalFee;


        return Math.Min(normalFee, NightFlatRate(session.Vehicle));
    }

    private static bool IsSubscriberFree(ParkingSession session)
    {
        var isWeekday = session.EntryTime.DayOfWeek is
            DayOfWeek.Sunday or
            DayOfWeek.Monday or
            DayOfWeek.Tuesday or
            DayOfWeek.Wednesday or
            DayOfWeek.Thursday;

        return session.IsSubscriber &&
            session.EntryTime.Date == session.ExitTime.Date &&
            isWeekday &&
            session.EntryTime.TimeOfDay >= TimeSpan.FromHours(7) &&
            session.ExitTime.TimeOfDay <= TimeSpan.FromHours(19);
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

    private static decimal LostTicketFee(VehicleType vehicleType) =>
        vehicleType switch
        {
            VehicleType.Car => 150m,
            VehicleType.Motorcycle => 75m,
            VehicleType.Truck => 300m,
            _ => throw new ArgumentOutOfRangeException(nameof(vehicleType))
        };

}
