using ImageProcess2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HoughCoinCounter;

namespace Image_Processing
{
    public partial class Form3 : Form
    {
        Bitmap loaded, processed;

        public Form3()
        {
            InitializeComponent();
        }

        // HACK: Cleanup the code pag may time
        private void button1_Click(object sender, EventArgs e)
        {
            if (loaded == null)
            {
                MessageBox.Show("Please load an image first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            processed = (Bitmap)loaded.Clone();

            // Preprocess the image
            BitmapFilter.GrayScale(processed);
            BitmapFilter.GaussianBlur(processed, 5);
            BasicDIP.Binary(processed);
            BitmapFilter.EdgeDetectConvolution(processed, BitmapFilter.EDGE_DETECT_SOBEL, 5);

            // Define the coin types with their respective radii and values
            List<CoinType> coinTypes = new List<CoinType>
            {
                // CoinType(radius, value, threshold)
                new CoinType(80, 5.00m, 150), // 5 Peso coin
                new CoinType(70, 1.00m, 150), // 1 Peso coin
                new CoinType(60, 0.25m, 150), // 25 Cent coin
                new CoinType(50, 0.10m, 180), // 10 Cent coin
                new CoinType(45, 0.05m, 180)  // 5 Cent coin
            };

            // Initialize the CoinCounter with the defined coin types
            CoinCounter coinCounter = new CoinCounter(coinTypes);

            // Count the total value of the coins in the processed image
            int suppressionRadius = 10; // Adjust as needed
            var (totalValue, detectedCoins, coinCounts) = coinCounter.CountTotalValue(processed, suppressionRadius);

            // Display the total value of the coins
            MessageBox.Show($"Total value of coins: {totalValue:C}", "Coin Counter");

            // Display the count for each denomination
            StringBuilder countsMessage = new StringBuilder("Coin counts:\n");
            int totalCoins = 0;
            foreach (var coinCount in coinCounts)
            {
                countsMessage.AppendLine($"{coinCount.Key:C}: {coinCount.Value}");
                totalCoins += coinCount.Value;
            }
            countsMessage.AppendLine($"\nTotal coins found: {totalCoins}");
            MessageBox.Show(countsMessage.ToString(), "Coin Counts");

            // Draw the detected circles and their values on the image
            PenGraphic pen = new PenGraphic();
            foreach (var (center, value) in detectedCoins)
            {
                using (Graphics g = Graphics.FromImage(processed))
                {
                    pen.Draw(g, center, value.ToString("C"));
                }
            }

            // Show the image with circles drawn
            pictureBox1.Image = processed;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            loaded = new Bitmap(openFileDialog1.FileName);
            pictureBox1.Image = loaded;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            processed.Save(saveFileDialog1.FileName);
        }
    }
}
