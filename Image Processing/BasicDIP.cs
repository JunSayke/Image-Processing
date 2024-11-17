using ImageProcess2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Image_Processing
{
    class BasicDIP
    {
        public static void PixelCopy(Bitmap a)
        {
            for (int x = 0; x < a.Width; x++)
            {
                for (int y = 0; y < a.Height; y++)
                {
                    a.SetPixel(x, y, a.GetPixel(x, y));
                }
            }
        }

        public static void Inversion(Bitmap a)
        {
            for (int x = 0; x < a.Width; x++)
            {
                for (int y = 0; y < a.Height; y++)
                {
                    Color pixel = a.GetPixel(x, y);
                    a.SetPixel(x, y, Color.FromArgb(255 - pixel.R, 255 - pixel.G, 255 - pixel.B));
                }
            }
        }

        public static void MirrorVertical(Bitmap a)
        {
            int width = a.Width;
            int height = a.Height;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height / 2; y++)
                {
                    Color topPixel = a.GetPixel(x, y);
                    Color bottomPixel = a.GetPixel(x, height - 1 - y);

                    a.SetPixel(x, y, bottomPixel);
                    a.SetPixel(x, height - 1 - y, topPixel);
                }
            }
        }

        public static void MirrorHorizontal(Bitmap a)
        {
            int width = a.Width;
            int height = a.Height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width / 2; x++)
                {
                    Color leftPixel = a.GetPixel(x, y);
                    Color rightPixel = a.GetPixel(width - 1 - x, y);

                    a.SetPixel(x, y, rightPixel);
                    a.SetPixel(width - 1 - x, y, leftPixel);
                }
            }
        }

        public static void Binary(Bitmap a)
        {
            for (int x = 0; x < a.Width; x++)
            {
                for (int y = 0; y < a.Height; y++)
                {
                    Color pixel = a.GetPixel(x, y);
                    int gray = (pixel.R + pixel.G + pixel.B) / 3;
                    if (gray > 127)
                    {
                        a.SetPixel(x, y, Color.White);
                    }
                    else
                    {
                        a.SetPixel(x, y, Color.Black);
                    }
                }
            }
        }

        public static void Sepia(Bitmap a) {
            Color pixel;
            for (int x = 0; x < a.Width; x++)
            {
                for (int y = 0; y < a.Height; y++)
                {
                    pixel = a.GetPixel(x, y);
                    int red = Math.Min(255, (int)(0.393 * pixel.R + 0.769 * pixel.G + 0.189 * pixel.B));
                    int green = Math.Min(255, (int)(0.349 * pixel.R + 0.686 * pixel.G + 0.168 * pixel.B));
                    int blue = Math.Min(255, (int)(0.272 * pixel.R + 0.534 * pixel.G + 0.131 * pixel.B));

                    a.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }
        }

        public static void GreenScreen(ref Bitmap a, ref Bitmap b, ref Bitmap c, int threshold)
        {
            Color mygreen = Color.FromArgb(0, 255, 0);
            int greygreen = (mygreen.R + mygreen.G + mygreen.B) / 3;

            for (int x = 0; x < b.Width; x++)
                for (int y = 0; y < b.Height; y++)
                {
                    Color pixel = a.GetPixel(x, y);
                    Color backpixel = b.GetPixel(x, y);
                    int grey = (pixel.R + pixel.G + pixel.B) / 3;
                    int subtractvalue = Math.Abs(grey - greygreen);
                    if (subtractvalue < threshold)
                        c.SetPixel(x, y, backpixel);
                    else
                        c.SetPixel(x, y, pixel);
                }
        }

        public static void Scale(ref Bitmap a, int nwidth, int nheight)
        {
            int targetWidth = nwidth;
            int targetHeight = nheight;
            int xTarget, yTarget, xSource, ySource;
            int width = a.Width;
            int height = a.Height;
            Bitmap b = new Bitmap(targetWidth, targetHeight);

            for (xTarget = 0; xTarget < targetWidth; xTarget++)
            {
                for (yTarget = 0; yTarget < targetHeight; yTarget++)
                {
                    xSource = xTarget * width / targetWidth;
                    ySource = yTarget * height / targetHeight;
                    b.SetPixel(xTarget, yTarget, a.GetPixel(xSource, ySource));
                }
            }
            a = b;
        }


        public static void Rotate(ref Bitmap a, int value)
        {
            //float angleRadians = (float)(value * Math.PI / 180);
            float angleRadians = (float)value;
            int xCenter = (int)(a.Width / 2);
            int yCenter = (int)(a.Height / 2);
            int width, height, xs, ys, xp, yp, x0, y0;
            float cosA, sinA;
            cosA = (float)Math.Cos(angleRadians);
            sinA = (float)Math.Sin(angleRadians);
            width = a.Width;
            height = a.Height;
            Bitmap b = new Bitmap(width, height);
            for (xp = 0; xp < width; xp++)
            {
                for (yp = 0; yp < height; yp++)
                {
                    x0 = xp - xCenter; // translate to (0, 0)
                    y0 = yp - yCenter;
                    xs = (int)(x0 * cosA + y0 * sinA); // rotate around the origin
                    ys = (int)(-x0 * sinA + y0 * cosA);
                    //xs += (int)(xs + xCenter); // translate back to (xCenter, yCenter)
                    //ys += (int)(ys + yCenter);
                    xs = (int)(xs + xCenter); // translate back to (xCenter, yCenter)
                    ys = (int)(ys + yCenter);
                    xs = Math.Max(0, Math.Min(width - 1, xs)); // force the source point to be within the image
                    ys = Math.Max(0, Math.Min(height - 1, ys));
                    b.SetPixel(xp, yp, a.GetPixel(xs, ys));
                }
            }
            a = b;
        }

        public static void Equalisation(Bitmap a, int degree)
        {
            int height = a.Height;
            int width = a.Width;
            int numSamples, histSum;
            int[] Ymap = new int[256];
            int[] hist = new int[256];
            int percent = degree;

            Bitmap temp = new Bitmap(a);

            // compute the histogram from the sub-image
            Color nakuha;
            Color gray;
            Byte graydata;
            // compute greyscale
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    nakuha = a.GetPixel(x, y);
                    graydata = (byte)((nakuha.R + nakuha.G + nakuha.B) / 3);
                    gray = Color.FromArgb(graydata, graydata, graydata);
                    temp.SetPixel(x, y, gray);
                }
            }

            // histogram 1d data;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    nakuha = temp.GetPixel(x, y);
                    hist[nakuha.R]++;
                }
            }

            // remap the Ys, use the maximum contrast (percent == 100
            // based on histogram equalizaation
            numSamples = width * height;
            histSum = 0;
            for (int h = 0; h < 256; h++)
            {
                histSum += hist[h];
                Ymap[h] = (int)(255.0 * histSum / numSamples);
            }

            // if desired contrast is not maximum (percent < 100), then adjust the mapping
            if (percent < 100)
            {
                for (int h = 0; h < 256; h++)
                {
                    Ymap[h] = h + ((int)Ymap[h] - h) * percent / 100;
                }
            }

            // enhance the region by remapping the intensities
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color color = Color.FromArgb(Ymap[a.GetPixel(x, y).R], Ymap[a.GetPixel(x, y).R], Ymap[a.GetPixel(x, y).R]);
                    a.SetPixel(x, y, color);
                }
            }
        }

        public static void Brightness(Bitmap a, int value)
        {
            Color sample;
            int red, green, blue;

            for (int x = 0; x < a.Width; x++)
            {
                for (int y = 0; y < a.Height; y++)
                {
                    sample = a.GetPixel(x, y);
                    red = sample.R + value;
                    green = sample.G + value;
                    blue = sample.B + value;

                    red = Math.Max(0, Math.Min(255, red));
                    green = Math.Max(0, Math.Min(255, green));
                    blue = Math.Max(0, Math.Min(255, blue));

                    a.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }
        }

        public static void Hist(Bitmap a)
        {
            // Create a copy of the original image to work on
            Bitmap temp = new Bitmap(a);

            Color sample;
            Color gray;
            Byte graydata;

            for (int x = 0; x < temp.Width; x++)
            {
                for (int y = 0; y < temp.Height; y++)
                {
                    sample = temp.GetPixel(x, y);
                    graydata = (byte)((sample.R + sample.G + sample.B) / 3);
                    gray = Color.FromArgb(graydata, graydata, graydata);
                    temp.SetPixel(x, y, gray);
                }
            }

            int[] histdata = new int[256];

            for (int x = 0; x < temp.Width; x++)
            {
                for (int y = 0; y < temp.Height; y++)
                {
                    sample = temp.GetPixel(x, y);
                    histdata[sample.R]++;
                }
            }

            a = new Bitmap(256, 800);
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 800; y++)
                {
                    a.SetPixel(x, y, Color.White);
                }
            }

            // plotting points based from histdata
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < Math.Min(histdata[x] / 5, a.Height - 1); y++)
                {
                    a.SetPixel(x, (a.Height - 1) - y, Color.Black);
                }
            }
        }

        public enum EMBOSS { HORIZONTAL_VERTICAL, ALL_DIRECTION, LOSSY, HORIZONTAL_ONLY, VERTICAL_ONLY };
        

        public static bool Emboss(Bitmap a, EMBOSS nType)
        {
            ConvMatrix m = new ConvMatrix();

            switch(nType)
            {
                case EMBOSS.HORIZONTAL_VERTICAL:
                    m.SetAll(0);
                    m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = -1;
                    m.Pixel = 4;
                    m.Offset = 127;
                    break;
                case EMBOSS.ALL_DIRECTION:
                    m.SetAll(-1);
                    m.Pixel = 8;
                    m.Offset = 127;
                    break;
                case EMBOSS.LOSSY:
                    m.SetAll(-2);
                    m.TopLeft = m.TopRight = m.BottomMid = 1;
                    m.Pixel = 4;
                    m.Offset = 127;
                    break;
                case EMBOSS.HORIZONTAL_ONLY:
                    m.SetAll(0);
                    m.MidLeft = m.MidRight = -1;
                    m.Pixel = 2;
                    m.Offset = 127;
                    break;
                case EMBOSS.VERTICAL_ONLY:
                    m.SetAll(0);
                    m.TopMid = -1;
                    m.BottomMid = 1;
                    m.Offset = 127;
                    break;
            }

            return BitmapFilter.Conv3x3(a, m);
        }

        public static bool EdgeDetect(Bitmap a)
        {
            ConvMatrix m = new ConvMatrix();
            m.SetAll(0);
            m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = -1;
            m.Pixel = -4;
            m.Offset = 127;

            return BitmapFilter.Conv3x3(a, m);
        }

        public static bool EdgeEnhance(Bitmap a)
        {
            ConvMatrix m = new ConvMatrix();
            m.MidLeft = -1;
            m.Offset = 127;

            return BitmapFilter.Conv3x3(a, m);
        }
    }
}
