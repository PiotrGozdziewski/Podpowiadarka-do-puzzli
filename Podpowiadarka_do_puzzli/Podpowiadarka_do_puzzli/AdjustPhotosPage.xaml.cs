using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Podpowiadarka_do_puzzli
{
    /// <summary>
    /// Logika interakcji dla klasy AdjustPhotosPage.xaml
    /// </summary>
    public partial class AdjustPhotosPage : Page
    {
        static public Image<Bgr, byte> imgIn = LoadingPhotosPage.imgInput;
        public Image<Bgr, byte> imgInPuzzle = LoadingPhotosPage.imgInputPuzzle;
        List<System.Drawing.Point> points = new List<System.Drawing.Point>();
        static internal System.Windows.Forms.ImageList ImageList = new ImageList();
        static internal ObservableCollection<Bitmap> lista_zdjec = new ObservableCollection<Bitmap>();

        Emgu.CV.Util.VectorOfVectorOfPoint contours;
        Emgu.CV.Util.VectorOfVectorOfPoint con;


        public AdjustPhotosPage()
        {
            try
            {
                InitializeComponent();
                zdj.Source = ToBitmapSource(imgInPuzzle);
                t1.Value = 100;
                t2.Value = 30;
            }
            catch(System.NullReferenceException){ }
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new LoadingPhotosPage());
        }

        public void wykrywanie_konturow(int t1, int t2)
        {
            Thread.Sleep(50);
            try
            {
                Image<Gray, byte> img = imgInPuzzle.Convert<Gray, byte>();
                Image<Gray, byte> img1 = img;

                Mat graymat = new Mat();
                Mat mask = new Mat();

                graymat = img1.Mat;

                CvInvoke.AdaptiveThreshold(graymat, mask, 255, AdaptiveThresholdType.MeanC, ThresholdType.BinaryInv, t1 * 10 - 1, t2);

                contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
                con = new Emgu.CV.Util.VectorOfVectorOfPoint();

                Mat hier = new Mat(mask.Rows, mask.Cols, DepthType.Cv8U, 0);

                CvInvoke.FindContours(mask, contours, hier, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);

                for (int i = 0; i < contours.Size; i++)
                {
                    double a = CvInvoke.ContourArea(contours[i], false);
                    if (a > 600 && a < ((mask.Cols * mask.Rows) / 2))
                        con.Push(contours[i]);
                }

                CvInvoke.DrawContours(mask, con, -1, new MCvScalar(255, 255, 255), 1);

                ilosc_konturow.Text = "Liczba konturów: " + con.Size.ToString();

                zdj.Source = ToBitmapSource(mask);
            }
            catch(System.NullReferenceException)
            {
                System.Windows.MessageBox.Show("Wczytaj zdjęcia", "Podpowiadarka do puzzli - Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }          
        }

        private void t1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (t2 == null) return;
            int a = Convert.ToInt32(t1.Value);
            int b = Convert.ToInt32(t2.Value);

            wykrywanie_konturow(a, b);
        }

        private void t2_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int x = Convert.ToInt32(t1.Value);
            int y = Convert.ToInt32(t2.Value);

            wykrywanie_konturow(x, y);
        }

        public void Crop2(Bitmap bm, int cropX, int cropY, int cropWidth, int cropHeight, string nazwa, int numer_zdjecia)
        {
            var rect = new System.Drawing.Rectangle(cropX, cropY, cropWidth, cropHeight);
            Bitmap bitmap = bm.Clone(rect, bm.PixelFormat);

            nazwa += numer_zdjecia.ToString();
            nazwa += ".png";

            ImageList.Images.Add(bitmap);

            lista_zdjec.Add(bitmap); 
        }

        void wyodrebnienie_puzzli(VectorOfVectorOfPoint con, List<System.Drawing.Point> p, Bitmap bmp)
        {
            for (int i = 0; i < con.Size; i++)
            {
                var array = con[i].ToArray();
                p.AddRange(array);

                var min_x = array.Min(a => a.X);
                var min_y = array.Min(a => a.Y);
                var max_x = array.Max(a => a.X);
                var max_y = array.Max(a => a.Y);

                Crop2(bmp, min_x, min_y, max_x - min_x, max_y - min_y, "zdjecie", i + 1);
            }
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            wyodrebnienie_puzzli(con, points, imgInPuzzle.ToBitmap());
            this.NavigationService.Navigate(new DetectPuzzlePage());
        }

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (System.Drawing.Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap();

                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr);
                return bs;
            }
        }
    }
}
