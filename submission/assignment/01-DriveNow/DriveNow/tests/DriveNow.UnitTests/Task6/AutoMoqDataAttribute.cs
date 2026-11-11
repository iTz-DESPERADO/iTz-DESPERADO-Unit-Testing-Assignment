using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using DriveNow.Domain.Entities;
using DriveNow.Domain.Enums;

namespace DriveNow.UnitTests.Task6;

internal sealed class AutoMoqDataAttribute : AutoDataAttribute
{
    public AutoMoqDataAttribute() : base(CreateFixture) { }

    static IFixture CreateFixture()
    {
        var fixture = new Fixture();
        fixture.Customize(new AutoMoqCustomization());
        return fixture;
    }
}

internal sealed class InlineAutoMoqDataAttribute : CompositeDataAttribute
{
    public InlineAutoMoqDataAttribute(params object[] values)
        : base(new InlineDataAttribute(values), new AutoMoqDataAttribute()) { }
}

internal sealed class ValidAutoMoqDataAttribute : AutoDataAttribute
{
    public ValidAutoMoqDataAttribute() : base(CreateFixture) { }

    static IFixture CreateFixture()
    {
        var fixture = new Fixture();
        fixture.Customize(new AutoMoqCustomization());
        fixture.Customize(new ValidAdultCustomerCustomization());
        return fixture;
    }
}

internal sealed class ValidAdultCustomerCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<Customer>(composer => composer
            .With(customer => customer.DateOfBirth, new DateTime(1990, 1, 1))
            .With(customer => customer.LicenseIssueDate, new DateTime(2020, 1, 1))
            .With(customer => customer.IsBlacklisted, false)
            .With(customer => customer.LoyaltyTier, LoyaltyTier.None));
    }
}
