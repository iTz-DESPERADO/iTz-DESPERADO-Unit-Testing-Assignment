using DriveNow.Domain.Entities;
using DriveNow.Domain.ValueObjects;

namespace DriveNow.Application.Interfaces;

public interface ILineItemFactory
{
    List<LineItem> Create(Booking booking, ReturnReport? report);
}
