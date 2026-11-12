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
        var totalHours = Math.Ceiling(duration.TotalHours);

        var totalFee = (decimal)totalHours * 10m;

        return Math.Min(totalFee, 80m);
    }
}
