using VehicleRentalSystem.Models;
using VehicleRentalSystem.Services;

namespace VehicleRentalSystem.Forms;

/// <summary>Araç iade: aktif kiralamaları listeler ve seçileni tamamlar.</summary>
public class ReturnControl : UserControl
{
    private readonly RentalManager _manager;
    private readonly DataGridView _grid = UiHelper.CreateGrid();

    public ReturnControl(RentalManager manager)
    {
        _manager = manager;
        Dock = DockStyle.Fill;
        BackColor = UiHelper.Surface;
        Padding = new Padding(20);

        var btnReturn = UiHelper.CreateButton("Seçili Aracı İade Al", UiHelper.Primary);
        btnReturn.Width = 200;
        btnReturn.Click += BtnReturn_Click;
        var bottom = new Panel { Dock = DockStyle.Bottom, Height = 52 };
        bottom.Controls.Add(btnReturn);

        Controls.Add(_grid);
        Controls.Add(bottom);
        Controls.Add(UiHelper.CreateTitle("Araç İade"));

        _manager.DataChanged += (_, _) => RefreshGrid();
        VisibleChanged += (_, _) => { if (Visible) RefreshGrid(); };
        RefreshGrid();
    }

    private void RefreshGrid()
    {
        _grid.DataSource = _manager.Rentals.Where(r => r.IsActive).Select(r => new
        {
            r.Id,
            Customer = r.Customer.FullName,
            Vehicle = $"{r.Vehicle.Plate} - {r.Vehicle.Brand} {r.Vehicle.Model}",
            Start = AppCulture.Date(r.StartDate),
            End = AppCulture.Date(r.EndDate),
            r.TotalDays,
            TotalPrice = AppCulture.Money(r.TotalPrice)
        }).ToList();
        UiHelper.SetHeaders(_grid, ("Id", "ID"), ("Customer", "Müşteri"), ("Vehicle", "Araç"),
            ("Start", "Başlangıç"), ("End", "Bitiş"), ("TotalDays", "Gün"), ("TotalPrice", "Toplam Ücret"));
        UiHelper.ResetSelection(_grid);
    }

    private void BtnReturn_Click(object? sender, EventArgs e)
    {
        int? id = UiHelper.SelectedId(_grid);
        if (id == null) { UiHelper.ShowWarning("İade için tablodan bir kiralama seçiniz."); return; }
        try
        {
            _manager.ReturnRental(id.Value);
            UiHelper.ShowSuccess("İade tamamlandı. Araç yeniden müsait duruma getirildi.");
        }
        catch (RentalException ex)
        {
            UiHelper.ShowError(ex.Message);
        }
    }
}
