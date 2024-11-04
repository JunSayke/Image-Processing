using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Image_Processing
{
    public partial class Form2 : Form
    {
        Bitmap imageA, imageB;
        public Form2()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (imageA.Width != imageB.Width || imageA.Height != imageB.Height)
            {
                DialogResult result = MessageBox.Show("Images must have the same dimensions. Do you want to resize them?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    return;
                }

                int maxwidth = Math.Max(imageA.Width, imageB.Width);
                int maxheight = Math.Max(imageA.Height, imageB.Height);
                Bitmap newImageA = new Bitmap(maxwidth, maxheight);
                Bitmap newImageB = new Bitmap(maxwidth, maxheight);
                BasicDIP.Scale(ref imageA, ref newImageA, maxwidth, maxheight);
                BasicDIP.Scale(ref imageB, ref newImageB, maxwidth, maxheight);
                imageA = newImageA;
                imageB = newImageB;
            }

            Color mygreen = Color.FromArgb(0, 255, 0);
            int greygreen = (mygreen.R + mygreen.G + mygreen.B) / 3;
            int threshold = 5;

            Bitmap resultImage = new Bitmap(imageB.Width, imageB.Height);

            for (int x = 0; x < imageB.Width; x++) 
                for (int y = 0; y < imageB.Height; y++)
                {
                    Color pixel = imageA.GetPixel(x, y);
                    Color backpixel = imageB.GetPixel(x, y);
                    int grey = (pixel.R + pixel.G + pixel.B) / 3;
                    int subtractvalue = Math.Abs(grey - greygreen);
                    if (subtractvalue < threshold)
                        resultImage.SetPixel(x, y, backpixel);
                    else
                        resultImage.SetPixel(x, y, pixel);

                }
            pictureBox3.Image = resultImage;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            imageA = new Bitmap(openFileDialog1.FileName);
            pictureBox1.Image = imageA;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            imageB = new Bitmap(openFileDialog2.FileName);
            pictureBox2.Image = imageB;
        }
    }
}
