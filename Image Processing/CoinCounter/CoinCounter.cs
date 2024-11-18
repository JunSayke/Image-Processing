using Image_Processing;
using ImageProcess2;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HoughCoinCounter
{
    internal class CoinCounter
    {
        private readonly List<DenominationData> denominationDatas;

        internal class DenominationData
        {
            public int Radius { set; get; }
            public decimal Value { set; get; }
            public int Threshold { set; get; }

            public int SuppressionRadius { set; get; }
        }

        private decimal totalValue = 0;
        private Dictionary<decimal, List<Point>> detectedCoins = new Dictionary<decimal, List<Point>>();
        private Dictionary<decimal, int[,]> accumulators = new Dictionary<decimal, int[,]>();
        public decimal TotalValue { get => totalValue; }
        public Dictionary<decimal, List<Point>> DetectedCoins { get => detectedCoins; }
        public Dictionary<decimal, int[,]> Accumulators { get => accumulators; }


        public CoinCounter(List<DenominationData> denominationDatas)
        {
            this.denominationDatas = denominationDatas;
        }

        private void clear()
        {
            this.totalValue = 0;
            this.detectedCoins.Clear();
            this.accumulators.Clear();
        }

        public void CountCoins(Bitmap image, bool visualize = false) 
        {
            clear();


            foreach(var denominationData in this.denominationDatas)
            {
                int[,] accumulator = HoughCircle.GetAccumulator(image, denominationData.Radius);
                this.accumulators[denominationData.Value] = accumulator;
                List<Point> centers = HoughCircle.SearchCircles(accumulator, denominationData.Threshold, denominationData.SuppressionRadius);
                this.totalValue += centers.Count * denominationData.Value;
                this.detectedCoins[denominationData.Value] = centers;
            }

            if (visualize)
            {
                foreach (var accumulator in this.accumulators)
                {
                    ShowVisualizations(accumulator.Value, accumulator.Key.ToString(), new Size(800, 800));
                }
            }
        }

        public Bitmap PreprocessImage(Bitmap image)
        {
            Bitmap temp = (Bitmap)image.Clone();
            BitmapFilter.GrayScale(temp);
            BitmapFilter.GaussianBlur(temp, 5);
            BasicDIP.Binary(temp);
            BitmapFilter.EdgeDetectConvolution(temp, BitmapFilter.EDGE_DETECT_SOBEL, 5);
            return temp;
        }

        public void ShowVisualizations(int[,] accumulator, string label, Size size)
        {
            Bitmap accumulatorBitmap = HoughCircle.VisualizeAccumulator(accumulator);
            Form accumulatorForm = new Form
            {
                Text = $"Accumulator Visualization for {label}",
                ClientSize = new Size(size.Width + 200, size.Height + 200)
            };
            PictureBox accumulatorPictureBox = new PictureBox
            {
                Image = accumulatorBitmap,
                Size = size,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Dock = DockStyle.Fill
            };
            accumulatorForm.Controls.Add(accumulatorPictureBox);
            accumulatorForm.Show();
        }
    }
}