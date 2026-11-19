using System.Globalization;

namespace VehicleRentalSystem.Models;

/// <summary>Para ve tarih biçimlerini bilgisayarın bölge ayarından bağımsız olarak Türkçe gösterir.</summary>
public static class AppCulture
{
    public static readonly CultureInfo Tr = new("tr-TR");

    public static string Money(decimal value) => value.ToString("C2", Tr);

    public static string Date(DateTime value) => value.ToString("dd.MM.yyyy", Tr);
}
