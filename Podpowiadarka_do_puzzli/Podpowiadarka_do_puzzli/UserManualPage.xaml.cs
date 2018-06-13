using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Podpowiadarka_do_puzzli
{
    /// <summary>
    /// Logika interakcji dla klasy UserManualPage.xaml
    /// </summary>
    public partial class UserManualPage : Page
    {
        public UserManualPage()
        {
            InitializeComponent();

            if (File.Exists(MainWindow.PATH)) checkBoxSkip.IsChecked = true;
            else checkBoxSkip.IsChecked = false;

        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoadingPhotosPage());
        }

        private void skipChange(object sender, RoutedEventArgs e)
        {
            if (checkBoxSkip.IsChecked == false) File.Delete(MainWindow.PATH);
            else File.Create(MainWindow.PATH);
        }

        private void skip(object sender, RoutedEventArgs e)
        {

        }
    }
}
