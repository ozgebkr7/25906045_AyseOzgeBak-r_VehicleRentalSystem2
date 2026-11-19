using VehicleRentalSystem.Models;

namespace VehicleRentalSystem.Services;

/// <summary>
/// Araç, müşteri ve kiralama koleksiyonlarını yöneten iş mantığı sınıfı.
/// Formlar yalnızca bu sınıfı çağırır; kurallar burada toplanmıştır.
/// Veriler List&lt;T&gt; ile bellekte tutulur.
/// </summary>
public class RentalManager
{
    private readonly List<Vehicle> _vehicles = new();
    private readonly List<Customer> _customers = new();
    private readonly List<Rental> _rentals = new();
    private int _nextVehicleId = 1;
    private int _nextCustomerId = 1;
    private int _nextRentalId = 1;

    /// <summary>Veri değiştiğinde formların kendini yenilemesi için olay (event).</summary>
    public event EventHandler? DataChanged;

    public IReadOnlyList<Vehicle> Vehicles => _vehicles;
    public IReadOnlyList<Customer> Customers => _customers;
    public IReadOnlyList<Rental> Rentals => _rentals;

    // ---------- Dashboard için hesaplanan değerler ----------
    public int TotalVehicles => _vehicles.Count;
    public int AvailableVehicles => _vehicles.Count(v => v.IsAvailable);
    public int RentedVehicles => _vehicles.Count(v => !v.IsAvailable);
    public int TotalCustomers => _customers.Count;
    public int ActiveRentals => _rentals.Count(r => r.IsActive);

    private void OnDataChanged() => DataChanged?.Invoke(this, EventArgs.Empty);

    // ---------- Araç işlemleri ----------
    public Vehicle AddVehicle(VehicleKind kind, string plate, string brand, string model,
                              int year, decimal dailyPrice, int extra)
    {
        EnsurePlateUnique(plate, null);

        Vehicle vehicle = kind switch
        {
            VehicleKind.Car => new Car(_nextVehicleId, plate, brand, model, year, dailyPrice, extra),
            VehicleKind.Truck => new Truck(_nextVehicleId, plate, brand, model, year, dailyPrice, extra),
            VehicleKind.Motorcycle => new Motorcycle(_nextVehicleId, plate, brand, model, year, dailyPrice, extra),
            _ => throw new RentalException("Geçersiz araç türü.")
        };

        _nextVehicleId++;
        _vehicles.Add(vehicle);
        OnDataChanged();
        return vehicle;
    }

    public void UpdateVehicle(int id, string plate, string brand, string model,
                              int year, decimal dailyPrice, int extra)
    {
        Vehicle vehicle = FindVehicle(id);
        EnsurePlateUnique(plate, id);

        // Önce eski değerleri sakla; doğrulama hata verirse geri al (tutarlılık)
        var (oldPlate, oldBrand, oldModel, oldYear, oldPrice, oldExtra) =
            (vehicle.Plate, vehicle.Brand, vehicle.Model, vehicle.Year, vehicle.DailyPrice, vehicle.ExtraValue);
        try
        {
            vehicle.Plate = plate;
            vehicle.Brand = brand;
            vehicle.Model = model;
            vehicle.Year = year;
            vehicle.DailyPrice = dailyPrice;
            vehicle.ExtraValue = extra;
        }
        catch
        {
            vehicle.Plate = oldPlate;
            vehicle.Brand = oldBrand;
            vehicle.Model = oldModel;
            vehicle.Year = oldYear;
            vehicle.DailyPrice = oldPrice;
            vehicle.ExtraValue = oldExtra;
            throw;
        }
        OnDataChanged();
    }

    public void DeleteVehicle(int id)
    {
        Vehicle vehicle = FindVehicle(id);
        if (_rentals.Any(r => r.Vehicle.Id == id))
            throw new RentalException("Kiralama kaydı bulunan araç silinemez.");
        _vehicles.Remove(vehicle);
        OnDataChanged();
    }

    public Vehicle FindVehicle(int id) =>
        _vehicles.FirstOrDefault(v => v.Id == id) ?? throw new RentalException("Araç bulunamadı.");

