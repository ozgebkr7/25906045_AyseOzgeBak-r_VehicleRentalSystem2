namespace VehicleRentalSystem.Models;

/// <summary>Bir müşterinin bir aracı hangi tarihler arasında kiraladığını temsil eder.</summary>
public class Rental
{
    public Rental(int id, Customer customer, Vehicle vehicle, DateTime startDate, DateTime endDate)
    {
        if (endDate.Date < startDate.Date)
            throw new ArgumentException("Bitiş tarihi başlangıç tarihinden önce olamaz.");

        Id = id;
        Customer = customer;
        Vehicle = vehicle;
        StartDate = startDate.Date;
        EndDate = endDate.Date;
        // Aynı gün alıp bırakma da en az 1 gün olarak ücretlendirilir
        TotalDays = Math.Max(1, (EndDate - StartDate).Days);
        // Ücret, araç türüne göre polimorfik olarak hesaplanır
        TotalPrice = vehicle.CalculateRentalPrice(TotalDays);
        Status = RentalStatus.Aktif;
    }

    public int Id { get; }
    public Customer Customer { get; }
    public Vehicle Vehicle { get; }
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }
    public int TotalDays { get; }
    public decimal TotalPrice { get; }
    public RentalStatus Status { get; private set; }
    public DateTime? ReturnDate { get; private set; }

    public bool IsActive => Status == RentalStatus.Aktif;

    public void Complete(DateTime returnDate)
    {
        if (!IsActive)
            throw new InvalidOperationException("Bu kiralama zaten tamamlanmış.");
        Status = RentalStatus.Tamamlandi;
        ReturnDate = returnDate;
    }
}
