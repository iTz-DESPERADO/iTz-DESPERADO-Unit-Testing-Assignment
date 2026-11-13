using Bogus;
using DriveNow.Domain.Entities;
using DriveNow.Domain.Enums;

namespace DriveNow.UnitTests.Task5;

public sealed class CustomerFaker : Faker<Customer>
{
    public CustomerFaker(int seed = 12345)
    {
        UseSeed(seed);
        RuleFor(x => x.Id, f => f.Random.Int(1, 10000));
        RuleFor(x => x.FullName, f => f.Name.FullName());
        RuleFor(x => x.Email, f => f.Internet.Email());
        RuleFor(x => x.DateOfBirth, _ => new DateTime(1990, 1, 1));
        RuleFor(x => x.LicenseIssueDate, _ => new DateTime(2010, 1, 1));
        RuleFor(x => x.LoyaltyTier, _ => LoyaltyTier.None);
        RuleFor(x => x.IsBlacklisted, _ => false);
    }
    public CustomerFaker WithAge(int age, DateTime? pickup = null)
    {
        var date = (pickup ?? new DateTime(2026, 11, 12)).Date;
        RuleFor(x => x.DateOfBirth, _ => date.AddYears(-age));
        return this;
    }
    public CustomerFaker WithTier(LoyaltyTier tier)
    {
        RuleFor(x => x.LoyaltyTier, _ => tier);
        return this;
    }
}

