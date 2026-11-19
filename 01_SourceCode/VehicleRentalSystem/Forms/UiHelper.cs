namespace VehicleRentalSystem.Forms;

/// <summary>Formlarda tekrar eden arayüz kodlarını toplayan yardımcı sınıf.</summary>
internal static class UiHelper
{
    public static readonly Color Primary = Color.FromArgb(31, 78, 121);
    public static readonly Color Success = Color.FromArgb(46, 125, 50);
    public static readonly Color Danger = Color.FromArgb(192, 57, 43);
    public static readonly Color Neutral = Color.FromArgb(96, 108, 118);
    public static readonly Color Surface = Color.FromArgb(244, 246, 249);

    public static DataGridView CreateGrid() => new DataGridView
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        AllowUserToResizeRows = false,
        MultiSelect = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        BackgroundColor = Color.White,
        BorderStyle = BorderStyle.None,
        RowHeadersVisible = false,
        EnableHeadersVisualStyles = false,
        ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
        ColumnHeadersHeight = 34,
        ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Primary,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
        }
    }.WithNoAutoSelection();

    /// <summary>
    /// DataGridView ilk gösterildiğinde ilk satırı kendiliğinden seçer; bu, kullanıcı
    /// farkında olmadan o satırı silmesine yol açabilir. Gösterimden sonra seçimi temizleriz.
    /// </summary>
    private static DataGridView WithNoAutoSelection(this DataGridView grid)
    {
        void Reset() { if (grid.IsHandleCreated) grid.BeginInvoke(new Action(() => ResetSelection(grid))); }
        grid.VisibleChanged += (_, _) => { if (grid.Visible) Reset(); };
        grid.HandleCreated += (_, _) => Reset();
        grid.DataBindingComplete += (_, _) => Reset();
        return grid;
    }

    public static Button CreateButton(string text, Color color) => new()
    {
        Text = text,
        BackColor = color,
        ForeColor = Color.White,
        FlatStyle = FlatStyle.Flat,
        Height = 36,
        Width = 128,
        Margin = new Padding(0, 4, 8, 4),
        Cursor = Cursors.Hand
    };

    public static Label CreateTitle(string text) => new()
    {
        Text = text,
        Dock = DockStyle.Top,
        Height = 44,
        Font = new Font("Segoe UI", 15f, FontStyle.Bold),
        ForeColor = Primary
    };

    /// <summary>Etiket + kontrol çiftlerinden iki sütunlu bir giriş formu üretir.</summary>
    public static TableLayoutPanel CreateFormTable(params (string Label, Control Input)[] rows)
    {
        var table = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 2
        };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        foreach (var (label, input) in rows)
        {
            var lbl = new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 8, 0, 8) };
            input.Dock = DockStyle.Fill;
            input.Margin = new Padding(0, 4, 0, 4);
            table.RowCount++;
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            table.Controls.Add(lbl, 0, table.RowCount - 1);
            table.Controls.Add(input, 1, table.RowCount - 1);
        }
        return table;
    }

    public static FlowLayoutPanel CreateButtonRow(params Button[] buttons)
    {
        var flow = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(0, 8, 0, 0) };
        flow.Controls.AddRange(buttons);
        return flow;
    }

    public static void SetHeaders(DataGridView grid, params (string Column, string Header)[] headers)
    {
        foreach (var (column, header) in headers)
        {
            if (grid.Columns[column] != null)
                grid.Columns[column]!.HeaderText = header;
        }
        FitColumns(grid);
    }

    /// <summary>
    /// Sütunlar tablo genişliğine sığar (yatay kaydırma çıkmaz); metin sütunlarına daha fazla pay verilir.
    /// </summary>
    public static void FitColumns(DataGridView grid)
    {
        foreach (DataGridViewColumn col in grid.Columns)
        {
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            // Sütun adına göre en az genişlik (varsayılan 100 pikseli aşmamalı: WinForms,
            // Fill modunda varsayılandan büyük MinimumWidth verilince NullReferenceException fırlatabiliyor)
            col.MinimumWidth = col.Name switch
            {
                "Id" => 34,
                "Year" or "TotalDays" => 44,
                "Type" => 92,
                "Plate" => 88,
                "DailyPrice" or "TotalPrice" => 96,
                "Status" => 62,
                "Start" or "End" => 86,
                "Vehicle" => 100,
                "Customer" => 100,
                _ => 70
            };
            col.FillWeight = col.Name switch
            {
                "Id" => 30,
                "Year" or "TotalDays" => 42,
                "Status" => 70,
                "Vehicle" => 190,
                "Customer" or "Brand" => 110,
                _ => 100
            };
        }
    }

    /// <summary>Tabloda hiçbir satırın otomatik seçili kalmamasını sağlar.</summary>
    public static void ResetSelection(DataGridView grid)
    {
        grid.CurrentCell = null;
        grid.ClearSelection();
    }

    /// <summary>Seçili satırın "Id" hücresini okur; seçim yoksa null döner.</summary>
    public static int? SelectedId(DataGridView grid)
    {
        if (grid.CurrentRow == null || !grid.CurrentRow.Selected) return null;
        object? value = grid.CurrentRow.Cells["Id"].Value;
        return value is int id ? id : null;
    }

    public static void ShowSuccess(string message) =>
        MessageBox.Show(message, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

    public static void ShowError(string message) =>
        MessageBox.Show(message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

    public static void ShowWarning(string message) =>
        MessageBox.Show(message, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    public static bool Confirm(string message) =>
        MessageBox.Show(message, "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
}
