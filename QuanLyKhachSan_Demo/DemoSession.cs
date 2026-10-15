using System.Windows;
using System.Windows.Controls;

namespace QuanLyKhachSan;

// Các mã vai trò tương ứng với TaiKhoan.MaVaiTro; tài khoản chỉ dùng cho demo.
public enum DemoRole { Admin = 1, QuanLy, LeTan, ThuNgan, BuongPhong }
public sealed record DemoAccount(string Username, string Password, DemoRole Role, string RoleName);

public static class DemoSession
{
    public static IReadOnlyList<DemoAccount> Accounts { get; } = Array.AsReadOnly(new[]
    {
        new DemoAccount("admin", "admin123", DemoRole.Admin, "Quản trị hệ thống"),
        new DemoAccount("quanly", "demo123", DemoRole.QuanLy, "Quản lý khách sạn"),
        new DemoAccount("letan", "demo123", DemoRole.LeTan, "Lễ tân"),
        new DemoAccount("thungan", "demo123", DemoRole.ThuNgan, "Thu ngân"),
        new DemoAccount("buongphong", "demo123", DemoRole.BuongPhong, "Nhân viên buồng phòng")
    });

    public static DemoAccount? Current { get; private set; }

    public static bool SignIn(string username, string password)
    {
        Current = Accounts.FirstOrDefault(a =>
            a.Username.Equals(username.Trim(), StringComparison.OrdinalIgnoreCase) && a.Password == password);
        return Current != null;
    }

    public static void SignOut() => Current = null;

    public static string Describe(DemoRole role) => role switch
    {
        DemoRole.Admin => "Toàn bộ nghiệp vụ, tài khoản và phân quyền",
        DemoRole.QuanLy => "Nghiệp vụ, báo cáo và nhật ký; không quản trị tài khoản",
        DemoRole.LeTan => "Đặt phòng, phân bổ phòng, tư vấn AI, lưu trú, khách hàng và ghi nhận dịch vụ",
        DemoRole.ThuNgan => "Lập hóa đơn, thu tiền, áp dụng ưu đãi; không hủy hóa đơn",
        DemoRole.BuongPhong => "Xem phòng, bắt đầu/hoàn thành dọn và báo yêu cầu bảo trì",
        _ => ""
    };

    private static readonly Dictionary<string, string[]> Groups = new()
    {
        ["LeTanGroupView"] = ["DatPhongView", "LuuTruView", "KhachHangView", "HoaDonView", "PhanBoPhongView", "TuVanAIView", "LichSuLuuTruView", "LichSuThuTienView"],
        ["PhongBuongGroupView"] = ["PhongView", "BuongPhongView", "DanhMucPhongView"],
        ["DichVuKhachSanGroupView"] = ["DichVuView", "GoiUuDaiView"],
        ["KhoVatTuGroupView"] = ["KhoView", "VatTuNhaCungCapView", "TieuHaoView"],
        ["QuanLyNhanSuGroupView"] = ["NhanVienView", "NhanSuView"],
        ["HeThongGroupView"] = ["TaiKhoanView", "NhatKyView"]
    };

    public static bool CanOpen(string view)
    {
        if (Current == null) return false;
        if (view == "LichSuLuuTruView") return CanOpen("LuuTruView");
        if (view == "LichSuThuTienView") return CanOpen("HoaDonView");
        if (Groups.TryGetValue(view, out var children)) return children.Any(CanOpen);
        if (view is not ("DashboardView" or "BaoCaoView") &&
            !Groups.Values.Any(views => views.Contains(view))) return false;

        return Current.Role switch
        {
            DemoRole.Admin => true,
            DemoRole.QuanLy => view != "TaiKhoanView",
            DemoRole.LeTan => view is "DatPhongView" or "LuuTruView" or "KhachHangView"
                or "PhongView" or "DichVuView" or "GoiUuDaiView" or "PhanBoPhongView" or "TuVanAIView",
            DemoRole.ThuNgan => view is "HoaDonView" or "KhachHangView" or "GoiUuDaiView",
            DemoRole.BuongPhong => view is "PhongView" or "BuongPhongView",
            _ => false
        };
    }

    public static bool CanUseButton(string view, string button)
    {
        if (!CanOpen(view)) return false;
        if (Current!.Role is DemoRole.Admin or DemoRole.QuanLy) return true;
        if (button.StartsWith("btnTim") || button.StartsWith("btnMoi") || button.EndsWith("ChiTiet")) return true;

        return Current.Role switch
        {
            DemoRole.LeTan => view is "DatPhongView" or "LuuTruView" or "KhachHangView"
                or "PhanBoPhongView" or "TuVanAIView"
                || (view == "DichVuView" && button.StartsWith("btnSuDungDichVu")),
            DemoRole.ThuNgan => view == "HoaDonView" && button is
                "btnHoaDonTaoHoaDon" or "btnHoaDonThuTienMat" or "btnHoaDonApDungUuDai" or "btnHoaDonXemMauIn",
            DemoRole.BuongPhong => view == "BuongPhongView" && button is
                "btnDonPhongBatDauDon" or "btnDonPhongHoanThanhDon" or "btnBaoTriTaoYeuCau",
            _ => false
        };
    }

    // Duyệt logical tree để áp dụng cả nội dung của các tab chưa được chọn.
    public static void ApplyAccess(DependencyObject element, string view = "")
    {
        if (element is UserControl control)
        {
            view = control.GetType().Name;
            if (!CanOpen(view))
            {
                control.Visibility = Visibility.Collapsed;
                control.IsEnabled = false;
                return;
            }
        }

        if (element is TabItem tab && tab.Content is UserControl content && !CanOpen(content.GetType().Name))
        {
            tab.Visibility = Visibility.Collapsed;
            tab.IsEnabled = false;
        }
        if (element is Button button && !CanUseButton(view, button.Name))
        {
            button.IsEnabled = false;
            button.ToolTip = "Vai trò hiện tại không được thực hiện thao tác này.";
            ToolTipService.SetShowOnDisabled(button, true);
        }

        foreach (var child in LogicalTreeHelper.GetChildren(element).OfType<DependencyObject>())
            ApplyAccess(child, view);

        if (element is TabControl tabs &&
            (tabs.SelectedItem is not TabItem selected || selected.Visibility != Visibility.Visible))
            tabs.SelectedItem = tabs.Items.OfType<TabItem>().FirstOrDefault(t => t.Visibility == Visibility.Visible);
    }
}
