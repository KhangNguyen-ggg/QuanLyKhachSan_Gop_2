using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class TonKhoChiTietWindow : Window
{
    private readonly TonKhoMau _item;

    public TonKhoChiTietWindow(TonKhoMau? item = null)
    {
        InitializeComponent();
        _item = item ?? new TonKhoMau();
        DataContext = _item;
        btnTonKhoNhapKho.IsEnabled = DemoSession.CanUseButton("KhoView", "btnTonKhoNhapKho");
        btnTonKhoXuatKho.IsEnabled = DemoSession.CanUseButton("KhoView", "btnTonKhoXuatKho");
        btnTonKhoDieuChinhTon.IsEnabled = DemoSession.CanUseButton("KhoView", "btnTonKhoDieuChinhTon");

    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnTonKhoNhapKho_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("KhoView", "btnTonKhoNhapKho")) return;
        TonKhoNhapKhoWindow form = new TonKhoNhapKhoWindow();
        form.Owner = this;
        form.DataContext = _item;
        form.Title += $" — {_item.MaVatTu}";
        var optionKho = form.cboKho.Items.OfType<ComboBoxItem>().FirstOrDefault(i => Equals(i.Content, _item.Kho));
        if (optionKho == null) { optionKho = new ComboBoxItem { Content = _item.Kho }; form.cboKho.Items.Add(optionKho); }
        form.cboKho.SelectedItem = optionKho;
        var optionVatTu = form.cboVatTu.Items.OfType<ComboBoxItem>().FirstOrDefault(i => Equals(i.Content, _item.VatTu));
        if (optionVatTu == null) { optionVatTu = new ComboBoxItem { Content = _item.VatTu }; form.cboVatTu.Items.Add(optionVatTu); }
        form.cboVatTu.SelectedItem = optionVatTu;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }

    private void btnTonKhoXuatKho_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("KhoView", "btnTonKhoXuatKho")) return;
        TonKhoXuatKhoWindow form = new TonKhoXuatKhoWindow();
        form.Owner = this;
        form.DataContext = _item;
        form.Title += $" — {_item.MaVatTu}";
        var optionKho = form.cboKho.Items.OfType<ComboBoxItem>().FirstOrDefault(i => Equals(i.Content, _item.Kho));
        if (optionKho == null) { optionKho = new ComboBoxItem { Content = _item.Kho }; form.cboKho.Items.Add(optionKho); }
        form.cboKho.SelectedItem = optionKho;
        var optionVatTu = form.cboVatTu.Items.OfType<ComboBoxItem>().FirstOrDefault(i => Equals(i.Content, _item.VatTu));
        if (optionVatTu == null) { optionVatTu = new ComboBoxItem { Content = _item.VatTu }; form.cboVatTu.Items.Add(optionVatTu); }
        form.cboVatTu.SelectedItem = optionVatTu;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }

    private void btnTonKhoDieuChinhTon_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("KhoView", "btnTonKhoDieuChinhTon")) return;
        TonKhoForm form = new TonKhoForm();
        form.Owner = this;
        form.DataContext = _item;
        form.Title += $" — {_item.MaVatTu}";
        var optionKho = form.cboKho.Items.OfType<ComboBoxItem>().FirstOrDefault(i => Equals(i.Content, _item.Kho));
        if (optionKho == null) { optionKho = new ComboBoxItem { Content = _item.Kho }; form.cboKho.Items.Add(optionKho); }
        form.cboKho.SelectedItem = optionKho;
        var optionVatTu = form.cboVatTu.Items.OfType<ComboBoxItem>().FirstOrDefault(i => Equals(i.Content, _item.VatTu));
        if (optionVatTu == null) { optionVatTu = new ComboBoxItem { Content = _item.VatTu }; form.cboVatTu.Items.Add(optionVatTu); }
        form.cboVatTu.SelectedItem = optionVatTu;
        form.txtTonKiemKe.Text = _item.Ton;
        // shortcut: giữ form nghiệp vụ demo có sẵn, nối tầng lưu trữ trước khi ghi giao dịch thật.
        form.ShowDialog();
    }
}
