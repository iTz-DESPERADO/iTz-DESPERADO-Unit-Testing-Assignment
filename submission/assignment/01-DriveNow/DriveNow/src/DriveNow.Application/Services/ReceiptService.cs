using DriveNow.Application.Receipts;
using DriveNow.Domain.Entities;
using DriveNow.Domain.ValueObjects;

namespace DriveNow.Application.Services;

public class ReceiptService
{
    readonly LineItemFactory _lineItems;
    readonly TaxCalculator _tax;
    readonly CurrencyFormatter _currency;
    readonly ReceiptFormatter _receipts;

    public ReceiptService(
        LineItemFactory lineItems,
        TaxCalculator tax,
        CurrencyFormatter currency,
        ReceiptFormatter receipts)
    {
        _lineItems = lineItems;
        _tax = tax;
        _currency = currency;
        _receipts = receipts;
    }

    public string Build(Booking booking, ReturnReport? report)
    {
        var items = _lineItems.Create(booking, report);
        decimal vat = 0m;
        decimal net = 0m;
        foreach (var item in items)
        {
            vat += _tax.CalculateVat(item.Amount);
            net += item.Amount;
        }

        var total = net + vat;
        var formattedItems = items
            .Select(item => (Item: item, Amount: _currency.Format(item.Amount)))
            .ToList();
        var formattedVat = _currency.Format(vat);
        var formattedTotal = _currency.Format(total);
        var width = formattedItems.Count == 0
            ? 0
            : formattedItems.Max(item => item.Amount.Length);

        var lines = new List<string>();
        foreach (var item in formattedItems)
            lines.Add(item.Item.Description.PadRight(30, '.') + item.Amount.PadLeft(width));

        lines.Add("VAT 14%".PadRight(30, '.') + formattedVat.PadLeft(width));
        lines.Add("Total".PadRight(30, '.') + formattedTotal.PadLeft(width));
        return _receipts.Render(booking.Reference, lines);
    }
}
