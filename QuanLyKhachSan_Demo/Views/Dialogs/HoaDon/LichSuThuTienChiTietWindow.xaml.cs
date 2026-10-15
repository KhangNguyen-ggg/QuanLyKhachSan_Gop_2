using System;
using System.Windows;
using System.Windows.Controls;
using ET_QuanLyKhachSan;
// Ghép Gop: dùng đúng namespace nghiệp vụ gốc, giữ nguyên luồng giao diện.
using BUS_QuanlyKhachSan.LeTanValuuTru;

namespace QuanLyKhachSan.Views.Dialogs;
public partial class LichSuThuTienChiTietWindow : Window
{
    public LichSuThuTienChiTietWindow()
    {
        InitializeComponent();
    }
    private void btnDong_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
