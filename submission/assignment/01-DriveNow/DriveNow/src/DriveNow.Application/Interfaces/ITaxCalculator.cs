namespace DriveNow.Application.Interfaces;

public interface ITaxCalculator
{
    decimal CalculateVat(decimal amount);
}
