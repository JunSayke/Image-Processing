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

        private void button1_Click(object sender, EventArgs e)
        {
            if (loaded == null)
            {
                MessageBox.Show("Please load an image first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Step 1: Define the coin types
            var denominationDatas = new List<CoinCounter.DenominationData>
            {
                new CoinCounter.DenominationData { Radius = 80, Value = 5.00m, Threshold = 150, SuppressionRadius = 20 },
                new CoinCounter.DenominationData { Radius = 70, Value = 1.00m, Threshold = 150, SuppressionRadius = 15 },
                new CoinCounter.DenominationData { Radius = 60, Value = 0.25m, Threshold = 150, SuppressionRadius = 10 },
                new CoinCounter.DenominationData { Radius = 50, Value = 0.10m, Threshold = 180, SuppressionRadius = 5 },
                new CoinCounter.DenominationData { Radius = 45, Value = 0.05m, Threshold = 180, SuppressionRadius = 5 }
                // Add more denominations as needed
            };

            // Step 2: Create an instance of CoinCounter
            var coinCounter = new CoinCounter(denominationDatas);

            processed = coinCounter.PreprocessImage(loaded);

            // Step 4: Count the coins
            coinCounter.CountCoins(processed, visualize: true);

            // Step 5: Access the results
            decimal totalValue = coinCounter.TotalValue;
            var detectedCoins = coinCounter.DetectedCoins;
            var accumulators = coinCounter.Accumulators;

            // Draw the detected circles and their values on the image
            PenGraphic pen = new PenGraphic();
            foreach (var (key, centers) in detectedCoins)
            {
                foreach (var center in centers)
                {
                    using (Graphics g = Graphics.FromImage(processed))
                    {
                        pen.Draw(g, center, key.ToString("C"));
                    }
                }
            }

            // Show the image with circles drawn
            pictureBox1.Image = processed;

            // Build the message string
            StringBuilder message = new StringBuilder();
            int totalCount = 0;
            foreach (var (key, centers) in detectedCoins)
            {
                int count = centers.Count();
                message.AppendLine($"Denomination: {key:C}, Count: {count}");
                totalCount += count;
            }
            message.AppendLine($"Total Count: {totalCount}");
            message.AppendLine($"Total Value: {totalValue:C}");

            // Show the message box
            MessageBox.Show(message.ToString(), "Coin Count Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
