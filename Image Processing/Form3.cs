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

namespace Image_Processing
{
    public partial class Form3 : Form
    {
        Bitmap loaded;

        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Initialize Hough Transform class
            Hough hough = new Hough();

            // Preprocess the image
            Console.WriteLine("Preprocessing the image...");
            BitmapFilter.GrayScale(loaded);
            BitmapFilter.GaussianBlur(loaded, 5);
            BasicDIP.Binary(loaded);
            BitmapFilter.EdgeDetectConvolution(loaded, BitmapFilter.EDGE_DETECT_SOBEL, 5);


            /* 
             * Define the radius range for 5 PHP coin (based on the coin's size relative to the image scale in pixels)
             * Upon testing 
             * 5 Peso = {
             *  radius: 80,
             *  threshold: 200
             * }
             * 1 Peso = {
             *  radius: 70,
             *  threshold: 150
             * }
             * 25 Cent = {
             *  radius: 60,
             *  threshold: 200
             * }
             * 10 Cent = {
             *  radius: 50,
             *  threshold: 200
             * }
             * 5 Cent = {
             *  radius: 45,
             *  threshold: 180
             * }
             */
            int radius = 45; // adjust as needed
            int threshold = 180; // Threshold for circle detection (adjust as needed, higher value means stricter)

            // Perform Hough Transform for circles with the current radius
            Bitmap houghImage = hough.TransformCircle(loaded, threshold, radius);
            // Get the detected circles
            List<Point> detectedCircles = hough.getDetectedCircles;

            Debug.WriteLine($"Detected {detectedCircles.Count} circles for radius {radius}");
            Debug.WriteLine($"Total detected circles: {detectedCircles.Count}");

            // Show the image with circles drawn
            pictureBox1.Image = houghImage;
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
    }
}
