using System.Windows;

namespace QuanLyKhachSan.Views.Dialogs
{
    public partial class KhoForm : Window
    {
        public KhoForm()
        {
            InitializeComponent();
        }

        private void btnDong_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnLuu_Click(object sender, RoutedEventArgs e)
        {

            Close();
        }
    }
}
