using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BUS_QuanlyKhachSan.NhanSu;
using QuanLyKhachSan.Views.Dialogs;

namespace QuanLyKhachSan.Helpers;

public static class NhanSuGiaoDien
{
    // Thêm: Nhân sự và Kho dùng chung cách mở khóa; mã và trường tự tính luôn chỉ xem.
    public static void MoKhoa(DependencyObject vung, bool sua)
    {
        foreach (var con in LogicalTreeHelper.GetChildren(vung).OfType<DependencyObject>())
        {
            bool choSua = sua && ((con as FrameworkElement)?.Tag as string) != "Khoa";
            if (con is TextBox oNhap)
            {
                oNhap.IsReadOnly = !choSua;
                // Thêm: OneWay không ghi số đang gõ vào ET; ép đọc lại để Hủy sửa trả đúng số cũ.
                if (!sua) oNhap.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
            }
            else if (con is ComboBox oChon) oChon.IsEnabled = choSua;
            else if (con is DatePicker ngay)
            {
                ngay.IsEnabled = choSua;
                if (!sua) ngay.GetBindingExpression(DatePicker.SelectedDateProperty)?.UpdateTarget();
            }
            else MoKhoa(con, sua);
        }
    }
    // Thêm: ET chỉ gồm các trường đơn; dùng chung bản sao độc lập cho form Thêm/Sửa, không sửa dòng gốc.
    public static T BanNhap<T>(T duLieu) where T : class =>
        JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(duLieu))!;

    public static void DanhMuc(ComboBox oChon, IEnumerable<KeyValuePair<string, string>> nguon, string giaTri)
    {
        oChon.DisplayMemberPath = "Value";
        oChon.SelectedValuePath = "Key";
        oChon.ItemContainerStyle = (Style)oChon.FindResource("ReadableComboBoxItem");
        oChon.ItemsSource = nguon.ToList();
        oChon.SelectedValue = giaTri;
    }

    public static void LuaChon<T>(ComboBox oChon, IEnumerable<T> nguon, string ten, string ma, object giaTri)
    {
        oChon.DisplayMemberPath = ten;
        oChon.SelectedValuePath = ma;
        oChon.ItemContainerStyle = (Style)oChon.FindResource("ReadableComboBoxItem");
        oChon.ItemsSource = nguon.ToList();
        oChon.SelectedValue = giaTri;
    }

    public static string TrangThai(ComboBox oChon) => oChon.SelectedValue as string ?? "";

    public static bool XacNhanXoa(Window chu, string noiDung)
    {
        var xacNhan = new XacNhanDemoWindow { Owner = chu };
        xacNhan.txtNoiDung.Text = noiDung + "? Thao tác này không thể hoàn tác.";
        return xacNhan.ShowDialog() == true;
    }

    // Thêm: dùng lại cùng cách chặn gõ/dán của Phòng ban cho 6 ô tìm kiếm còn lại.
    public static void GanTimKiem(TextBox oNhap, TextBlock loi)
    {
        string hopLeTruoc = oNhap.Text;
        void BaoLoi(bool coLoi)
        {
            loi.Visibility = coLoi ? Visibility.Visible : Visibility.Collapsed;
            oNhap.SetResourceReference(Control.BorderBrushProperty, coLoi ? "DangerBrush" : "BorderBrush");
        }
        bool ChoPhep(string vanBan)
        {
            string duKien = oNhap.Text.Remove(oNhap.SelectionStart, oNhap.SelectionLength).Insert(oNhap.SelectionStart, vanBan);
            bool hopLe = BUS_PhongBan.TuKhoaTimKiemHopLe(duKien);
            BaoLoi(!hopLe);
            return hopLe;
        }
        oNhap.PreviewTextInput += (_, e) => e.Handled = !ChoPhep(e.Text);
        DataObject.AddPastingHandler(oNhap, (_, e) =>
        {
            string? vanBan = e.DataObject.GetData(DataFormats.UnicodeText, true) as string;
            if (vanBan == null || !ChoPhep(vanBan)) { e.CancelCommand(); BaoLoi(true); }
        });
        oNhap.TextChanged += (_, _) =>
        {
            if (BUS_PhongBan.TuKhoaTimKiemHopLe(oNhap.Text)) { hopLeTruoc = oNhap.Text; BaoLoi(false); }
            else
            {
                int viTri = oNhap.CaretIndex;
                oNhap.Text = hopLeTruoc;
                oNhap.CaretIndex = Math.Min(viTri, hopLeTruoc.Length);
                BaoLoi(true);
            }
        };
    }

    public static void MoiTimKiem(TextBox oNhap, TextBlock loi, ComboBox loc)
    {
        oNhap.Clear();
        loi.Visibility = Visibility.Collapsed;
        oNhap.SetResourceReference(Control.BorderBrushProperty, "BorderBrush");
        loc.SelectedIndex = 0;
    }
}
