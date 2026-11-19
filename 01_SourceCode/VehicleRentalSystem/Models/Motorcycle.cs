namespace VehicleRentalSystem.Models;

/// <summary>
/// Motosiklet: Vehicle'dan kalıtım alır. Günlük ücret üzerinden %15 indirimli hesaplanır.
/// </summary>
public class Motorcycle : Vehicle
{
    private int _engineCc;

    public Motorcycle(int id, string plate, string brand, string model, int year, decimal dailyPrice, int engineCc = 250)
        : base(id, plate, brand, model, year, dailyPrice)
    {
        EngineCc = engineCc;
    }

    public int EngineCc
    {
        get => _engineCc;
        set
        {
            if (value < 50)
                throw new ArgumentException("Motor hacmi en az 50 cc olmalıdır.");
            _engineCc = value;
        }
    }

    public override string VehicleType => "Motosiklet";
    public override string ExtraLabel => "Motor Hacmi (cc)";
    public override int ExtraValue { get => EngineCc; set => EngineCc = value; }

    public override decimal CalculateRentalPrice(int days)
    {
        EnsureDays(days);
        return DailyPrice * days * 0.85m;
    }
}
