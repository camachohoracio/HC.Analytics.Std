#region

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HC.Core.Exceptions;
using HC.Core.SearchUtils;

#endregion

namespace HC.Analytics.Mathematics.Functions.DataStructures
{
    [Serializable]
    public abstract class AFunction3D : AFunction
    {
        #region Properties

        public double XAngle { get; set; }
        public double ZAngle { get; set; }

        public double ZMin { get; set; }
        public double ZMax { get; set; }
        public string ZLabel { get; set; }

        #endregion

        #region Members

        private readonly List<FunctionRow3D> m_cachedFunctionValues;
        private readonly GenericSearchUtils3D<FunctionRow3D> m_genericSearchUtils;
        //private List<FunctionRow3D> m_functionValues;

        #endregion

        #region Constructors

        public AFunction3D()
        {
            m_cachedFunctionValues = new List<FunctionRow3D>();
            m_genericSearchUtils = new GenericSearchUtils3D<FunctionRow3D>();
            m_genericSearchUtils.OnEvaluateSearchFunctionObjectX +=
                m_genericSearchUtils_OnEvaluateSearchFunctionObjectX;
            m_genericSearchUtils.OnEvaluateSearchFunctionObjectY +=
                m_genericSearchUtils_OnEvaluateSearchFunctionObjectY;
            //
            // Set default labels
            //
            XLabel = "x";
            YLabel = "y";
            ZLabel = "z";
        }

        #endregion

        //private readonly Dictionary<string, object> checkDict = 
        //    new Dictionary<string, object>();

        private double m_genericSearchUtils_OnEvaluateSearchFunctionObjectX(
            FunctionRow3D value)
        {
            return value.X;
        }

        private double m_genericSearchUtils_OnEvaluateSearchFunctionObjectY(
            FunctionRow3D value)
        {
            return value.Y;
        }

        public virtual List<FunctionRow3D> LoadDataArr()
        {
            //List<FunctionRow3D>  m_functionValues = new List<FunctionRow3D>();
            InitialiseFunctionLimits();

            // initialize x values
            double dblXDelta = (XMax - XMin)/(Resolution - 1);
            double dblCurrentXValue = XMin;

            // initialize y values
            double dblYDelta = (YMax - YMin)/(Resolution - 1);
            double dblCurrentYValue = YMin;

            //
            // get threshold values
            //
            double[] xThresholds = new double[Resolution];
            double[] yThresholds = new double[Resolution];
            FunctionRow3D[,] functionRow3DArray =
                new FunctionRow3D[Resolution,Resolution];
            for (int i = 0; i < Resolution; i++)
            {
                xThresholds[i] = dblCurrentXValue;
                dblCurrentXValue += dblXDelta;

                yThresholds[i] = dblCurrentYValue;
                dblCurrentYValue += dblYDelta;
            }


            //for (int i = 0; i < Resolution; i++)
            Parallel.For(0, Resolution, delegate(int i)
                                            {
                                                for (int j = 0; j < Resolution; j++)
                                                {
                                                    //
                                                    // search for function value closest to the threshold provided
                                                    //
                                                    FunctionRow3D functionRow3D =
                                                        m_genericSearchUtils.SearchFunctionValue(
                                                            m_cachedFunctionValues,
                                                            xThresholds[i],
                                                            yThresholds[j]);
                                                    //
                                                    // check if cuntion provided is within the thresholds
                                                    //
                                                    if (!(functionRow3D != null &&
                                                          functionRow3D.X <= xThresholds[i] + dblXDelta/2.0 &&
                                                          functionRow3D.X >= xThresholds[i] - dblXDelta/2.0 &&
                                                          functionRow3D.Y <= yThresholds[j] + dblYDelta/2.0 &&
                                                          functionRow3D.Y >= yThresholds[j] - dblYDelta/2.0))
                                                    {
                                                        //
                                                        // a new function value is calculated
                                                        // function value could noy be reecycled
                                                        //
                                                        CalculateFunctionRow(
                                                            xThresholds,
                                                            yThresholds,
                                                            functionRow3DArray,
                                                            i,
                                                            j);
                                                    }
                                                    else
                                                    {
                                                        //
                                                        // this value will be recycled
                                                        //

                                                        functionRow3DArray[i, j] =
                                                            DoInterpolation(
                                                                xThresholds[i],
                                                                yThresholds[j],
                                                                m_cachedFunctionValues.IndexOf(functionRow3D));

                                                        if (functionRow3DArray[i, j] == null)
                                                        {
                                                            CalculateFunctionRow(
                                                                xThresholds,
                                                                yThresholds,
                                                                functionRow3DArray,
                                                                i,
                                                                j);
                                                        }

                                                        ////dfgFunctionRow3D recycledValue =
                                                        //functionRow3DArray[i, j] = 
                                                        //new FunctionRow3D(
                                                        //    xThresholds[i],
                                                        //    yThresholds[j],
                                                        //    functionRow3D.Z);
                                                    }
                                                }
                                            }
                );
            List<FunctionRow3D> functionValues = new List<FunctionRow3D>(Resolution*Resolution + 1);
            for (int i = 0; i < Resolution; i++)
            {
                for (int j = 0; j < Resolution; j++)
                {
                    //m_functionValues.Add(new FunctionRow3D(
                    //        xThresholds[i],
                    //        yThresholds[j],
                    //        EvaluateFunction(
                    //            xThresholds[i],
                    //            yThresholds[j])));
                    if (functionRow3DArray[i, j].IsNewRow)
                    {
                        m_cachedFunctionValues.Add(functionRow3DArray[i, j]);
                        functionRow3DArray[i, j].IsNewRow = false;
                    }
                    functionValues.Add(functionRow3DArray[i, j]);
                }
            }

            //
            // sort function values
            //
            m_cachedFunctionValues.Sort();
            functionValues.Sort();
            return functionValues;
        }

