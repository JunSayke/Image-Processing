using System;
using System.Collections.Generic;
using System.Drawing;

namespace Image_Processing
{
    internal class Hough
    {
        private int[,] Accum;
        private List<Point> detectedCircles;

        public List<Point> getDetectedCircles => detectedCircles;

        /**** Algorithms for searching local maxima ****/
        public List<Point> SearchCircles(Point Size, int tr)
        {
            List<Point> circles = new List<Point>();

            // Find local maxima in the accumulator
            for (int y = 1; y < Size.Y - 1; y++)
            {
                for (int x = 1; x < Size.X - 1; x++)
                {
                    int sum = Accum[y, x]; // Center value

                    // Check the neighboring points for local maxima
                    if (sum >= tr)
                    {
                        bool isMaxima = true;
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                if (sum < Accum[y + i, x + j])
                                {
                                    isMaxima = false;
                                    break;
                                }
                            }

                            if (!isMaxima)
                                break;
                        }

                        // If it's a local maxima and above the threshold, add it to the circles list
                        if (isMaxima)
                            circles.Add(new Point(x, y));
                    }
                }
            }

            return circles;
        }

        /**** Maximum in accumulator ****/
        public int AccumMax(Point Size)
        {
            int amax = 0;
            for (int y = 0; y < Size.Y; y++)
                for (int x = 0; x < Size.X; x++)
                    if (Accum[y, x] > amax) amax = Accum[y, x];
            return amax;
        }

        /**** Normalization in accumulator ****/
        public void Normalize(Point Size, int amax)
        {
            for (int y = 0; y < Size.Y; y++)
                for (int x = 0; x < Size.X; x++)
                {
                    int c = (int)(((double)Accum[y, x] / (double)amax) * 255.0);
                    Accum[y, x] = c;
                }
        }

        /**** Non-Maximum Suppression (with a distance threshold) ****/
        public List<Point> NonMaxSuppression(List<Point> detectedCircles, int distanceThreshold)
        {
            List<Point> finalCircles = new List<Point>();

            foreach (var circle in detectedCircles)
            {
                bool isDuplicate = false;

                // Check for existing circles in the final list
                foreach (var finalCircle in finalCircles)
                {
                    // Calculate the distance between the centers of two circles
                    double distance = Math.Sqrt(Math.Pow(circle.X - finalCircle.X, 2) + Math.Pow(circle.Y - finalCircle.Y, 2));

                    // If circles are within the threshold, they are considered duplicates
                    if (distance < distanceThreshold)
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                if (!isDuplicate)
                {
                    finalCircles.Add(circle);  // Add the circle to the final list
                }
            }

            return finalCircles;
        }

        public Bitmap TransformCircle(Bitmap img, int tr, int r)
        {
            Point Size = new Point(img.Width, img.Height);
            int mang = 360;

            Accum = new int[Size.Y, Size.X];
            double dt = Math.PI / 180.0;

            // Generate the accumulator based on edge points
            for (int y = 0; y < img.Height; y++)
                for (int x = 0; x < img.Width; x++)
                    if (img.GetPixel(x, y).R == 255)
                    {
                        for (int i = 0; i < mang; i++)
                        {
                            int Tx = (int)Math.Round(x - r * Math.Cos(dt * (double)i));
                            int Ty = (int)Math.Round(y + r * Math.Sin(dt * (double)i));
                            if ((Tx < Size.X) && (Tx > 0) && (Ty < Size.Y) && (Ty > 0)) Accum[Ty, Tx]++;
                        }
                    }

            // Normalize the accumulator values
            int amax = AccumMax(Size);
            if (amax != 0)
            {
                Normalize(Size, amax);

                // Find all the circles using the search method
                detectedCircles = SearchCircles(Size, tr);

                // Apply Non-Maximum Suppression to avoid duplicates
                detectedCircles = NonMaxSuppression(detectedCircles, 20);  // Adjust threshold as needed

                // Draw the detected circles
                foreach (var circle in detectedCircles)
                {
                    // Draw a red circle at the detected center (adjust the size for visualization)
                    using (Graphics g = Graphics.FromImage(img))
                    {
                        g.FillEllipse(Brushes.Red, circle.X - 25, circle.Y - 25, 50, 50); // Small red circle (adjust size as needed)
                    }
                }
            }

            return img;
        }
    }
}
