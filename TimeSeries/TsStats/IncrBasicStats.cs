using System;

namespace HC.Analytics.TimeSeries.TsStats
{
    [Serializable]
    public class IncrBasicStats
    {
        public double MaxValue { get; private set; }
        public double MinValue { get; private set; }
        public double sumOfValues;
        public int NumValues;
        public double MeanValue { get; private set; }
        public double StdevValue { get; private set; }
        public double kurtosisValue;
        public double varValue;
        public double sumValSqrd;
        public double fourthMomentAboutMean;
        private bool hasBeenUpdated = false;
        private double prevValue;
        private int countNegStretch = 0;
        private int countPosStretch = 0;
        private double sumAbsValues = 0;
        private double lastValue = 0;

        public int numPositiveValues;
        public int numNegativeValues;
        public double meanPositiveValue;
        public double meanNegativeValue;
        public double sumOfPositiveValues;
        public double sumOfNegativeValues;

        public double getSumAbsValues()
        {
            return sumAbsValues;
        }

        public virtual String toString()
        {
            return "High=" + MaxValue + 
                ", Low=" + MinValue + 
                ", Sum=" + sumOfValues + 
                ", Num=" + NumValues + 
                ", Mean=" + MeanValue + 
                ", Stdev=" + StdevValue;
        }

        public override String ToString()
        {
            return toString();
        }

        public bool isHasBeenUpdated()
        {
            return hasBeenUpdated;
        }

        public IncrBasicStats()
        {
            MaxValue = Double.NegativeInfinity;
            MinValue = Double.PositiveInfinity;
            sumOfValues = 0;
            NumValues = 0;
            MeanValue = 0;
            StdevValue = 0;
            kurtosisValue = 0;
            sumValSqrd = 0;
            fourthMomentAboutMean = 0;
            hasBeenUpdated = false;
            countNegStretch = 0;
            countPosStretch = 0;
            sumAbsValues = 0;

            numPositiveValues = 0;
            numNegativeValues = 0;
            meanPositiveValue = 0;
            meanNegativeValue = 0;
            sumOfPositiveValues = 0;
            sumOfNegativeValues = 0;
        }


        public double getVarValue()
        {
            return varValue;
        }

        public void Update(double value)
        {
            if (!Double.IsNaN(value))
            {
                hasBeenUpdated = true;
                NumValues++;

                if (value > MaxValue)
                {
                    MaxValue = value;
                }
                if (value < MinValue)
                {
                    MinValue = value;
                }
                lastValue = value;
                sumOfValues += value;
                sumValSqrd += (value * value);
                double meanOfValSqrd = sumValSqrd / (double)NumValues;
                sumAbsValues += System.Math.Abs(value);
                MeanValue = sumOfValues / (double)NumValues;

                if (value > 0)
                {
                    sumOfPositiveValues += value;
                    numPositiveValues++;
                    meanPositiveValue = sumOfPositiveValues / (double)numPositiveValues;
                }
                else
                {
                    sumOfNegativeValues += value;
                    numNegativeValues++;
                    meanNegativeValue = sumOfNegativeValues / (double)numNegativeValues;
                }

                varValue = meanOfValSqrd - (MeanValue * MeanValue);
                StdevValue = System.Math.Sqrt(varValue);

                double valDiff = MeanValue - value;
                valDiff *= valDiff;
                valDiff *= valDiff;
                fourthMomentAboutMean += valDiff;
                // kurtosisValue = -3 + (fourthMomentAboutMean/numValues) / (varValue*varValue);
                kurtosisValue = (fourthMomentAboutMean / NumValues) / (varValue * varValue);

                if (prevValue >= 0 && value >= 0)
                {
                    countPosStretch++;
                    countNegStretch = 0;
                }
                else if (prevValue < 0 && value < 0)
                {
                    countNegStretch++;
                    countPosStretch = 0;
                }
                else
                {
                    countPosStretch = 0;
                    countNegStretch = 0;
                }

                prevValue = value;
            }
        }

        public double getLastValue()
        {
            return lastValue;
        }

        public int getCountNegStretch()
        {
            return countNegStretch;
        }

        public int getCountPosStretch()
        {
            return countPosStretch;
        }

        public double getHighestValue()
        {
            return MaxValue;
        }

        public double getLowestValue()
        {
            return MinValue;
        }

        public double getSumOfValues()
        {
            return sumOfValues;
        }

        public double getStdevValue()
        {
            return StdevValue;
        }

        public int getNumValues()
        {
            return NumValues;
        }

        public double getMeanValue()
        {
            return MeanValue;
        }

        public double getKurtosisValue()
        {
            return kurtosisValue;
        }

        public int getNumPositiveValues()
        {
            return numPositiveValues;
        }

        public int getNumNegativeValues()
        {
            return numNegativeValues;
        }

        public double getSumPositiveValues()
        {
            return sumOfPositiveValues;
        }

        public double getSumNegativeValues()
        {
            return sumOfNegativeValues;
        }

        public double getMeanPostiveValue()
        {
            return meanPositiveValue;
        }

        public double getMeanNegativeValue()
        {
            return meanNegativeValue;
        }
    }
}
