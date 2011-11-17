using System;
using System.Linq;
using System.Text;

namespace Navi.KinectEngine
{
    public class DepthHistogram
    {
        #region private fields

        private short[][] _histograms;
        private int _maxDepth;

        #endregion

        #region ctor

        public DepthHistogram(int maxDepth)
        {
            _maxDepth = maxDepth;
            _histograms = new short[3][];


            _histograms[0] = new short[maxDepth];
            _histograms[1] = new short[maxDepth];
            _histograms[2] = new short[maxDepth];
        }

        #endregion

        #region internal methods

        internal void AddDepthValue(int bucket, short depthValue)
        {
            _histograms[bucket][depthValue]++;
        }

        internal void Clear()
        {
            foreach (short[] arr in _histograms.AsParallel())
                Array.Clear(arr, 0, arr.Length);
        }

        internal double[] GetClosestDepthValuesOverThreshold(int threshold, int windowSize)
        {
            double[] result = new double[3];
            
            foreach (int pos in Enumerable.Range(0, 3).AsParallel())
            {
                result[pos] = GetClosestDepthValueOverThresholdInRegion(threshold, windowSize, ref _histograms[pos]);
            }

            return result;
        }

        #endregion

        #region private methods

        private double GetClosestDepthValueOverThresholdInRegion(int threshold, int windowSize, ref short[] values)
        {
            for (int i = 500; i <= values.Length - windowSize; i += windowSize / 2)
            {
                short sum = 0;
                for (int j = 0; j < windowSize; j++)
                    sum += values[i + j];

                if (sum > threshold)
                {
                    return (i + 0.5 * windowSize);
                }
            }
            return 0;
        }

        #endregion

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 3; i++ )
            {
                for (int j = 0; j < _maxDepth; j++)
                {
                    sb.Append(_histograms[i][j]);
                    sb.Append(",");
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }
        
    }
}
