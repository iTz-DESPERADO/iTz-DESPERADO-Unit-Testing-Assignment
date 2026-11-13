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

        return CalculateFee(duration);
    }

    private static decimal CalculateFee(TimeSpan duration)
    {
        var fullDays = (int)(duration.TotalHours / 24);

        var remainingDuration = duration - TimeSpan.FromDays(fullDays);

        var remainingFee = Math.Min((decimal)Math.Ceiling(remainingDuration.TotalHours) * 10m, 80m);

        return (fullDays * 80m) + remainingFee;
    }
}
