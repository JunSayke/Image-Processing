using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;

namespace HoughCoinCounter
{
    internal class HoughCircle
    {
        public static List<Point> SearchCircles(int[,] accumulator, int threshold, int supressionRadius = 10)
        {
            int[,] temp = (int[,])accumulator.Clone(); // Prevent overriding the original 2D array
            List<Point> circles = new List<Point>();
            int rows = temp.GetLength(0);
            int cols = temp.GetLength(1);

            // Loop through all points in the accumulator
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    int totalVotes = temp[y, x];
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
                                int neighborVotes = temp[neighborY, neighborX];
                                if (neighborVotes > totalVotes)
                                {
                                    isLocalMaximum = false;
                                    break;
                                } else
                                {
                                    temp[neighborY, neighborX] = 0; // Suppress the neighbor
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
                        temp[y, x] = 0;
                    }
                }
            }

            return circles;
        }

        public static int[,] GetAccumulator(Bitmap img, int radius, int edgePixel = 255)
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

        public static Bitmap VisualizeAccumulator(int[,] accumulator)
        {
            int rows = accumulator.GetLength(0);
            int cols = accumulator.GetLength(1);
            Bitmap visualization = new Bitmap(cols, rows);

            // Find the maximum value in the accumulator to normalize the colors
            int maxVotes = 0;
            foreach (var vote in accumulator)
            {
                maxVotes = Math.Max(maxVotes, vote);
            }

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    int totalVotes = accumulator[y, x];
                    // Normalize the vote count to a range between 0 and 255 for the intensity
                    int colorIntensity = (int)((double)totalVotes / maxVotes * 255);
                    // Create a color based on the intensity (light red to really red gradient)
                    Color color = Color.FromArgb(255, colorIntensity, 0, 0);
                    visualization.SetPixel(x, y, color);
                }
            }

            return visualization;
        }

    }
}