    private void EnsurePlateUnique(string plate, int? ignoreId)
    {
        string normalized = (plate ?? "").Replace(" ", "").ToUpperInvariant();
        if (_vehicles.Any(v => v.Plate == normalized && v.Id != ignoreId))
            throw new RentalException("Bu plaka ile kayıtlı başka bir araç var.");
    }

    // ---------- Müşteri işlemleri ----------
    public Customer AddCustomer(string firstName, string lastName, string phone, string licenseNumber)
    {
        var customer = new Customer(_nextCustomerId, firstName, lastName, phone, licenseNumber);
        _nextCustomerId++;
        _customers.Add(customer);
        OnDataChanged();
        return customer;
    }

    public void UpdateCustomer(int id, string firstName, string lastName, string phone, string licenseNumber)
    {
        Customer c = FindCustomer(id);
        var (f, l, p, n) = (c.FirstName, c.LastName, c.Phone, c.LicenseNumber);
        try
        {
            c.FirstName = firstName;
            c.LastName = lastName;
            c.Phone = phone;
            c.LicenseNumber = licenseNumber;
        }
        catch
        {
            c.FirstName = f; c.LastName = l; c.Phone = p; c.LicenseNumber = n;
            throw;
        }
        OnDataChanged();
    }

    public void DeleteCustomer(int id)
    {
        Customer c = FindCustomer(id);
        if (_rentals.Any(r => r.Customer.Id == id && r.IsActive))
            throw new RentalException("Aktif kiralaması bulunan müşteri silinemez.");
        _customers.Remove(c);
        OnDataChanged();
    }

    public Customer FindCustomer(int id) =>
        _customers.FirstOrDefault(c => c.Id == id) ?? throw new RentalException("Müşteri bulunamadı.");

    // ---------- Kiralama / iade ----------
    public Rental CreateRental(Customer? customer, Vehicle? vehicle, DateTime start, DateTime end)
    {
        if (customer == null) throw new RentalException("Lütfen bir müşteri seçiniz.");
        if (vehicle == null) throw new RentalException("Lütfen bir araç seçiniz.");
        if (!vehicle.IsAvailable) throw new RentalException("Seçilen araç müsait değil.");
        if (end.Date < start.Date) throw new RentalException("Bitiş tarihi başlangıç tarihinden önce olamaz.");

        var rental = new Rental(_nextRentalId, customer, vehicle, start, end);
        _nextRentalId++;
        vehicle.MarkRented();
        _rentals.Add(rental);
        OnDataChanged();
        return rental;
    }

    public void ReturnRental(int rentalId)
    {
        Rental rental = _rentals.FirstOrDefault(r => r.Id == rentalId)
                        ?? throw new RentalException("Kiralama kaydı bulunamadı.");
        if (!rental.IsActive)
            throw new RentalException("Bu kiralama zaten tamamlanmış.");

        rental.Complete(DateTime.Now);
        rental.Vehicle.MarkReturned();
        OnDataChanged();
    }

    // ---------- Örnek veriler ----------
    public void LoadSampleData()
    {
        AddVehicle(VehicleKind.Car, "34 ABC 123", "Toyota", "Corolla", 2022, 1200m, 4);
        AddVehicle(VehicleKind.Car, "06 DEF 456", "Renault", "Clio", 2021, 900m, 4);
        AddVehicle(VehicleKind.Car, "35 GHI 789", "Volkswagen", "Passat", 2023, 1800m, 4);
        AddVehicle(VehicleKind.Truck, "16 JKL 321", "Ford", "Transit", 2020, 2000m, 1500);
        AddVehicle(VehicleKind.Motorcycle, "07 MNO 654", "Honda", "PCX", 2022, 600m, 150);

        Customer c1 = AddCustomer("Ayşe", "Yılmaz", "5321234567", "A123456");
        AddCustomer("Mehmet", "Demir", "5337654321", "B654321");
        AddCustomer("Zeynep", "Kaya", "5421112233", "C112233");

        // Başlangıçta bir aktif kiralama bulunsun
        CreateRental(c1, _vehicles[1], DateTime.Today, DateTime.Today.AddDays(3));
    }
}
