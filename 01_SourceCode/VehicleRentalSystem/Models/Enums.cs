namespace VehicleRentalSystem.Models;

/// <summary>Aracın kiralama durumu (Müsait / Kirada).</summary>
public enum VehicleStatus
{
    Musait,
    Kirada
}

/// <summary>Kiralama kaydının durumu.</summary>
public enum RentalStatus
{
    Aktif,
    Tamamlandi
}

/// <summary>Oluşturulabilecek araç türleri (Vehicle'dan türeyen sınıflar).</summary>
public enum VehicleKind
{
    Car,
    Truck,
    Motorcycle
}
