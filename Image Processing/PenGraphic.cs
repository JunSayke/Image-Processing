using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Processing
{
    internal class PenGraphic
    {
        public Font Font { get; set; } = new Font("Arial", 28, FontStyle.Bold); // Default font
        public Brush TextBrush { get; set; } = Brushes.LightGreen; // Default brush for text color

        public PenGraphic()
        {
        }

        public PenGraphic(Font font, Brush textBrush)
        {
            Font = font;
            TextBrush = textBrush;
        }

        public void Draw(Graphics g, Point center, string Text)
        {
            // Measure the size of the text to center it
            SizeF textSize = g.MeasureString(Text, Font);

            // Calculate position to center the string
            float x = center.X - textSize.Width / 2;
            float y = center.Y - textSize.Height / 2;

            // Draw the text at the calculated position
            g.DrawString(Text, Font, TextBrush, x, y);
        }
    }
}
