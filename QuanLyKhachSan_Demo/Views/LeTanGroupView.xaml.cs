using System.Windows;
using System.Windows.Controls;

namespace QuanLyKhachSan.Views
{
    public partial class LeTanGroupView : UserControl
    {
        public LeTanGroupView()
        {
            InitializeComponent();
            txtDataMode.Text = DAL.CauHinhKetNoi.DaCauHinh ? "Lễ tân: dữ liệu SQL Server" :
                "Cấu hình connection.txt hoặc QLKS_CONNECTION để dùng chức năng Lễ tân.";
            viewDatPhong.dgvDatPhong.SelectionChanged += (_, _) =>
                viewPhanBoPhong.SetDatPhong(viewDatPhong.dgvDatPhong.SelectedItem as System.Data.DataRowView);
            viewDatPhong.DatPhongChanged += (_, _) =>
                viewPhanBoPhong.SetDatPhong(viewDatPhong.dgvDatPhong.SelectedItem as System.Data.DataRowView);
            viewPhanBoPhong.PhanBoChanged += (_, _) => {
                viewDatPhong.RefreshSqlFromGroup();
            };
            viewPhanBoPhong.SetDatPhong(viewDatPhong.dgvDatPhong.SelectedItem as System.Data.DataRowView);
            UpdateContextTabs();
        }

        private void TabNhom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source == tabNhom) UpdateContextTabs();
        }

        private void UpdateContextTabs()
        {
            // SelectionChanged có thể chạy khi XAML chưa tạo xong các tab.
            if (tabDatPhong == null || tabPhanBoPhong == null || tabTuVanAI == null ||
                tabLuuTru == null || tabLichSuLuuTru == null || tabHoaDon == null || tabLichSuThuTien == null) return;
            bool bookingSelected = tabDatPhong.IsSelected || tabPhanBoPhong.IsSelected || tabTuVanAI.IsSelected;
            tabPhanBoPhong.Visibility = bookingSelected && DemoSession.CanOpen(nameof(PhanBoPhongView))
                ? Visibility.Visible : Visibility.Collapsed;
            tabTuVanAI.Visibility = bookingSelected && DemoSession.CanOpen(nameof(TuVanAIView))
                ? Visibility.Visible : Visibility.Collapsed;
            tabLichSuLuuTru.Visibility = (tabLuuTru.IsSelected || tabLichSuLuuTru.IsSelected)
                && DemoSession.CanOpen(nameof(LichSuLuuTruView)) ? Visibility.Visible : Visibility.Collapsed;
            tabLichSuThuTien.Visibility = (tabHoaDon.IsSelected || tabLichSuThuTien.IsSelected)
                && DemoSession.CanOpen(nameof(LichSuThuTienView)) ? Visibility.Visible : Visibility.Collapsed;
        }

    }
}
