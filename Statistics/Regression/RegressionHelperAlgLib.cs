#region

using System;
using System.Collections.Generic;
using System.Linq;
using HC.Core.Exceptions;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Statistics.Regression
{
    public static class RegressionHelperAlgLib
    {
        public static double[] GetRegressionWeights(
            List<double[]> x,
            List<double> y,
            out alglib.linreg.lrreport lrreport)
        {
            lrreport = null;
            try
            {
                if (x == null ||
                    x.Count == 0 ||
                    y == null ||
                    y.Count == 0 ||
                    x.First() == null)
                {
                    return null;
                }

                if(x[0].Length >= x.Count - 1)
                {
                    Logger.Log(new HCException("Number of variables [" + x[0].Length +
                                    "] are too large in comparison to num of samples [" + x.Count + "]"));
                    return null;
                }

                double[,] xy = GetXy(x, y); // samples,varcounts + 1

                double[] weights = 
                    GetRegressionWeights(
                        out lrreport, 
                        xy);
                return weights;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return new double[0];
        }

        public static double[] GetRegressionWeights(
            out alglib.linreg.lrreport lrreport,
            double[,] xy)
        {
            lrreport = null;
            try
            {
                int intNumSamples = xy.GetLength(0);
                int intNumVars = xy.GetLength(1)-1;

                int intInfo = 0;
                var linearModel = new alglib.linreg.linearmodel();

                lrreport = new alglib.linreg.lrreport();
                alglib.linreg.lrbuild(xy,
                    intNumSamples,
                    intNumVars,
                    ref intInfo,
                    linearModel,
                    lrreport);
                var weights = new double[intNumVars + 1];
                if (weights.Length == 0)
                {
                    throw new HCException("Empty weight vector");
                }
                alglib.linreg.lrunpack(linearModel, ref weights, ref intNumVars);
                if (intInfo != 1)
                {
                    Logger.Log("Regression error. intInfo [" + intInfo + "]");
                }
                return weights;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new double[0];
        }

        public static double[,] GetXy(
            List<double[]> x, 
            List<double> y)
        {
            int intSamples = x.Count;
            int intVarCount = x.First().Length;

            var xy = new double[intSamples,intVarCount + 1];
            for (int i = 0; i < intSamples; i++)
            {
                for (int j = 0; j < intVarCount; j++)
                {
                    double dblValue = x[i][j];
                    if (double.IsNaN(dblValue) || double.IsInfinity(dblValue))
                    {
                        throw new HCException("Invalid value");
                    }
                    xy[i, j] = dblValue;
                }
                double dblYValue = y[i];
                if (double.IsNaN(dblYValue) || double.IsInfinity(dblYValue))
                {
                    throw new HCException("Invalid value");
                }
                xy[i, intVarCount] = dblYValue;
            }
            return xy;
        }
    }
}

