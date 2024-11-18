using System;
using System.Collections.Generic;
using System.Drawing;

namespace HoughCoinCounter
{
    // TODO: Multiple radii for each coin type
    internal class HoughCircle
    {
        private int edgePixel = 255;
        public int EdgePixel { get => edgePixel; set => edgePixel = value; }

        public List<Point> SearchCircles(int[,] accumulator, int threshold, int supressionRadius = 10)
        {
            List<Point> circles = new List<Point>();
            int rows = accumulator.GetLength(0);
            int cols = accumulator.GetLength(1);

            // Loop through all points in the accumulator
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    int totalVotes = accumulator[y, x];
                    if (totalVotes < threshold)
                        continue; // Skip points below the threshold

                    bool isLocalMaximum = true;

                    // Check the 8 neighbors
                    for (int dy = -supressionRadius; dy <= supressionRadius; dy++)
                    {
                        for (int dx = -supressionRadius; dx <= supressionRadius; dx++)
                        {
                            if (dx == 0 && dy == 0)
                                continue; // Skip the center point

                            int neighborX = x + dx;
                            int neighborY = y + dy;

                            // Ensure neighbor is within bounds
                            if (neighborX >= 0 && neighborX < cols && neighborY >= 0 && neighborY < rows)
                            {
                                int neighborVotes = accumulator[neighborY, neighborX];
                                if (neighborVotes > totalVotes)
                                {
                                    isLocalMaximum = false;
                                    break;
                                } else
                                {
                                    accumulator[neighborY, neighborX] = 0; // Suppress the neighbor
                                }
                            }
                        }

                        if (!isLocalMaximum)
                            break;
                    }

                    // If this point is a local maximum, add it to circles
                    if (isLocalMaximum)
                    {
                        circles.Add(new Point(x, y));
                    }
                    else
                    {
                        // Set the current point to 0 (suppressed)
                        accumulator[y, x] = 0;
                    }
                }
            }

            return circles;
        }


        public int[,] GetAccumulator(Bitmap img, int radius)
        {
            int rows = img.Height;
            int cols = img.Width;

            int[,] accumulator = new int[rows, cols];
            int degrees = 360;
            double radian = Math.PI / 180.0;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    if (img.GetPixel(x, y).R == edgePixel)
                    {
                        for (int i = 0; i < degrees; i++)
                        {
                            int Tx = (int)Math.Round(x - radius * Math.Cos(radian * i));
                            int Ty = (int)Math.Round(y + radius * Math.Sin(radian * i));

                            if (Tx >= 0 && Tx < cols && Ty >= 0 && Ty < rows)
                            {
                                accumulator[Ty, Tx]++;
                            }
                        }
                    }
                }
            }

            return accumulator;
        }
    }
}
