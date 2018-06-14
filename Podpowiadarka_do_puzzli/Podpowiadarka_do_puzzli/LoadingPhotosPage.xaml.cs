using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
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

            if (imgInputPuzzle != null)
            {
                image.Source = bitmapImg;
            }

            if(imgInputPuzzle != null)
            {
                puzzle.Source = bitmapPuz;
            }
        }

        public static BitmapImage bitmapImg; 
        public static BitmapImage bitmapPuz; 
        public static Image<Bgr, byte> imgInput;
        public static Image<Bgr, byte> imgInputPuzzle;
        static internal System.Windows.Forms.ImageList ImL;
        static public BitmapImage bmp;

        public void add_Image(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                bitmapImg = new BitmapImage(new Uri(op.FileName));
                image.Source = bitmapImg;
                var path = op.FileName;
                Bitmap bm = new Bitmap(path);
                System.Drawing.Image img = bm;
                ImL = new System.Windows.Forms.ImageList();
                ImL.Images.Add(img);
                bmp = new BitmapImage(new Uri(path));
                Image<Bgr, byte> imgInput1 = new Image<Bgr, byte>(path);
                if (imgInput1.Width > 1024 && imgInput1.Width < 2050)
                {
                    imgInput = imgInput1.Resize(imgInput1.Width/2, imgInput1.Height/2, Emgu.CV.CvEnum.Inter.Linear);
                }
                else if(imgInput1.Width>2050)
                {
                    imgInput = imgInput1.Resize(imgInput1.Width / 3, imgInput1.Height / 3, Emgu.CV.CvEnum.Inter.Linear);
                }
                else
                {
                    imgInput = imgInput1;
                }                  
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
                bitmapPuz = new BitmapImage(new Uri(op.FileName));
                puzzle.Source = bitmapPuz;
                var path = op.FileName;
                Image<Bgr, byte> imgInputPuzzle1 = new Image<Bgr, byte>(path);
                if(imgInputPuzzle1.Width>1024&& imgInputPuzzle1.Width<2050)
                {
                    imgInputPuzzle = imgInputPuzzle1.Resize(imgInputPuzzle1.Width / 2, imgInputPuzzle1.Height / 2, Emgu.CV.CvEnum.Inter.Linear);
                }
                else if(imgInputPuzzle1.Width>2050)
                {
                    imgInputPuzzle = imgInputPuzzle1.Resize(imgInputPuzzle1.Width / 3, imgInputPuzzle1.Height / 3, Emgu.CV.CvEnum.Inter.Linear);
                }
                else
                {
                    imgInputPuzzle = imgInputPuzzle1;
                }
            }
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            if (imgInput != null && imgInputPuzzle != null)
            {
                this.NavigationService.Navigate(new AdjustPhotosPage());
            }
            else
            {
                string error_message = "";

                if (imgInput == null && imgInputPuzzle == null) error_message = "zdjęcie obrazka oraz zdjęcie puzzli";
                else if (imgInput == null) error_message = "zdjęcie obrazka";
                else if (imgInputPuzzle == null) error_message = "zdjęcie puzzli";

                MessageBox.Show("Dodaj "+error_message+".", "Błąd");
            }
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new UserManualPage());
        }
    }
}