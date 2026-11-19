namespace VehicleRentalSystem.Models;

/// <summary>
/// Kiralanabilir nesneler için ortak sözleşme (interface - soyutlama).
/// </summary>
public interface IRentable
{
    bool IsAvailable { get; }
    decimal CalculateRentalPrice(int days);
    void MarkRented();
    void MarkReturned();
}