        private void CalculateFunctionRow(double[] xThresholds, double[] yThresholds,
                                          FunctionRow3D[,] functionRow3DArray, int i, int j)
        {
            FunctionRow3D newFunctionRow3D = new FunctionRow3D(
                xThresholds[i],
                yThresholds[j],
                EvaluateFunction(
                    xThresholds[i],
                    yThresholds[j]));
            newFunctionRow3D.IsNewRow = true;
            functionRow3DArray[i, j] = newFunctionRow3D;

            //string strKey = newFunctionRow3D.X + "_" +
            //                newFunctionRow3D.Y + "_" +
            //                newFunctionRow3D.Z;
            //checkDict.Add(strKey, null);
        }

        private FunctionRow3D DoInterpolation(
            double dblTargetXValue,
            double dblTargetYValue,
            int i)
        {
            // retrieve current function row
            FunctionRow3D currentFunctionRow3D =
                m_cachedFunctionValues[i];

            if (currentFunctionRow3D.X == dblTargetXValue &&
                currentFunctionRow3D.Y == dblTargetYValue)
            {
                //
                // no need to interpolate
                //
                return currentFunctionRow3D;
            }

            FunctionRow3D functionRow3DX = null;

            if (dblTargetXValue > currentFunctionRow3D.X)
            {
                functionRow3DX =
                    m_genericSearchUtils.GetNextXValue(
                        m_cachedFunctionValues,
                        i);
            }
            else if (dblTargetXValue < currentFunctionRow3D.X)
            {
                functionRow3DX =
                    m_genericSearchUtils.GetPreviousXValue(
                        m_cachedFunctionValues,
                        i);
            }
            else
            {
                functionRow3DX = currentFunctionRow3D.Copy();
            }

            //
            // check that actual row and retrieved row is between the sarch value
            //
            if (!MathHelper.ValueIsBetween(
                     currentFunctionRow3D.X,
                     functionRow3DX.X,
                     dblTargetXValue))
            {
                throw new HCException("Error. Function row in X not found.");
            }


            FunctionRow3D functionRow3DY = null;
            if (dblTargetYValue > currentFunctionRow3D.Y)
            {
                functionRow3DY =
                    m_genericSearchUtils.GetNextYValue(
                        m_cachedFunctionValues,
                        i);
            }
            else if (dblTargetYValue < currentFunctionRow3D.Y)
            {
                functionRow3DY =
                    m_genericSearchUtils.GetPreviousYValue(
                        m_cachedFunctionValues,
                        i);
            }
            else
            {
                functionRow3DY = currentFunctionRow3D.Copy();
            }

            //
            // check that actual row and retrieved row is between the sarch value
            //
            if (!MathHelper.ValueIsBetween(
                     currentFunctionRow3D.Y,
                     functionRow3DY.Y,
                     dblTargetYValue))
            {
                throw new HCException("Error. Function row in Y not found.");
            }


            FunctionRow3D functionRow3DX_1 = functionRow3DX =
                                             m_genericSearchUtils.SearchFunctionValue(
                                                 m_cachedFunctionValues,
                                                 functionRow3DX.X,
                                                 currentFunctionRow3D.Y);
            // the retrieved row must exist
            if (functionRow3DX_1.X != functionRow3DX.X ||
                functionRow3DX_1.Y != currentFunctionRow3D.Y)
            {
                return null;
            }
            functionRow3DX = functionRow3DX_1;


            FunctionRow3D functionRow3DY_1 = functionRow3DY =
                                             m_genericSearchUtils.SearchFunctionValue(
                                                 m_cachedFunctionValues,
                                                 currentFunctionRow3D.X,
                                                 functionRow3DY.Y);
            // the retrieved row must exist
            if (functionRow3DY_1.X != currentFunctionRow3D.X ||
                functionRow3DY_1.Y != functionRow3DY.Y)
            {
                return null;
            }
            functionRow3DY = functionRow3DY_1;


            FunctionRow3D interpolatedFunctionRow3D = new FunctionRow3D(
                dblTargetXValue,
                dblTargetYValue,
                0);

            //
            // do linear interpolation in x
            //
            interpolatedFunctionRow3D.Z =
                MathHelper.DoLinearInterpolation(
                    dblTargetXValue,
                    currentFunctionRow3D.X,
                    currentFunctionRow3D.Z,
                    functionRow3DX.X,
                    functionRow3DX.Z);

            //
            // Do interpolation in Y
            //
            double dblDeltaZ = MathHelper.DoLinearInterpolation(
                                   dblTargetYValue,
                                   currentFunctionRow3D.Y,
                                   currentFunctionRow3D.Z,
                                   functionRow3DY.Y,
                                   functionRow3DY.Z) -
                               currentFunctionRow3D.Z;

            interpolatedFunctionRow3D.Z += dblDeltaZ;

            return interpolatedFunctionRow3D;
        }

        #region Private

        private double searchUtils_evaluateFunction(
            double dblParameter)
        {
            return EvaluateFunction(
                dblParameter,
                dblParameter);
        }

        #endregion

        #region AbstractMethods

        public abstract double EvaluateFunction(double dblX, double dblY);

        #endregion

        #region Public

        public void GetLowLimit()
        {
            SearchUtilsClass searchUtilsClass = new SearchUtilsClass();
            searchUtilsClass.evaluateFunction +=
                searchUtils_evaluateFunction;
            searchUtilsClass.InterpolatedBinarySearch(0);
        }

        #endregion
    }
}
