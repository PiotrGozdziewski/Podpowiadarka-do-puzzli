using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
    /// Logika interakcji dla klasy LoadingPhotosPage.xaml
    /// </summary>
    public partial class LoadingPhotosPage : Page
    {
        public LoadingPhotosPage()
        {
            InitializeComponent();
        }

        public static Image<Rgb, byte> imgInput;
        public static Image<Rgb, byte> imgInputPuzzle;

        public void add_Image(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                image.Source = new BitmapImage(new Uri(op.FileName));
                var path = op.FileName;
                imgInput = new Image<Rgb, byte>(path);
            }
        }


        public void add_Puzzle(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                puzzle.Source = new BitmapImage(new Uri(op.FileName));
                var path = op.FileName;
                imgInputPuzzle = new Image<Rgb, byte>(path);
            }
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new AdjustPhotosPage());
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new UserManualPage());
        }
    }
}
