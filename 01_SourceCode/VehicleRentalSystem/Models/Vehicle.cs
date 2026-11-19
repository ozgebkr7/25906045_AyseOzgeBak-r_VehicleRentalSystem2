namespace VehicleRentalSystem.Models;

/// <summary>
/// Tüm araçların ortak alanlarını tanımlayan soyut temel sınıf (abstract class).
/// Alanlar private tutulur, erişim doğrulamalı property'lerle yapılır (kapsülleme).
/// </summary>
public abstract class Vehicle : IRentable
{
    private string _plate = "";
    private string _brand = "";
    private string _model = "";
    private int _year;
    private decimal _dailyPrice;

    protected Vehicle(int id, string plate, string brand, string model, int year, decimal dailyPrice)
    {
        Id = id;
        Plate = plate;
        Brand = brand;
        Model = model;
        Year = year;
        DailyPrice = dailyPrice;
        Status = VehicleStatus.Musait;
    }

    public int Id { get; }

    public string Plate
    {
        get => _plate;
        set
        {
            // Boşlukları temizle, büyük harfe çevir: "34 abc 123" -> "34ABC123"
            string normalized = (value ?? "").Replace(" ", "").ToUpperInvariant();
            if (normalized.Length < 5 || normalized.Length > 10)
                throw new ArgumentException("Plaka 5-10 karakter arasında olmalıdır.");
            _plate = normalized;
        }
    }

    public string Brand
    {
        get => _brand;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Marka boş bırakılamaz.");
            _brand = value.Trim();
        }
    }

    public string Model
    {
        get => _model;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Model boş bırakılamaz.");
            _model = value.Trim();
        }
    }

    public int Year
    {
        get => _year;
        set
        {
            if (value < 1980 || value > DateTime.Now.Year + 1)
                throw new ArgumentException($"Üretim yılı 1980 ile {DateTime.Now.Year + 1} arasında olmalıdır.");
            _year = value;
        }
    }

    public decimal DailyPrice
    {
        get => _dailyPrice;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Günlük kira ücreti sıfırdan büyük olmalıdır.");
            _dailyPrice = value;
        }
    }

    public VehicleStatus Status { get; private set; }

    public bool IsAvailable => Status == VehicleStatus.Musait;

    /// <summary>Araç türünün görünen adı (alt sınıf belirler).</summary>
    public abstract string VehicleType { get; }

    /// <summary>Türe özgü sayısal bilgi için etiket (kapı sayısı, yük kapasitesi vb.).</summary>
    public abstract string ExtraLabel { get; }

    /// <summary>Türe özgü sayısal bilginin değeri.</summary>
    public abstract int ExtraValue { get; set; }

    /// <summary>
    /// Çok biçimlilik: her araç türü kira ücretini kendi kuralına göre hesaplar.
    /// </summary>
    public abstract decimal CalculateRentalPrice(int days);

    public void MarkRented()
    {
        if (Status == VehicleStatus.Kirada)
            throw new InvalidOperationException("Araç zaten kirada.");
        Status = VehicleStatus.Kirada;
    }

    public void MarkReturned() => Status = VehicleStatus.Musait;

    protected static void EnsureDays(int days)
    {
        if (days <= 0)
            throw new ArgumentException("Kiralama süresi en az 1 gün olmalıdır.");
    }

    public override string ToString() =>
        $"{Plate} - {Brand} {Model} ({VehicleType}) - {AppCulture.Money(DailyPrice)}/gün";
}
