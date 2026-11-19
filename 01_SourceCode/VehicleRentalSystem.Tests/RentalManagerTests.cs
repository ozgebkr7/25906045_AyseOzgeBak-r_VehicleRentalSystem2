using VehicleRentalSystem.Models;
using VehicleRentalSystem.Services;
using Xunit;

namespace VehicleRentalSystem.Tests;

/// <summary>Raporda tanımlanan test senaryolarının otomatik karşılıkları.</summary>
public class RentalManagerTests
{
    private static (RentalManager m, Customer c, Vehicle v) Setup()
    {
        var m = new RentalManager();
        var v = m.AddVehicle(VehicleKind.Car, "34 ABC 123", "Toyota", "Corolla", 2022, 1000m, 4);
        var c = m.AddCustomer("Ali", "Veli", "5321234567", "A1");
        return (m, c, v);
    }

    [Fact]
    public void Rent_AvailableVehicle_CreatesRentalAndMarksRented()
    {
        var (m, c, v) = Setup();
        var r = m.CreateRental(c, v, new DateTime(2026, 11, 1), new DateTime(2026, 11, 4));
        Assert.Equal(3, r.TotalDays);
        Assert.Equal(3000m, r.TotalPrice);
        Assert.Equal(VehicleStatus.Kirada, v.Status);
        Assert.Equal(1, m.ActiveRentals);
    }

    [Fact]
    public void Rent_AlreadyRentedVehicle_Throws()
    {
        var (m, c, v) = Setup();
        m.CreateRental(c, v, DateTime.Today, DateTime.Today.AddDays(1));
        Assert.Throws<RentalException>(() => m.CreateRental(c, v, DateTime.Today, DateTime.Today.AddDays(1)));
    }

    [Fact]
    public void Rent_EndBeforeStart_Throws()
    {
        var (m, c, v) = Setup();
        Assert.Throws<RentalException>(() => m.CreateRental(c, v, DateTime.Today.AddDays(2), DateTime.Today));
    }

    [Fact]
    public void Rent_WithoutCustomer_Throws()
    {
        var (m, _, v) = Setup();
        Assert.Throws<RentalException>(() => m.CreateRental(null, v, DateTime.Today, DateTime.Today.AddDays(1)));
    }

    [Fact]
    public void Return_CompletesRentalAndFreesVehicle()
    {
        var (m, c, v) = Setup();
        var r = m.CreateRental(c, v, DateTime.Today, DateTime.Today.AddDays(2));
        m.ReturnRental(r.Id);
        Assert.Equal(RentalStatus.Tamamlandi, r.Status);
        Assert.True(v.IsAvailable);
        Assert.Equal(0, m.ActiveRentals);
        Assert.Throws<RentalException>(() => m.ReturnRental(r.Id));
    }

    [Fact]
    public void AddVehicle_DuplicatePlate_Throws()
    {
        var (m, _, _) = Setup();
        Assert.Throws<RentalException>(() => m.AddVehicle(VehicleKind.Car, "34abc123", "A", "B", 2020, 500m, 4));
    }

    [Fact]
    public void AddVehicle_ZeroPrice_Throws()
    {
        var m = new RentalManager();
        Assert.Throws<ArgumentException>(() => m.AddVehicle(VehicleKind.Car, "06 XYZ 111", "A", "B", 2020, 0m, 4));
    }

    [Fact]
    public void DeleteCustomer_WithActiveRental_Throws()
    {
        var (m, c, v) = Setup();
        m.CreateRental(c, v, DateTime.Today, DateTime.Today.AddDays(1));
        Assert.Throws<RentalException>(() => m.DeleteCustomer(c.Id));
    }

    [Fact]
    public void DeleteVehicle_WithRentalRecord_Throws()
    {
        var (m, c, v) = Setup();
        var r = m.CreateRental(c, v, DateTime.Today, DateTime.Today.AddDays(1));
        m.ReturnRental(r.Id);
        Assert.Throws<RentalException>(() => m.DeleteVehicle(v.Id));
    }

    [Fact]
    public void AddCustomer_InvalidPhone_Throws()
    {
        var m = new RentalManager();
        Assert.Throws<ArgumentException>(() => m.AddCustomer("A", "B", "123", "L1"));
    }

    [Fact]
    public void UpdateVehicle_InvalidValue_RollsBack()
    {
        var (m, _, v) = Setup();
        Assert.Throws<ArgumentException>(() => m.UpdateVehicle(v.Id, "35 NEW 999", "Honda", "Civic", 2020, -5m, 4));
        Assert.Equal("34ABC123", v.Plate);
        Assert.Equal("Toyota", v.Brand);
    }

    [Fact]
    public void Polymorphism_PriceDiffersByVehicleType()
    {
        var car = new Car(1, "34AAA111", "A", "B", 2022, 1000m);
        var truck = new Truck(2, "34BBB222", "A", "B", 2022, 1000m);
        var moto = new Motorcycle(3, "34CCC333", "A", "B", 2022, 1000m);
        Assert.Equal(5000m, car.CalculateRentalPrice(5));
        Assert.Equal(6000m, truck.CalculateRentalPrice(5));
        Assert.Equal(4250m, moto.CalculateRentalPrice(5));
        Assert.Equal(6300m, car.CalculateRentalPrice(7));   // %10 uzun dönem indirimi
    }

    [Fact]
    public void Dashboard_CountsComeFromCollections()
    {
        var m = new RentalManager();
        m.LoadSampleData();
        Assert.Equal(5, m.TotalVehicles);
        Assert.Equal(4, m.AvailableVehicles);
        Assert.Equal(1, m.RentedVehicles);
        Assert.Equal(3, m.TotalCustomers);
        Assert.Equal(1, m.ActiveRentals);
    }
}
