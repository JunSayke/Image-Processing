using HNUDIP;
using ImageProcess2;
using WebCamLib;

namespace Image_Processing
{
    public partial class Form1 : Form
    {
        Bitmap loaded, processed, previous;
        Device[] mgaDevice;
        FILTER currentFilter;
        bool stackable = false;

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
            Sepia,
            Smoothing,
            Sharpening,
            EdgeEnhance,
            EdgeDetect,
            EmbossLaplacian,
            MeanRemoval,
            GaussianBlur,
            EmbossHorzVert,
            EmbossAllDir,
            EmbossLossy,
            EmbossHorz,
            EmbossVert,
            Quadrant,
            EdgeDetectSobel,
            EdgeDetectPrewitt,
            EdgeDetectKirsh,
            EdgeDetectQuick,
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mgaDevice = DeviceManager.GetAllDevices();
        }

        private void openFileDialog1_FileOk_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            loaded = new Bitmap(openFileDialog1.FileName);
            processed = (Bitmap)loaded.Clone();
            previous = (Bitmap)processed.Clone();
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

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            processed.Save(saveFileDialog1.FileName);
        }

        private void subtractionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form2 = new Form2();
            form2.Show();
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

        private void webcamLoop_Tick(object sender, EventArgs e)
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
                ApplyFilter(currentFilter);
            }
        }

        private void ApplyFilter(FILTER filter)
        {
            if (loaded == null)
            {
                MessageBox.Show("No image loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            switch (filter)
            {
                case FILTER.PixelCopy:
                    BasicDIP.PixelCopy(processed);
                    break;
                case FILTER.GrayScale:
                    BitmapFilter.GrayScale(processed);
                    break;
                case FILTER.Inversion:
                    BasicDIP.Inversion(processed);
                    break;
                case FILTER.Histogram:
                    BasicDIP.Hist(ref processed);
                    break;
                case FILTER.Brightness:
                    BasicDIP.Brightness(processed, trackBar1.Value);
                    break;
                case FILTER.Contrast:
                    BasicDIP.Equalisation(processed, trackBar2.Value / 100);
                    break;
                case FILTER.MirrorHorizontal:
                    BasicDIP.MirrorHorizontal(processed);
                    break;
                case FILTER.MirrorVertical:
                    BasicDIP.MirrorVertical(processed);
                    break;
                case FILTER.Rotate:
                    BasicDIP.Rotate(ref processed, trackBar3.Value);
                    break;
                case FILTER.Scale:
                    BasicDIP.Scale(ref processed, (int)weightInpt.Value, (int)heightInpt.Value);
                    break;
                case FILTER.Binary:
                    BasicDIP.Binary(processed);
                    break;
                case FILTER.Sepia:
                    BasicDIP.Sepia(processed);
                    break;
                case FILTER.Smoothing:
                    BitmapFilter.Smooth(processed, 1);
                    break;
                case FILTER.Sharpening:
                    BitmapFilter.Sharpen(processed, 1);
                    break;
                case FILTER.EdgeEnhance:
                    BasicDIP.EdgeEnhance(processed);
                    break;
                case FILTER.EdgeDetect:
                    BasicDIP.EdgeDetect(processed);
                    break;
                case FILTER.EmbossLaplacian:
                    BitmapFilter.EmbossLaplacian(processed);
                    break;
                case FILTER.MeanRemoval:
                    BitmapFilter.MeanRemoval(processed, 9);
                    break;
                case FILTER.GaussianBlur:
                    BitmapFilter.GaussianBlur(processed, 5);
                    break;
                case FILTER.EmbossHorzVert:
                    BasicDIP.Emboss(processed, BasicDIP.EMBOSS.HORIZONTAL_VERTICAL);
                    break;
                case FILTER.EmbossAllDir:
                    BasicDIP.Emboss(processed, BasicDIP.EMBOSS.ALL_DIRECTION);
                    break;
                case FILTER.EmbossLossy:
                    BasicDIP.Emboss(processed, BasicDIP.EMBOSS.LOSSY);
                    break;
                case FILTER.EmbossHorz:
                    BasicDIP.Emboss(processed, BasicDIP.EMBOSS.HORIZONTAL_ONLY);
                    break;
                case FILTER.EmbossVert:
                    BasicDIP.Emboss(processed, BasicDIP.EMBOSS.VERTICAL_ONLY);
                    break;
                case FILTER.Quadrant:
                    quadrantFilter(processed);
                    break;
                case FILTER.EdgeDetectSobel:
                    BitmapFilter.EdgeDetectConvolution(processed, BitmapFilter.EDGE_DETECT_SOBEL, 5);
                    break;
                case FILTER.EdgeDetectPrewitt:
                    BitmapFilter.EdgeDetectConvolution(processed, BitmapFilter.EDGE_DETECT_PREWITT, 5);
                    break;
                case FILTER.EdgeDetectKirsh:
                    BitmapFilter.EdgeDetectConvolution(processed, BitmapFilter.EDGE_DETECT_KIRSH, 5);
                    break;
                case FILTER.EdgeDetectQuick:
                    BitmapFilter.EdgeDetectQuick(processed);
                    break;
                default:
                    break;
            }

            pictureBox2.Image = processed;
            currentFilter = filter;

            if (!stackable)
            {
                processed = (Bitmap)previous.Clone();
            } else
            {
                previous = (Bitmap)processed.Clone();
            }
        }

        private void scaleBtn_Click(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.Scale);
        }

        private void quadrantFilter(Bitmap a)
        {
            Bitmap b = new Bitmap(a.Width, a.Height);
            int width = a.Width;
            int height = a.Height;
            int halfWidth = width / 2;
            int halfHeight = height / 2;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Inversion
                    if (x >= halfWidth && y < halfHeight)
                    {
                        Color pixel = a.GetPixel(x, y);
                        b.SetPixel(x, y, Color.FromArgb(255 - pixel.R, 255 - pixel.G, 255 - pixel.B));
                    }
                    // GrayScale
                    else if (x < halfWidth && y < halfHeight)
                    {
                        Color pixel = a.GetPixel(x, y);
                        int gray = (pixel.R + pixel.G + pixel.B) / 3;
                        b.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                    }
                    // PixelCopy
                    else if (x < halfWidth && y >= halfHeight)
                    {
                        Color pixel = a.GetPixel(x, y);
                        b.SetPixel(x, y, pixel);
                    }
                    // MirrorVertical
                    else if (x >= halfWidth && y >= halfHeight)
                    {
                        Color pixel = a.GetPixel(x, height - y - 1);
                        b.SetPixel(x, y, pixel);
                    }
                }
            }

            processed = b;
        }

        private void pixelCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.PixelCopy);
        }

        private void grayScaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.GrayScale);
        }

        private void inversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.Inversion);
        }

        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.Histogram);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.Brightness);
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.Contrast);
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.Rotate);
        }

        private void mirrorHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.MirrorHorizontal);
        }

        private void mirronVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.MirrorVertical);
        }

        private void binaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.Binary);
        }

        private void sepiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.Sepia);
        }

        private void smoothToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.Smoothing);
        }

        private void sharpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.Sharpening);
        }

        private void meanRemovalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.MeanRemoval);
        }

        private void gaussianBlurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.GaussianBlur);
        }

        private void laplascianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.EmbossLaplacian);
        }

        private void horizontalVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.EmbossHorzVert);
        }

        private void allDirectionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.EmbossAllDir);
        }

        private void lossyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.EmbossLossy);
        }

        private void horizontalOnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.EmbossHorz);
        }

        private void verticalOnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.EmbossVert);
        }

        private void enhanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.EdgeEnhance);
        }

        private void detectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.EdgeDetect);
        }

        private void quadrantToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.Quadrant);
        }

        private void detectSobelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.EdgeDetectSobel);
        }

        private void detectPrewittToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.EdgeDetectPrewitt);
        }

        private void detectKirshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.EdgeDetectKirsh);
        }

        private void quickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFilter(FILTER.EdgeDetectQuick);
        }

        private void UpdateStackableStatus()
        {
            stackableStatusLabel.Text = stackable ? "Stackable: ON" : "Stackable: OFF";
        }

        private void onToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (!stackable)
            {
                stackable = true;
                UpdateStackableStatus();
            }
        }

        private void offToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (stackable)
            {
                stackable = false;
                previous = (Bitmap)processed.Clone();
                UpdateStackableStatus();
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = (Bitmap)loaded.Clone();
            previous = (Bitmap)processed.Clone();
            ApplyFilter(FILTER.PixelCopy);
        }

        private void coinDetectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form3 = new Form3();
            form3.Show();
        }
    }
}



