namespace Image_Processing
{
    public partial class Form1 : Form
    {
        Bitmap loaded, processed;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            loaded = new Bitmap(openFileDialog1.FileName);
            pictureBox1.Image = loaded;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void pixelCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    processed.SetPixel(x, y, pixel);
                }
            }
            pictureBox2.Image = processed;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            processed.Save(saveFileDialog1.FileName);
        }

        private void grayScaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            int ave;
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    ave = (int)(pixel.R + pixel.G + pixel.B) / 3;
                    Color gray = Color.FromArgb(ave, ave, ave);
                    processed.SetPixel(x, y, gray);
                }
            }
            pictureBox2.Image = processed;
        }

        private void inversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    Color invert = Color.FromArgb(255 - pixel.R, 255 - pixel.G, 255 - pixel.B);
                    processed.SetPixel(x, y, invert);
                }
            }
            pictureBox2.Image = processed;
        }

        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BasicDIP.Hist(ref loaded, ref processed);
            pictureBox2.Image = processed;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            BasicDIP.Brightness(ref loaded, ref processed, trackBar1.Value);
            pictureBox2.Image = processed;
        }

        private void contrastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BasicDIP.Equalisation(ref loaded, ref processed, trackBar2.Value / 100);
            pictureBox2.Image = processed;
        }

        private void mirrorHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    processed.SetPixel(loaded.Width - 1 - x, y, pixel);
                }
            }
            pictureBox2.Image = processed;
        }

        private void mirronVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    processed.SetPixel(x, loaded.Height - 1 - y, pixel);
                }
            }
            pictureBox2.Image = processed;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            BasicDIP.Rotate(ref loaded, ref processed, trackBar3.Value);
            pictureBox2.Image = processed;
        }

        private void scaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BasicDIP.Scale(ref loaded, ref processed, 1000, 1000);
            pictureBox2.Image = processed;
        }

        private void binaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            int ave;
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    ave = (int)(pixel.R + pixel.G + pixel.B) / 3;
                    if (ave < 180)
                    {
                        processed.SetPixel(x, y, Color.Black);
                    }
                    else
                    {
                        processed.SetPixel(x, y, Color.White);
                    }
                }
            }
            pictureBox2.Image = processed;
        }
    }
}
