namespace VehicleRentalSystem.Models;

/// <summary>Müşteri bilgilerini tutar; girişler property'lerde doğrulanır.</summary>
public class Customer
{
    private string _firstName = "";
    private string _lastName = "";
    private string _phone = "";
    private string _licenseNumber = "";

    public Customer(int id, string firstName, string lastName, string phone, string licenseNumber)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        LicenseNumber = licenseNumber;
    }

    public int Id { get; }

    public string FirstName
    {
        get => _firstName;
        set => _firstName = RequireText(value, "Ad");
    }

    public string LastName
    {
        get => _lastName;
        set => _lastName = RequireText(value, "Soyad");
    }

    public string Phone
    {
        get => _phone;
        set
        {
            string digits = new string((value ?? "").Where(char.IsDigit).ToArray());
            if (digits.Length != 10 && digits.Length != 11)
                throw new ArgumentException("Telefon 10 veya 11 haneli rakamlardan oluşmalıdır.");
            _phone = digits;
        }
    }

    public string LicenseNumber
    {
        get => _licenseNumber;
        set => _licenseNumber = RequireText(value, "Ehliyet numarası");
    }

    public string FullName => $"{FirstName} {LastName}";

    private static string RequireText(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{fieldName} boş bırakılamaz.");
        return value.Trim();
    }

    public override string ToString() => $"{FullName} ({LicenseNumber})";
}
