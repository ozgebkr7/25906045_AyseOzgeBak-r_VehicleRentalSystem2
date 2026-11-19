using System.Drawing;
using System.Windows.Forms;
using VehicleRentalSystem.Forms;
using Xunit;

namespace VehicleRentalSystem.Tests;

/// <summary>
/// Ana formu gerçekten açar, her menü sayfasına geçer ve ekran görüntüsü alır.
/// Görüntüler rapora konabilir (CI'da artifact olarak yüklenir).
/// </summary>
public class UiSmokeTests
{
    [Fact]
    public void MainForm_OpensAllPages_AndSavesScreenshots()
    {
        string outDir = Environment.GetEnvironmentVariable("SCREENSHOT_DIR")
                        ?? Path.Combine(Path.GetTempPath(), "vrs-screens");
        Directory.CreateDirectory(outDir);
        Exception? error = null;

        var t = new Thread(() =>
        {
            try
            {
                ApplicationConfiguration.Initialize();
                using var form = new MainForm();
                form.StartPosition = FormStartPosition.Manual;
                form.Location = new Point(0, 0);
                form.Show();
                form.Size = new Size(1400, 800);   // CI ekranı küçük olabilir; görüntü için büyüt
                Application.DoEvents();

                // Menü düğmeleri metne göre bulunur (görsel sıradan bağımsız)
                var buttons = Descendants(form).OfType<Button>().Where(b => b.Dock == DockStyle.Top).ToList();
                Assert.Equal(5, buttons.Count);
                (string Text, string File)[] pages =
                {
                    ("Ana Sayfa", "1_AnaSayfa"), ("Araç Yönetimi", "2_AracYonetimi"),
                    ("Müşteri Yönetimi", "3_MusteriYonetimi"), ("Araç Kiralama", "4_AracKiralama"),
                    ("Araç İade", "5_AracIade")
                };
                for (int i = 0; i < pages.Length; i++)
                {
                    var button = buttons.Single(b => b.Text.Trim() == pages[i].Text);
                    button.PerformClick();
                    Application.DoEvents();
                    using var bmp = new Bitmap(form.Width, form.Height);
                    form.DrawToBitmap(bmp, new Rectangle(0, 0, form.Width, form.Height));
                    bmp.Save(Path.Combine(outDir, pages[i].File + ".png"));
                }
                form.Close();
            }
            catch (Exception ex) { error = ex; }
        });
        t.SetApartmentState(ApartmentState.STA);
        t.Start();
        t.Join();

        Assert.Null(error);
        Assert.Equal(5, Directory.GetFiles(outDir, "*.png").Length);
    }

    private static IEnumerable<Control> Descendants(Control root)
    {
        foreach (Control c in root.Controls)
        {
            yield return c;
            foreach (var d in Descendants(c)) yield return d;
        }
    }
}
