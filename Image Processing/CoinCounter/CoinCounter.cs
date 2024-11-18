using System.Collections.Concurrent;
using System.Drawing;
using System.Threading.Tasks;

namespace HoughCoinCounter
{
    internal class CoinCounter
    {
        private readonly HoughCircle houghCircle;
        private readonly List<CoinType> coinTypes;
        private readonly object lockObject = new object();

        public CoinCounter(List<CoinType> coinTypes)
        {
            this.houghCircle = new HoughCircle();
            this.coinTypes = coinTypes;
        }

        // OPTIMIZE: Instead of using threading it could be better to improve the HoughCircle class to allow for multiple radii
        public (decimal totalValue, List<(Point center, decimal value)> detectedCoins, Dictionary<decimal, int> coinCounts) CountTotalValue(Bitmap img, int suppressionRadius = 10)
        {
            decimal totalValue = 0;
            List<(Point center, decimal value)> detectedCoins = new List<(Point center, decimal value)>();
            Dictionary<decimal, int> coinCounts = new Dictionary<decimal, int>();

            // Use a concurrent dictionary for efficient thread-safe updates
            var coinCountDict = new ConcurrentDictionary<decimal, int>();

            // Process each coinType in parallel to improve performance
            Parallel.ForEach(coinTypes, coinType =>
            {
                int[,] accumulator;
                lock (lockObject)
                {
                    accumulator = houghCircle.GetAccumulator(img, coinType.Radius);
                }

                List<Point> circles = houghCircle.SearchCircles(accumulator, coinType.Threshold, suppressionRadius);

                foreach (var circle in circles)
                {
                    // Accumulate total value and add detected coins
                    lock (lockObject)
                    {
                        totalValue += coinType.Value;
                        detectedCoins.Add((circle, coinType.Value));
                    }

                    // Efficiently update the coinCounts using ConcurrentDictionary
                    coinCountDict.AddOrUpdate(coinType.Value, 1, (key, oldValue) => oldValue + 1);
                }
            });

            // Convert concurrent dictionary to normal dictionary for final result
            foreach (var coinCount in coinCountDict)
            {
                coinCounts[coinCount.Key] = coinCount.Value;
            }

            return (totalValue, detectedCoins, coinCounts);
        }
    }
}