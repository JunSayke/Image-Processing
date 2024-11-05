using ImageProcess2;
using WebCamLib;

namespace Image_Processing
{
    public partial class Form1 : Form
    {
        Bitmap loaded, processed;
        Device[] mgaDevice;
        FILTER currentFilter;

        public enum FILTER
        {
            None,
            PixelCopy,
            GrayScale,
            Inversion,
            Histogram,
            Brightness,
            Contrast,
            MirrorHorizontal,
            MirrorVertical,
            Rotate,
            Scale,
            Binary,
            Sepia
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mgaDevice = DeviceManager.GetAllDevices();
            currentFilter = FILTER.None;
        }

        private void openFileDialog1_FileOk_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            loaded = new Bitmap(openFileDialog1.FileName);
            pictureBox1.Image = loaded;
            if (webcamLoop.Enabled)
            {
                mgaDevice[0].Stop();
                webcamLoop.Enabled = false;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void pixelCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentFilter = FILTER.PixelCopy;
            ApplyFilter();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            processed.Save(saveFileDialog1.FileName);
        }

        private void scaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentFilter = FILTER.Scale;
            ApplyFilter();
        }

        private void grayScaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentFilter = FILTER.GrayScale;
            ApplyFilter();
        }

        private void inversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentFilter = FILTER.Inversion;
            ApplyFilter();
        }

        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentFilter = FILTER.Histogram;
            ApplyFilter();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentFilter = FILTER.Brightness;
            ApplyFilter();
        }

        private void contrastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentFilter = FILTER.Contrast;
            ApplyFilter();
        }

        private void mirrorHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentFilter = FILTER.MirrorHorizontal;
            ApplyFilter();
        }

        private void mirronVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentFilter = FILTER.MirrorVertical;
            ApplyFilter();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            currentFilter = FILTER.Rotate;
            ApplyFilter();
        }

        private void binaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentFilter = FILTER.Binary;
            ApplyFilter();
        }

        private void sepiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentFilter = FILTER.Sepia;
            ApplyFilter();
        }

        private void subtractionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form2 = new Form2();
            form2.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            IDataObject data;
            Image bmap;

            mgaDevice[0].Sendmessage();
            data = Clipboard.GetDataObject();

            bmap = (Image)(data.GetData("System.Drawing.Bitmap", true));

            if (bmap != null)
            {
                Bitmap b = new Bitmap(bmap);
                loaded = b;
                ApplyFilter();
            }
        }

        private void ApplyFilter()
        {
            if (loaded == null) {
                MessageBox.Show("No image loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            switch (currentFilter)
            {
                case FILTER.PixelCopy:
                    BasicDIP.PixelCopy(ref loaded, ref processed);
                    break;
                case FILTER.GrayScale:
                    processed = (Bitmap)loaded.Clone();
                    BitmapFilter.GrayScale(processed);
                    break;
                case FILTER.Inversion:
                    BasicDIP.Inversion(ref loaded, ref processed);
                    break;
                case FILTER.Histogram:
                    BasicDIP.Hist(ref loaded, ref processed);
                    break;
                case FILTER.Brightness:
                    BasicDIP.Brightness(ref loaded, ref processed, trackBar1.Value);
                    break;
                case FILTER.Contrast:
                    BasicDIP.Equalisation(ref loaded, ref processed, trackBar2.Value / 100);
                    break;
                case FILTER.MirrorHorizontal:
                    BasicDIP.MirrorHorizontal(ref loaded, ref processed);
                    break;
                case FILTER.MirrorVertical:
                    BasicDIP.MirrorVertical(ref loaded, ref processed);
                    break;
                case FILTER.Rotate:
                    BasicDIP.Rotate(ref loaded, ref processed, trackBar3.Value);
                    break;
                case FILTER.Scale:
                    BasicDIP.Scale(ref loaded, ref processed, 1000, 1000);
                    break;
                case FILTER.Binary:
                    BasicDIP.Binary(ref loaded, ref processed);
                    break;
                case FILTER.Sepia:
                    BasicDIP.Sepia(ref loaded, ref processed);
                    break;
                default:
                    break;
            }

            pictureBox2.Image = processed;
        }

        private void onToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            mgaDevice[0].ShowWindow(pictureBox1);
            webcamLoop.Enabled = true;
        }

        private void offToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            mgaDevice[0].Stop();
            webcamLoop.Enabled = false;
        }
    }
}
