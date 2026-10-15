using System.Windows;

namespace QuanLyKhachSan.Views.Dialogs
{
    public partial class XacNhanDemoWindow : Window
    {
        public XacNhanDemoWindow()
        {
            InitializeComponent();
        }

        private void btnDong_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnLuu_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
