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

        var totalHours = Math.Ceiling(duration.TotalHours);

        var totalFee = (decimal)totalHours * 10m;

        return  totalHours > 8 ? 80 : totalFee;
    }
}
