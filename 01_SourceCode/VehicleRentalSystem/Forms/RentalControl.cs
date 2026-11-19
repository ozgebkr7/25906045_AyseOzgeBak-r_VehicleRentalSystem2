using VehicleRentalSystem.Models;
using VehicleRentalSystem.Services;

namespace VehicleRentalSystem.Forms;

/// <summary>Araç kiralama: müşteri + müsait araç + tarih seçip kayıt oluşturur.</summary>
public class RentalControl : UserControl
{
    private readonly RentalManager _manager;
    private readonly ComboBox _cmbCustomer = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _cmbVehicle = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DateTimePicker _dtpStart = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "dd.MM.yyyy", Value = DateTime.Today };
    private readonly DateTimePicker _dtpEnd = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "dd.MM.yyyy", Value = DateTime.Today.AddDays(1) };
    private readonly Label _lblDays = new() { AutoSize = true, Font = new Font("Segoe UI", 10f, FontStyle.Bold) };
    private readonly Label _lblTotal = new() { AutoSize = true, Font = new Font("Segoe UI", 12f, FontStyle.Bold), ForeColor = UiHelper.Success };
    private readonly DataGridView _grid = UiHelper.CreateGrid();

    public RentalControl(RentalManager manager)
    {
        _manager = manager;
        Dock = DockStyle.Fill;
        BackColor = UiHelper.Surface;
        Padding = new Padding(20);

        var btnRent = UiHelper.CreateButton("Kirala", UiHelper.Success);
        btnRent.Width = 200;
        btnRent.Click += BtnRent_Click;

        var summary = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoSize = true };
        summary.Controls.Add(_lblDays);
        summary.Controls.Add(_lblTotal);

        // Form üstte, kiralama listesi altta tam genişlikte gösterilir
        var holder = new Panel { Dock = DockStyle.Left, Width = 560 };
        holder.Controls.Add(UiHelper.CreateButtonRow(btnRent));
        holder.Controls.Add(UiHelper.CreateFormTable(
            ("Müşteri", _cmbCustomer), ("Müsait Araç", _cmbVehicle),
            ("Başlangıç", _dtpStart), ("Bitiş", _dtpEnd), ("Hesap", summary)));
        var top = new Panel { Dock = DockStyle.Top, Height = 250 };
        top.Controls.Add(holder);

        Controls.Add(_grid);
        Controls.Add(top);
        Controls.Add(UiHelper.CreateTitle("Araç Kiralama"));

        _cmbVehicle.DropDownWidth = 480;

        // Olaylar: seçim/tarih değiştikçe ücret yeniden hesaplanır
        _dtpStart.ValueChanged += (_, _) => UpdatePrice();
        _dtpEnd.ValueChanged += (_, _) => UpdatePrice();
        _cmbVehicle.SelectedIndexChanged += (_, _) => UpdatePrice();

        _manager.DataChanged += (_, _) => RefreshAll();
        VisibleChanged += (_, _) => { if (Visible) RefreshAll(); };
        RefreshAll();
    }

    private void RefreshAll()
    {
        // Seçimleri koruyarak listeleri yenile
        int? customerId = (_cmbCustomer.SelectedItem as Customer)?.Id;
        _cmbCustomer.DataSource = _manager.Customers.ToList();
        if (customerId != null)
            _cmbCustomer.SelectedItem = _manager.Customers.FirstOrDefault(c => c.Id == customerId);

        // Yalnızca müsait araçlar listelenir
        _cmbVehicle.DataSource = _manager.Vehicles.Where(v => v.IsAvailable).ToList();

        _grid.DataSource = _manager.Rentals.Select(r => new
        {
            r.Id,
            Customer = r.Customer.FullName,
            Vehicle = $"{r.Vehicle.Plate} - {r.Vehicle.Brand} {r.Vehicle.Model}",
            Start = AppCulture.Date(r.StartDate),
            End = AppCulture.Date(r.EndDate),
            r.TotalDays,
            TotalPrice = AppCulture.Money(r.TotalPrice),
            Status = r.IsActive ? "Aktif" : "Tamamlandı"
        }).ToList();
        UiHelper.SetHeaders(_grid, ("Id", "ID"), ("Customer", "Müşteri"), ("Vehicle", "Araç"),
            ("Start", "Başlangıç"), ("End", "Bitiş"), ("TotalDays", "Gün"),
            ("TotalPrice", "Toplam Ücret"), ("Status", "Durum"));
        UiHelper.ResetSelection(_grid);
        UpdatePrice();
    }

    private void UpdatePrice()
    {
        if (_dtpEnd.Value.Date < _dtpStart.Value.Date)
        {
            _lblDays.Text = "Bitiş tarihi başlangıçtan önce!";
            _lblTotal.Text = "";
            return;
        }

        int days = Math.Max(1, (_dtpEnd.Value.Date - _dtpStart.Value.Date).Days);
        _lblDays.Text = $"Süre: {days} gün";
        if (_cmbVehicle.SelectedItem is Vehicle v)
            _lblTotal.Text = $"Toplam: {AppCulture.Money(v.CalculateRentalPrice(days))}";   // polimorfik hesap
        else
            _lblTotal.Text = "Müsait araç yok";
    }

    private void BtnRent_Click(object? sender, EventArgs e)
    {
        try
        {
            Rental r = _manager.CreateRental(
                _cmbCustomer.SelectedItem as Customer,
                _cmbVehicle.SelectedItem as Vehicle,
                _dtpStart.Value, _dtpEnd.Value);
            UiHelper.ShowSuccess($"Kiralama oluşturuldu.\n{r.TotalDays} gün - {AppCulture.Money(r.TotalPrice)}");
        }
        catch (Exception ex) when (ex is ArgumentException or RentalException or InvalidOperationException)
        {
            UiHelper.ShowError(ex.Message);
        }
    }
}
