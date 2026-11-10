namespace DriveNow.Application.Interfaces;

public interface IReceiptFormatter
{
    string Render(string reference, IReadOnlyList<string> lines);
}
