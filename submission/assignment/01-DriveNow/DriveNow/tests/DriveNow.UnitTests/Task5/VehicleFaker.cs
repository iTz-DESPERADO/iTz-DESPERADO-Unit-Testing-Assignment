using Bogus;
using DriveNow.Domain.Entities;
using DriveNow.Domain.Enums;

namespace DriveNow.UnitTests.Task5;

public sealed class VehicleFaker : Faker<Vehicle>
{
    public VehicleFaker(int seed = 12345)
    {
        UseSeed(seed);
        RuleFor(x => x.Id, f => f.Random.Int(1, 10000));
        RuleFor(x => x.Model, f => f.Vehicle.Model());
        RuleFor(x => x.Category, _ => VehicleCategory.Economy);
        RuleFor(x => x.DailyRate, _ => 600m);
        RuleFor(x => x.IsAvailable, _ => true);
    }
    public VehicleFaker WithDailyRate(decimal dailyRate)
    {
        RuleFor(x => x.DailyRate, _ => dailyRate);
        return this;
    }
    public VehicleFaker WithCategory(VehicleCategory category)
    {
        RuleFor(x => x.Category, _ => category);
        return this;
    }
}
