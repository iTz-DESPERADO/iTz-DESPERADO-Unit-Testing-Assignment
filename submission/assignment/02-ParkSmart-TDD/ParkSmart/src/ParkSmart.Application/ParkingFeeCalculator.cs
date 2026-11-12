using ParkSmart.Domain;

namespace ParkSmart.Application;

public class ParkingFeeCalculator
{
    public decimal Calculate(ParkingSession session)
    {
        var duration = session.ExitTime - session.EntryTime;

        if (duration <= TimeSpan.FromMinutes(15))
            return 0m;

        var totalHours = Math.Ceiling(duration.TotalHours);

        return (decimal)totalHours * 10m;
    }
}
