#region

using System;

#endregion

namespace HC.Analytics.TimeSeries.TsStats
{
    public class IncrFinchWeightStats
    {
        private double SnPrev;
        private int count;
        private double highestValue = Double.NegativeInfinity;
        private double lastValue;
        private double lastWeight;

        private double lowestValue = Double.PositiveInfinity;
        private double previousWeightedMean;
        private double sumOfValues;
        private double sumOfWeights;
        private double sumWeightedValues;
        private double weightedMean;
        private double weightedStdev;

        public double getMeanWeight()
        {
            return sumOfWeights / count;
        }

        public double getSumWeightedValues()
        {
            return sumWeightedValues;
        }

        public double getSumOfValues()
        {
            return sumOfValues;
        }

        public int getNumValues()
        {
            return count;
        }

        public double getSumWeights()
        {
            return sumOfWeights;
        }

        public double getLastValue()
        {
            return lastValue;
        }

        public double getLastWeight()
        {
            return lastWeight;
        }

        public double getTempWeighted(double weight, double value)
        {
            double tempSumWeighted = sumWeightedValues + (weight * value);
            double tempSumWeights = sumOfWeights + weight;
            return tempSumWeighted / tempSumWeights;
        }


        public void update(double weight, double value)
        {
            if (isValid(weight) && isValid(value))
            {
                lastValue = value;
                lastWeight = weight;
                sumOfWeights += weight;
                sumOfValues += value;

                sumWeightedValues += (weight * value);
                weightedMean = previousWeightedMean + ((weight / sumOfWeights) * (value - previousWeightedMean));

                if (Double.IsNaN(weightedMean) || Double.IsNaN(previousWeightedMean))
                {
                    weightedMean = weightedMean;
                }

                double Sn = SnPrev + (weight * (value - previousWeightedMean) * (value - weightedMean));
                SnPrev = Sn;

                weightedStdev = System.Math.Sqrt(Sn / sumOfWeights);

                previousWeightedMean = weightedMean;
                count++;

                if (highestValue < value)
                {
                    highestValue = value;
                }

                if (lowestValue > value)
                {
                    lowestValue = value;
                }
            }
        }

        public double getHighestValue()
        {
            return highestValue;
        }

        public double getLowestValue()
        {
            return lowestValue;
        }

        public double getWeightedMean()
        {
            return weightedMean;
        }

        public double getWeightedStdev()
        {
            return weightedStdev;
        }

        public int getCount()
        {
            return count;
        }

        private bool isValid(double v)
        {
            return !Double.IsNaN(v) && !Double.IsInfinity(v);
        }
    }
}
