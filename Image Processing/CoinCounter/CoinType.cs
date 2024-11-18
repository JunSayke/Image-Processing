using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoughCoinCounter
{
    internal class CoinType
    {
        public int Radius { get; set; } // Coin radius in pixels
        public decimal Value { get; set; } // Coin value in pesos
        public int Threshold { get; set; } // Coin threshold for HoughCircle

        public CoinType(int radius, decimal value, int threshold)
        {
            Radius = radius;
            Value = value;
            Threshold = threshold;
        }
    }
}
