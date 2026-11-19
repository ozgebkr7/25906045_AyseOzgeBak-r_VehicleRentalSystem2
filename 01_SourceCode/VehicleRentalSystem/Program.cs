using System.Globalization;
using VehicleRentalSystem.Forms;

namespace VehicleRentalSystem;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        // Para birimi ve tarih biçimleri Türkçe gösterilsin
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("tr-TR");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("tr-TR");
        CultureInfo.CurrentCulture = new CultureInfo("tr-TR");

        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
