using VehicleRentalSystem.Services;

namespace VehicleRentalSystem.Forms;

/// <summary>Ana pencere: sol menü ile modüller arasında geçiş sağlar.</summary>
public class MainForm : Form
{
    private readonly RentalManager _manager = new();
    private readonly Panel _content = new() { Dock = DockStyle.Fill };
    private readonly List<(Button Button, UserControl Page)> _pages = new();

    public MainForm()
    {
        Text = "Araç Kiralama Yönetim Sistemi";
        Size = new Size(1280, 740);
        MinimumSize = new Size(1150, 640);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9.5f);

        _manager.LoadSampleData();   // başlangıç için örnek kayıtlar

        var menu = new Panel { Dock = DockStyle.Left, Width = 190, BackColor = UiHelper.Primary };
        var logo = new Label
        {
            Text = "ARAÇ KİRALAMA", ForeColor = Color.White, Dock = DockStyle.Top, Height = 70,
            TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 13f, FontStyle.Bold)
        };

        // Her modül için bir menü düğmesi; sayfa kontrolleri bir kez oluşturulur
        var definitions = new (string Title, UserControl Page)[]
        {
            ("Ana Sayfa", new DashboardControl(_manager)),
            ("Araç Yönetimi", new VehicleControl(_manager)),
            ("Müşteri Yönetimi", new CustomerControl(_manager)),
            ("Araç Kiralama", new RentalControl(_manager)),
            ("Araç İade", new ReturnControl(_manager))
        };

        foreach (var (title, page) in definitions)
        {
            var btn = new Button
            {
                Text = "   " + title, Dock = DockStyle.Top, Height = 52, FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White, BackColor = UiHelper.Primary, TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10.5f), Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (_, _) => ShowPage(page);
            page.Visible = false;
            _content.Controls.Add(page);
            _pages.Add((btn, page));
        }

        // Dock=Top kontrollerde sondan eklenen en üstte durur; sırayı korumak için ters ekle
        for (int i = _pages.Count - 1; i >= 0; i--) menu.Controls.Add(_pages[i].Button);
        menu.Controls.Add(logo);

        Controls.Add(_content);
        Controls.Add(menu);
        ShowPage(_pages[0].Page);
    }

    private void ShowPage(UserControl target)
    {
        foreach (var (button, page) in _pages)
        {
            bool active = page == target;
            page.Visible = active;
            button.BackColor = active ? Color.FromArgb(21, 58, 94) : UiHelper.Primary;
        }
    }
}
