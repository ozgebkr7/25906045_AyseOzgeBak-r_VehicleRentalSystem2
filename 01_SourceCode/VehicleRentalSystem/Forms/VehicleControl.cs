using VehicleRentalSystem.Models;
using VehicleRentalSystem.Services;

namespace VehicleRentalSystem.Forms;

/// <summary>Araç yönetimi: ekleme, listeleme, güncelleme ve silme.</summary>
public class VehicleControl : UserControl
{
    private readonly RentalManager _manager;
    private readonly TextBox _txtPlate = new();
    private readonly TextBox _txtBrand = new();
    private readonly TextBox _txtModel = new();
    private readonly NumericUpDown _nudYear = new() { Minimum = 1980, Maximum = DateTime.Now.Year + 1, Value = DateTime.Now.Year };
    private readonly NumericUpDown _nudPrice = new() { Minimum = 0, Maximum = 1000000, DecimalPlaces = 2, Increment = 50 };
    private readonly ComboBox _cmbType = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly NumericUpDown _nudExtra = new() { Minimum = 0, Maximum = 100000, Value = 4 };
    private readonly Label _lblExtra = new() { AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 8, 0, 8) };
    private readonly DataGridView _grid = UiHelper.CreateGrid();
    private bool _loadingSelection;

    private static readonly string[] TypeNames = { "Otomobil", "Ticari Araç", "Motosiklet" };
    private static readonly string[] ExtraLabels = { "Kapı Sayısı", "Yük Kapasitesi (kg)", "Motor Hacmi (cc)" };

    public VehicleControl(RentalManager manager)
    {
        _manager = manager;
        Dock = DockStyle.Fill;
        BackColor = UiHelper.Surface;
        Padding = new Padding(20);

        _cmbType.Items.AddRange(TypeNames);
        _cmbType.SelectedIndex = 0;
        _cmbType.SelectedIndexChanged += (_, _) => _lblExtra.Text = ExtraLabels[_cmbType.SelectedIndex];
        _lblExtra.Text = ExtraLabels[0];

        var btnAdd = UiHelper.CreateButton("Ekle", UiHelper.Success);
        var btnUpdate = UiHelper.CreateButton("Güncelle", UiHelper.Primary);
        var btnDelete = UiHelper.CreateButton("Sil", UiHelper.Danger);
        var btnClear = UiHelper.CreateButton("Temizle", UiHelper.Neutral);
        btnAdd.Click += BtnAdd_Click;
        btnUpdate.Click += BtnUpdate_Click;
        btnDelete.Click += BtnDelete_Click;
        btnClear.Click += (_, _) => ClearForm();

        // Form tablosu (ek bilgi etiketi türe göre değişir)
        var form = UiHelper.CreateFormTable(
            ("Tür", _cmbType), ("Plaka", _txtPlate), ("Marka", _txtBrand), ("Model", _txtModel),
            ("Üretim Yılı", _nudYear), ("Günlük Ücret (₺)", _nudPrice), ("", _nudExtra));
        // Son satırın etiketini dinamik etikete çevir
        form.Controls.Remove(form.GetControlFromPosition(0, form.RowCount - 1));
        form.Controls.Add(_lblExtra, 0, form.RowCount - 1);

        var left = new Panel { Dock = DockStyle.Left, Width = 310, Padding = new Padding(0, 0, 16, 0) };
        left.Controls.Add(UiHelper.CreateButtonRow(btnAdd, btnUpdate, btnDelete, btnClear));
        left.Controls.Add(form);

        Controls.Add(_grid);
        Controls.Add(left);
        Controls.Add(UiHelper.CreateTitle("Araç Yönetimi"));

        // Form alanları yalnızca kullanıcı bir satıra tıkladığında / ok tuşuyla gezdiğinde dolar
        _grid.CellClick += (_, _) => LoadSelection();
        _grid.KeyUp += (_, e) => { if (e.KeyCode is Keys.Up or Keys.Down) LoadSelection(); };
        _manager.DataChanged += (_, _) => RefreshGrid();
        RefreshGrid();
    }

    private VehicleKind SelectedKind => (VehicleKind)_cmbType.SelectedIndex;

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        try
        {
            _manager.AddVehicle(SelectedKind, _txtPlate.Text, _txtBrand.Text, _txtModel.Text,
                (int)_nudYear.Value, _nudPrice.Value, (int)_nudExtra.Value);
            UiHelper.ShowSuccess("Araç başarıyla eklendi.");
            ClearForm();
        }
        catch (Exception ex) when (ex is ArgumentException or RentalException)
        {
            UiHelper.ShowError(ex.Message);
        }
    }

    private void BtnUpdate_Click(object? sender, EventArgs e)
    {
        int? id = UiHelper.SelectedId(_grid);
        if (id == null) { UiHelper.ShowWarning("Güncellemek için tablodan bir araç seçiniz."); return; }
        try
        {
            _manager.UpdateVehicle(id.Value, _txtPlate.Text, _txtBrand.Text, _txtModel.Text,
                (int)_nudYear.Value, _nudPrice.Value, (int)_nudExtra.Value);
            UiHelper.ShowSuccess("Araç bilgileri güncellendi.");
        }
        catch (Exception ex) when (ex is ArgumentException or RentalException)
        {
            UiHelper.ShowError(ex.Message);
        }
    }

    private void BtnDelete_Click(object? sender, EventArgs e)
    {
        int? id = UiHelper.SelectedId(_grid);
        if (id == null) { UiHelper.ShowWarning("Silmek için tablodan bir araç seçiniz."); return; }
        if (!UiHelper.Confirm("Seçili araç silinsin mi?")) return;
        try
        {
            _manager.DeleteVehicle(id.Value);
            UiHelper.ShowSuccess("Araç silindi.");
            ClearForm();
        }
        catch (RentalException ex)
        {
            UiHelper.ShowError(ex.Message);
        }
    }

    private void LoadSelection()
    {
        int? id = UiHelper.SelectedId(_grid);
        if (id == null || _loadingSelection) return;
        _loadingSelection = true;
        try
        {
            Vehicle v = _manager.FindVehicle(id.Value);
            _cmbType.SelectedIndex = (int)(v switch { Car => VehicleKind.Car, Truck => VehicleKind.Truck, _ => VehicleKind.Motorcycle });
            _cmbType.Enabled = false;   // tür sonradan değiştirilemez
            _txtPlate.Text = v.Plate;
            _txtBrand.Text = v.Brand;
            _txtModel.Text = v.Model;
            _nudYear.Value = v.Year;
            _nudPrice.Value = v.DailyPrice;
            _nudExtra.Value = v.ExtraValue;
        }
        catch (RentalException) { }
        finally { _loadingSelection = false; }
    }

    private void ClearForm()
    {
        _cmbType.Enabled = true;
        _cmbType.SelectedIndex = 0;
        _txtPlate.Clear(); _txtBrand.Clear(); _txtModel.Clear();
        _nudYear.Value = DateTime.Now.Year;
        _nudPrice.Value = 0;
        _nudExtra.Value = 4;
        UiHelper.ResetSelection(_grid);
    }

    private void RefreshGrid()
    {
        _grid.DataSource = _manager.Vehicles.Select(v => new
        {
            v.Id,
            Type = v.VehicleType,
            v.Plate,
            v.Brand,
            v.Model,
            v.Year,
            DailyPrice = AppCulture.Money(v.DailyPrice),
            Status = v.Status.ToString().Replace("Musait", "Müsait")
        }).ToList();
        UiHelper.SetHeaders(_grid, ("Id", "ID"), ("Type", "Tür"), ("Plate", "Plaka"), ("Brand", "Marka"),
            ("Model", "Model"), ("Year", "Yıl"), ("DailyPrice", "Günlük Ücret"), ("Status", "Durum"));
        UiHelper.ResetSelection(_grid);
    }
}
