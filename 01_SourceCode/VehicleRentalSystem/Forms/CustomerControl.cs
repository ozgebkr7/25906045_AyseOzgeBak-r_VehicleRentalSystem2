using VehicleRentalSystem.Models;
using VehicleRentalSystem.Services;

namespace VehicleRentalSystem.Forms;

/// <summary>Müşteri yönetimi: ekleme, listeleme, güncelleme ve silme.</summary>
public class CustomerControl : UserControl
{
    private readonly RentalManager _manager;
    private readonly TextBox _txtFirstName = new();
    private readonly TextBox _txtLastName = new();
    private readonly TextBox _txtPhone = new() { MaxLength = 11 };
    private readonly TextBox _txtLicense = new();
    private readonly DataGridView _grid = UiHelper.CreateGrid();
    private bool _loadingSelection;

    public CustomerControl(RentalManager manager)
    {
        _manager = manager;
        Dock = DockStyle.Fill;
        BackColor = UiHelper.Surface;
        Padding = new Padding(20);

        var btnAdd = UiHelper.CreateButton("Ekle", UiHelper.Success);
        var btnUpdate = UiHelper.CreateButton("Güncelle", UiHelper.Primary);
        var btnDelete = UiHelper.CreateButton("Sil", UiHelper.Danger);
        var btnClear = UiHelper.CreateButton("Temizle", UiHelper.Neutral);
        btnAdd.Click += BtnAdd_Click;
        btnUpdate.Click += BtnUpdate_Click;
        btnDelete.Click += BtnDelete_Click;
        btnClear.Click += (_, _) => ClearForm();

        var left = new Panel { Dock = DockStyle.Left, Width = 310, Padding = new Padding(0, 0, 16, 0) };
        left.Controls.Add(UiHelper.CreateButtonRow(btnAdd, btnUpdate, btnDelete, btnClear));
        left.Controls.Add(UiHelper.CreateFormTable(
            ("Ad", _txtFirstName), ("Soyad", _txtLastName),
            ("Telefon", _txtPhone), ("Ehliyet No", _txtLicense)));

        Controls.Add(_grid);
        Controls.Add(left);
        Controls.Add(UiHelper.CreateTitle("Müşteri Yönetimi"));

        // Form alanları yalnızca kullanıcı bir satıra tıkladığında / ok tuşuyla gezdiğinde dolar
        _grid.CellClick += (_, _) => LoadSelection();
        _grid.KeyUp += (_, e) => { if (e.KeyCode is Keys.Up or Keys.Down) LoadSelection(); };
        _manager.DataChanged += (_, _) => RefreshGrid();
        RefreshGrid();
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        try
        {
            _manager.AddCustomer(_txtFirstName.Text, _txtLastName.Text, _txtPhone.Text, _txtLicense.Text);
            UiHelper.ShowSuccess("Müşteri başarıyla eklendi.");
            ClearForm();
        }
        catch (ArgumentException ex)
        {
            UiHelper.ShowError(ex.Message);
        }
    }

    private void BtnUpdate_Click(object? sender, EventArgs e)
    {
        int? id = UiHelper.SelectedId(_grid);
        if (id == null) { UiHelper.ShowWarning("Güncellemek için tablodan bir müşteri seçiniz."); return; }
        try
        {
            _manager.UpdateCustomer(id.Value, _txtFirstName.Text, _txtLastName.Text, _txtPhone.Text, _txtLicense.Text);
            UiHelper.ShowSuccess("Müşteri bilgileri güncellendi.");
        }
        catch (Exception ex) when (ex is ArgumentException or RentalException)
        {
            UiHelper.ShowError(ex.Message);
        }
    }

    private void BtnDelete_Click(object? sender, EventArgs e)
    {
        int? id = UiHelper.SelectedId(_grid);
        if (id == null) { UiHelper.ShowWarning("Silmek için tablodan bir müşteri seçiniz."); return; }
        if (!UiHelper.Confirm("Seçili müşteri silinsin mi?")) return;
        try
        {
            _manager.DeleteCustomer(id.Value);
            UiHelper.ShowSuccess("Müşteri silindi.");
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
            Customer c = _manager.FindCustomer(id.Value);
            _txtFirstName.Text = c.FirstName;
            _txtLastName.Text = c.LastName;
            _txtPhone.Text = c.Phone;
            _txtLicense.Text = c.LicenseNumber;
        }
        catch (RentalException) { }
        finally { _loadingSelection = false; }
    }

    private void ClearForm()
    {
        _txtFirstName.Clear(); _txtLastName.Clear(); _txtPhone.Clear(); _txtLicense.Clear();
        UiHelper.ResetSelection(_grid);
    }

    private void RefreshGrid()
    {
        _grid.DataSource = _manager.Customers
            .Select(c => new { c.Id, c.FirstName, c.LastName, c.Phone, c.LicenseNumber }).ToList();
        UiHelper.SetHeaders(_grid, ("Id", "ID"), ("FirstName", "Ad"), ("LastName", "Soyad"),
            ("Phone", "Telefon"), ("LicenseNumber", "Ehliyet No"));
        UiHelper.ResetSelection(_grid);
    }
}
