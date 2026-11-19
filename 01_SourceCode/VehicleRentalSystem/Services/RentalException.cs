namespace VehicleRentalSystem.Services;

/// <summary>İş kurallarının ihlalinde fırlatılan özel istisna (exception).</summary>
public class RentalException : Exception
{
    public RentalException(string message) : base(message) { }
}
