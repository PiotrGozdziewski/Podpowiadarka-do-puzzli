using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
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
    /// Logika interakcji dla klasy DetectPuzzlePage.xaml
    /// </summary>
    public partial class DetectPuzzlePage : Page
    {
        Image<Bgr, byte> imgIn = AdjustPhotosPage.imgIn;
        ObservableCollection<Bitmap> lista_zdjec = AdjustPhotosPage.lista_zdjec;

        public DetectPuzzlePage()
        {
            InitializeComponent();
            System.Windows.Forms.ImageList ImageList = AdjustPhotosPage.ImageList;
            
            int x=0;
            for (int i = 0; i < lista_zdjec.Count; i++)
            {
                x = i + 1;
                listViewPuzzle.Items.Add(new Images { Title = "puzzel " + x, ImageData = BitmapToImageSource(lista_zdjec[i]) });
            }
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            for(int i=0;i<listViewPuzzle.Items.Count;i++)
            {
                listViewPuzzle.Items.RemoveAt(i);
                lista_zdjec.Clear();
            }
            this.NavigationService.Navigate(new AdjustPhotosPage());
        }
        //=> this.NavigationService.Navigate(new AdjustPhotosPage());

        private void exit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void getSelectedItem(object sender, MouseButtonEventArgs e)
        {
            Images md = (Images)listViewPuzzle.SelectedItems[0];
            BitmapImage mI = md.ImageData;
            Bitmap b = BitmapImage2Bitmap(mI);
            Image<Bgr, byte> img = new Image<Bgr, byte>(b);
            Mat mat = img.Mat;
        
            long matchTime;
            using (Mat modelImage = mat)
            using (Mat observedImage = imgIn.Mat)
            {
                Mat result = Detect.Draw(modelImage, observedImage, out matchTime);
                image.Source = BitmapToImageSource(result.Bitmap);
            }
        }

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }

 
    public class Images
    {
        private string _Title;
        public string Title
        {
            get { return this._Title; }
            set { this._Title = value; }
        }

        private BitmapImage _ImageData;
        public BitmapImage ImageData
        {
            get { return this._ImageData; }
            set { this._ImageData = value; }
        }
    }

    public static class Detect
    {
        public static void FindMatch(Mat modelImage, Mat observedImage, out long matchTime, out VectorOfKeyPoint modelKeyPoints, out VectorOfKeyPoint observedKeyPoints, VectorOfVectorOfDMatch matches, out Mat mask, out Mat homography)
        {
            int k = 2;
            double uniquenessThreshold = 0.8;

            Stopwatch watch;
            homography = null;

            modelKeyPoints = new VectorOfKeyPoint();
            observedKeyPoints = new VectorOfKeyPoint();

            using (UMat uModelImage = modelImage.ToUMat(AccessType.Read))
            using (UMat uObservedImage = observedImage.ToUMat(AccessType.Read))
            {
                SIFT surfCPU = new SIFT();

                //extract features from the object image
                UMat modelDescriptors = new UMat();
                surfCPU.DetectAndCompute(uModelImage, null, modelKeyPoints, modelDescriptors, false);

                watch = Stopwatch.StartNew();

                // extract features from the observed image
                UMat observedDescriptors = new UMat();
                surfCPU.DetectAndCompute(uObservedImage, null, observedKeyPoints, observedDescriptors, false);
                BFMatcher matcher = new BFMatcher(DistanceType.L2);
                matcher.Add(modelDescriptors);
                matcher.KnnMatch(observedDescriptors, matches, k, null);
                mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
                mask.SetTo(new MCvScalar(255));
                Features2DToolbox.VoteForUniqueness(matches, uniquenessThreshold, mask);

                int nonZeroCount = CvInvoke.CountNonZero(mask);
                if (nonZeroCount >= 4)
                {
                    nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints,
                       matches, mask, 1.5, 20);
                    if (nonZeroCount >= 4)
                        homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints,
                           observedKeyPoints, matches, mask, 2);
                }
                watch.Stop();
            }
            matchTime = watch.ElapsedMilliseconds;
        }

        public static Mat Draw(Mat modelImage, Mat observedImage, out long matchTime)
        {
            Mat homography;
            VectorOfKeyPoint modelKeyPoints;
            VectorOfKeyPoint observedKeyPoints;
            using (VectorOfVectorOfDMatch matches = new VectorOfVectorOfDMatch())
            {
                Mat mask;
                FindMatch(modelImage, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, matches,
                   out mask, out homography);

                //Draw the matched keypoints
                Mat result = new Mat();
                Features2DToolbox.DrawMatches(modelImage, modelKeyPoints, observedImage, observedKeyPoints,
                   matches, result, new MCvScalar(255,255,255,0), new MCvScalar(255, 255, 255,0), mask, Features2DToolbox.KeypointDrawType.NotDrawSinglePoints);

                if (homography != null)
                {
                    //draw a rectangle along the projected model
                    System.Drawing.Rectangle rect = new System.Drawing.Rectangle(System.Drawing.Point.Empty, modelImage.Size);
                    PointF[] pts = new PointF[]
                    {
                  new PointF(rect.Left, rect.Bottom),
                  new PointF(rect.Right, rect.Bottom),
                  new PointF(rect.Right, rect.Top),
                  new PointF(rect.Left, rect.Top)
                    };
                    pts = CvInvoke.PerspectiveTransform(pts, homography);

                    System.Drawing.Point[] points = Array.ConvertAll<PointF, System.Drawing.Point>(pts, System.Drawing.Point.Round);
                    using (VectorOfPoint vp = new VectorOfPoint(points))
                    {
                        CvInvoke.Polylines(result, vp, true, new MCvScalar(255, 0, 0, 255), 4);
                    }
                }
                return result;
            }
        }
    }
}
