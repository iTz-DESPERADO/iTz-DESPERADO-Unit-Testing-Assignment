using System.Collections;
using System.Linq;
using DriveNow.Domain.Entities;
using DriveNow.Domain.Enums;

namespace DriveNow.UnitTests.Task5;

public sealed class PromoValidationCases : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        // case, promo in repository (null means missing), rental days, vehicle category, expected message (null => valid)
        yield return Case("missing", null, 1, VehicleCategory.Economy, "Promo code 'SAVE15' does not exist");
        yield return Case("valid inclusive first", Promo(new DateTime(2026, 11, 12), new DateTime(2026, 11, 15)), 1, VehicleCategory.Economy, null);
        // AI suggestion: both inclusive date boundaries and the date just before validity.
        yield return Case("valid inclusive last", Promo(new DateTime(2026, 11, 10), new DateTime(2026, 11, 12)), 1, VehicleCategory.Economy, null);
        yield return Case("not yet active", Promo(new DateTime(2026, 11, 13), new DateTime(2026, 11, 30)), 1, VehicleCategory.Economy, "Promo code 'SAVE15' has expired");
        yield return Case("expired", Promo(new DateTime(2026, 10, 1), new DateTime(2026, 11, 11)), 1, VehicleCategory.Economy, "Promo code 'SAVE15' has expired");
        yield return Case("minimum days", Promo(minDays: 2), 1, VehicleCategory.Economy, "Promo code 'SAVE15' requires at least 2 days");
        // AI suggestion: exact minimum days and both category outcomes.
        yield return Case("exact minimum", Promo(minDays: 2), 2, VehicleCategory.Economy, null);
        yield return Case("wrong category", Promo(allowed: [VehicleCategory.Luxury]), 2, VehicleCategory.Economy, "Promo code 'SAVE15' is not valid for Economy");
        yield return Case("valid category", Promo(allowed: [VehicleCategory.Luxury]), 2, VehicleCategory.Luxury, null);
        // AI suggestion: null categories allow every vehicle category, and usage has a boundary.
        yield return Case("null categories", Promo(), 1, VehicleCategory.Luxury, null);
        yield return Case("max uses reached", Promo(maxUses: 2, usedCount: 2), 1, VehicleCategory.Economy, "Promo code 'SAVE15' has been fully used");
        yield return Case("one use remains", Promo(maxUses: 2, usedCount: 1), 1, VehicleCategory.Economy, null);
        // AI suggestion: confirm the documented first-failure order.
        yield return Case("date takes priority over days", Promo(new DateTime(2026, 11, 13), new DateTime(2026, 11, 30), minDays: 7), 1, VehicleCategory.Economy, "Promo code 'SAVE15' has expired");
        yield return Case("days take priority over category", Promo(minDays: 7, allowed: [VehicleCategory.Luxury]), 1, VehicleCategory.Economy, "Promo code 'SAVE15' requires at least 7 days");
        yield return Case("category takes priority over uses", Promo(allowed: [VehicleCategory.Luxury], maxUses: 1, usedCount: 1), 1, VehicleCategory.Economy, "Promo code 'SAVE15' is not valid for Economy");
    }
    private static object[] Case(string name, PromoCode? promo, int days, VehicleCategory category, string? message)
        => [name, promo!, days, category, message!];
    private static PromoCode Promo(DateTime? from = null, DateTime? until = null, int minDays = 1,
        VehicleCategory[]? allowed = null, int maxUses = 100, int usedCount = 0) => new()
    {
        Code = "SAVE15", DiscountPercent = 15,
        ValidFrom = from ?? new DateTime(2026, 11, 1),
        ValidUntil = until ?? new DateTime(2026, 12, 1),
        MinDays = minDays, AllowedCategories = allowed?.ToList(), MaxUses = maxUses, UsedCount = usedCount
    };
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
