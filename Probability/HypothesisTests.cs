using System;
using System.Collections.Generic;
using System.Linq;
using HC.Analytics.Statistics;
using HC.Analytics.TimeSeries.TsStats.TrainerWrappers;
using HC.Core.Logging;
using NUnit.Framework;

namespace HC.Analytics.Probability
{
    public static class HypothesisTests
    {
        [Test]
        public static void TestIsClassifierBetter()
        {
            int intSampleSize = 500;
            double dblWinPrc1 = 0.5;
            double dblWinPrc2 = 0.52;

            bool blnResult = IsClassResultDifferent(
                intSampleSize, 
                dblWinPrc1, 
                dblWinPrc2);

            Console.WriteLine(blnResult);
        }

        public static bool IsClassResultDifferent(
            int intSampleSize, 
            double dblWinPrc1, 
            double dblWinPrc2)
        {
            try
            {
                const double dblSignificance = 0.95;
                double dblExpectedOutcome1 = intSampleSize*dblWinPrc1;
                double dblFreq1 = intSampleSize - dblExpectedOutcome1;
                double dblTestStat = Math.Pow(dblFreq1 - dblExpectedOutcome1, 2)/dblExpectedOutcome1;

                double dblExpectedOutcome2 = intSampleSize*dblWinPrc2;
                double dblFreq2 = intSampleSize - dblExpectedOutcome2;
                dblTestStat += Math.Pow(dblFreq2 - dblExpectedOutcome2, 2)/dblExpectedOutcome2;

                const double intDegreesOfFreedom = 1;

                double dblChi = alglib.chisquaredistr.invchisquaredistribution(
                    intDegreesOfFreedom, 1 - dblSignificance);

                bool blnResult = dblTestStat > dblChi;
                return blnResult;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return false;
        }

        [Test]
        public static void TestIsClassificationEvenlyDistributed()
        {
            // dice examples
            var classes = new List<int>();
            AddToClasses(5, 1, classes);
            AddToClasses(8, 2, classes);
            AddToClasses(9, 3, classes);
            AddToClasses(8, 4, classes);
            AddToClasses(10, 5, classes);
            AddToClasses(20, 6, classes);
            bool blnResult = IsClassificationEvenlyDistributed(classes);
            Assert.IsTrue(!blnResult);

            classes = new List<int>();
            AddToClasses(10, 1, classes);
            AddToClasses(8, 2, classes);
            AddToClasses(9, 3, classes);
            AddToClasses(8, 4, classes);
            AddToClasses(10, 5, classes);
            AddToClasses(9, 6, classes);
            blnResult = IsClassificationEvenlyDistributed(classes);
            Assert.IsTrue(blnResult);

            // binary example
            classes = new List<int>();
            AddToClasses(200, 0, classes);
            AddToClasses(190, 1, classes);
            blnResult = IsClassificationEvenlyDistributed(classes);
            Assert.IsTrue(blnResult);

            // binary example
            classes = new List<int>();
            AddToClasses(200, 0, classes);
            AddToClasses(100, 1, classes);
            blnResult = IsClassificationEvenlyDistributed(classes);
            Assert.IsTrue(!blnResult);

        }

        private static void AddToClasses(int intNum, int intClass, List<int> classes)
        {
            for (int i = 0; i < intNum; i++)
            {
                classes.Add(intClass);
            }
        }

        public static bool IsClassificationEvenlyDistributed(
            List<int> inputData)
        {
            try
            {
                const double dblSignificance = 0.95;
                var classesSet = new Dictionary<int, int>();
                for (int i = 0; i < inputData.Count; i++)
                {
                    int intClass = inputData[i];
                    int intCounter;
                    classesSet.TryGetValue(intClass, out intCounter);
                    intCounter++;
                    classesSet[intClass] = intCounter;
                }
                if (classesSet.Count <= 1)
                {
                    //
                    // seems like all classes are in only one bucket
                    //
                    return true;
                }
                int intDegreesOfFreedom = classesSet.Count - 1;
                int intNumClasses = classesSet.Count;
                double dblExpectedOutcome = inputData.Count*1.0/intNumClasses; // TODO, this could be extended to disired expectations

                double dblTestStat = 0;
                for (int i = 0; i < intNumClasses; i++)
                {
                    int intFreq;
                    classesSet.TryGetValue(i, out intFreq);
                    dblTestStat += Math.Pow(intFreq - dblExpectedOutcome, 2)/dblExpectedOutcome;
                }
                double dblChi = alglib.chisquaredistr.invchisquaredistribution(
                    intDegreesOfFreedom, 1- dblSignificance);
                    
                return dblTestStat < dblChi;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return false;
        }

        public static bool IsMeanDiffEqualTo(
            double dblAlpha,
            double dblTestValue, 
            List<double> list1, 
            List<double> list2)
        {
            try
            {
                double dblVar1 = StdDev.GetSampleVariance(list1);
                double dblVar2 = StdDev.GetSampleVariance(list2);
                int intN1 = list1.Count;
                int intN2 = list2.Count;
                double dblMean1 = list1.Average();
                double dblMean2 = list2.Average();

                return IsMeanDiffEqualTo(
                    dblAlpha, 
                    dblTestValue, 
                    dblMean1, 
                    dblMean2, 
                    intN1, 
                    intN2, 
                    dblVar1, 
                    dblVar2);
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return false;
        }

        public static bool IsMeanDiffEqualTo(
            double dblAlpha, 
            double dblTestValue, 
            double dblMean1, 
            double dblMean2, 
            int intN1, 
            int intN2, 
            double dblVar1, 
            double dblVar2)
        {
            double dblVar = (((intN1 - 1)*dblVar1) + ((intN2 - 1)*dblVar2))/
                            (intN1 + intN2 - 2);
            double dblDen = Math.Sqrt(dblVar*((1.0/intN1) + (1.0/intN2)));
            double dblT = (dblMean1 - dblMean2 - dblTestValue)/dblDen;
            int intDof = intN1 + intN2 - 2;
            double dblAlphaTail = dblAlpha/2.0;
            double dblLowerBound = alglib.studenttdistr.invstudenttdistribution(intDof, dblAlphaTail);
            double dblUpperBound = alglib.studenttdistr.invstudenttdistribution(intDof, 1.0 - dblAlphaTail);
            return dblT >= dblLowerBound && dblT <= dblUpperBound;
        }


        public static bool IsModel2BetterThanModel1(
            RegressionTrainerWrapper regression1,
            RegressionTrainerWrapper regression2, 
            out double dblF, 
            out double dblCriticalValue)
        {
            int intP1 = regression1.Weights.Length - 1;
            int intP2 = regression2.Weights.Length - 1;
            List<double> errors1 = regression1.GetErrors();
            List<double> errors2 = regression2.GetErrors();
            return IsModel2BetterThanModel1(
                intP1,
                intP2,
                errors1,
                errors2,
                out dblF,
                out dblCriticalValue);
        }

        public static bool IsModel2BetterThanModel1(
            int intVars1, 
            int intVars2, 
            List<double> errors1, 
            List<double> errors2, 
            out double dblF, 
            out double dblCriticalValue)
        {
            double dblRss1 = (from n in errors1 select n*n).Sum();
            double dblRss2 = (from n in errors2 select n*n).Sum();
            return IsModel2BetterThanModel1(
                errors1.Count,
                dblRss1,
                dblRss2,
                intVars1,
                intVars2,
                out dblF,
                out dblCriticalValue);
        }

        public static bool IsModel2BetterThanModel1(
            int intN, 
            double dblRss1, 
            double dblRss2, 
            int intVars1, 
            int intVars2, 
            out double dblF, 
            out double dblCriticalValue)
        {
            dblF = 0;
            dblCriticalValue = 0;
            try
            {
                dblF = ((dblRss1 - dblRss2) / (intVars2 - intVars1)) / (dblRss2 / (intN - intVars2));
                int intF1 = intVars2 - intVars1;
                int intF2 = intN - intVars2;
                if (intF1 <= 0 ||
                    intF2 <= 0)
                {
                    return false;
                }
                dblCriticalValue = alglib.fdistr.fdistribution(intF1, intF2, 0.05);

                //
                // null hypothesis: Model2 does not provide better fit than model1
                //
                bool blnRejectNullHyp = dblF > dblCriticalValue;
                return blnRejectNullHyp;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return false;
        }
    }
}
