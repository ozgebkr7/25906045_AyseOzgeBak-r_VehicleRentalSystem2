namespace VehicleRentalSystem.Models;

/// <summary>
/// Otomobil: Vehicle'dan kalıtım alır. 7 gün ve üzeri kiralamalarda %10 indirim uygular.
/// </summary>
public class Car : Vehicle
{
    private int _doorCount;

    public Car(int id, string plate, string brand, string model, int year, decimal dailyPrice, int doorCount = 4)
        : base(id, plate, brand, model, year, dailyPrice)
    {
        DoorCount = doorCount;
    }

    public int DoorCount
    {
        get => _doorCount;
        set
        {
            if (value < 2 || value > 6)
                throw new ArgumentException("Kapı sayısı 2 ile 6 arasında olmalıdır.");
            _doorCount = value;
        }
    }

    public override string VehicleType => "Otomobil";
    public override string ExtraLabel => "Kapı Sayısı";
    public override int ExtraValue { get => DoorCount; set => DoorCount = value; }

    public override decimal CalculateRentalPrice(int days)
    {
        EnsureDays(days);
        decimal total = DailyPrice * days;
        return days >= 7 ? total * 0.90m : total;   // uzun dönem indirimi
    }
}
