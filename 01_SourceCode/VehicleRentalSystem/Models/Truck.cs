namespace VehicleRentalSystem.Models;

/// <summary>
/// Ticari araç: Vehicle'dan kalıtım alır. Yük taşıma nedeniyle %20 ek ücret uygular.
/// </summary>
public class Truck : Vehicle
{
    private int _loadCapacityKg;

    public Truck(int id, string plate, string brand, string model, int year, decimal dailyPrice, int loadCapacityKg = 1000)
        : base(id, plate, brand, model, year, dailyPrice)
    {
        LoadCapacityKg = loadCapacityKg;
    }

    public int LoadCapacityKg
    {
        get => _loadCapacityKg;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Yük kapasitesi sıfırdan büyük olmalıdır.");
            _loadCapacityKg = value;
        }
    }

    public override string VehicleType => "Ticari Araç";
    public override string ExtraLabel => "Yük Kapasitesi (kg)";
    public override int ExtraValue { get => LoadCapacityKg; set => LoadCapacityKg = value; }

    public override decimal CalculateRentalPrice(int days)
    {
        EnsureDays(days);
        return DailyPrice * days * 1.20m;   // ticari kullanım ek ücreti
    }
}
