using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class NhatKyThaoTacChiTietWindow : Window
{
    private readonly NhatKyThaoTacMau _item;

    public NhatKyThaoTacChiTietWindow(NhatKyThaoTacMau? item = null)
    {
        InitializeComponent();
        _item = item ?? new NhatKyThaoTacMau();
        DataContext = _item;

    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();
}
