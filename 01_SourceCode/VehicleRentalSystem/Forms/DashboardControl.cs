using VehicleRentalSystem.Services;

namespace VehicleRentalSystem.Forms;

/// <summary>Ana sayfa: özet sayıları koleksiyonlardan hesaplayarak gösterir.</summary>
public class DashboardControl : UserControl
{
    private readonly RentalManager _manager;
    private readonly Label _lblTotalVehicles = new();
    private readonly Label _lblAvailable = new();
    private readonly Label _lblRented = new();
    private readonly Label _lblCustomers = new();
    private readonly Label _lblActive = new();

    public DashboardControl(RentalManager manager)
    {
        _manager = manager;
        Dock = DockStyle.Fill;
        BackColor = UiHelper.Surface;
        Padding = new Padding(20);

        var cards = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(0, 10, 0, 0) };
        cards.Controls.Add(CreateCard("Toplam Araç", _lblTotalVehicles, UiHelper.Primary));
        cards.Controls.Add(CreateCard("Müsait Araç", _lblAvailable, UiHelper.Success));
        cards.Controls.Add(CreateCard("Kiradaki Araç", _lblRented, UiHelper.Danger));
        cards.Controls.Add(CreateCard("Toplam Müşteri", _lblCustomers, UiHelper.Neutral));
        cards.Controls.Add(CreateCard("Aktif Kiralama", _lblActive, Color.FromArgb(230, 126, 34)));

        Controls.Add(cards);
        Controls.Add(UiHelper.CreateTitle("Ana Sayfa"));

        _manager.DataChanged += (_, _) => RefreshStats();
        RefreshStats();
    }

    private static Panel CreateCard(string title, Label valueLabel, Color color)
    {
        var panel = new Panel { Width = 190, Height = 110, BackColor = color, Margin = new Padding(0, 0, 16, 16) };
        var lblTitle = new Label
        {
            Text = title, ForeColor = Color.White, Dock = DockStyle.Top, Height = 34,
            Font = new Font("Segoe UI", 10.5f), Padding = new Padding(12, 10, 0, 0)
        };
        valueLabel.ForeColor = Color.White;
        valueLabel.Dock = DockStyle.Fill;
        valueLabel.Font = new Font("Segoe UI", 28f, FontStyle.Bold);
        valueLabel.Padding = new Padding(12, 0, 0, 0);
        panel.Controls.Add(valueLabel);
        panel.Controls.Add(lblTitle);
        return panel;
    }

    /// <summary>Değerler sabit metin değil; her seferinde koleksiyonlardan hesaplanır.</summary>
    public void RefreshStats()
    {
        _lblTotalVehicles.Text = _manager.TotalVehicles.ToString();
        _lblAvailable.Text = _manager.AvailableVehicles.ToString();
        _lblRented.Text = _manager.RentedVehicles.ToString();
        _lblCustomers.Text = _manager.TotalCustomers.ToString();
        _lblActive.Text = _manager.ActiveRentals.ToString();
    }
}
