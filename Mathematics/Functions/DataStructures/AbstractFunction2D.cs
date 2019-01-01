#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HC.Core.SearchUtils;

#endregion

namespace HC.Analytics.Mathematics.Functions.DataStructures
{
    [Serializable]
    public abstract class AFunction2D : AFunction
    {
        #region Members

        private readonly List<FuncRow2D> m_functionValues;
        private readonly GenericSearchUtils2D<FuncRow2D> m_genericSearchUtils;
        protected int m_intItemsLimitSize;

        #endregion

        #region Constructors

        protected AFunction2D() : this(0){}

        protected AFunction2D(int intItemsLimitSize)
        {
            m_intItemsLimitSize = intItemsLimitSize;
            m_functionValues = new List<FuncRow2D>();
            m_genericSearchUtils = new GenericSearchUtils2D<FuncRow2D>();
            m_genericSearchUtils.OnEvaluateSearchFunctionObject +=
                GenericSearchUtilsOnEvaluateSearchFunctionObject;
        }

        #endregion

        private static double GenericSearchUtilsOnEvaluateSearchFunctionObject(
            FuncRow2D value)
        {
            return value.X;
        }

        #region AbstractMethods

        public abstract double EvaluateFunction(double dblX);

        #endregion

        #region Public

        private readonly object m_functionValuesLock = new object();
        public virtual List<FuncRow2D> GetFunctionData()
        {
            lock (m_functionValuesLock)
            {
                InitialiseFunctionLimits();
                double dblDelta = (XMax - XMin)/(Resolution - 1);
                double dblCurrentValue = XMin;

                //
                // get threshold values
                //
                double[] thresholds = new double[Resolution];
                FuncRow2D[] functionRow2DArray =
                    new FuncRow2D[Resolution];
                for (int i = 0; i < Resolution; i++)
                {
                    thresholds[i] = dblCurrentValue;
                    dblCurrentValue += dblDelta;
                }

                Parallel.For(0, Resolution, delegate(int i)
                                                //for (int i = 0; i < Resolution; i++)
                                                {
                                                    FuncRow2D functionRow2D =
                                                        m_genericSearchUtils.SearchFunctionValue(
                                                            m_functionValues,
                                                            thresholds[i]);
                                                    if (functionRow2D == null ||
                                                        !(functionRow2D.X <= thresholds[i] &&
                                                          functionRow2D.X >= thresholds[i] - dblDelta))
                                                    {
                                                        //
                                                        // a new function value will be added to the function list
                                                        // this will happen in a subsequent step
                                                        //
                                                        functionRow2DArray[i] = new FuncRow2D(
                                                            thresholds[i],
                                                            EvaluateFunction(
                                                                thresholds[i]));
                                                    }
                                                }
                    );

                for (int i = 0; i < Resolution; i++)
                {
                    if (functionRow2DArray[i] != null)
                    {
                        m_functionValues.Add(functionRow2DArray[i]);
                    }
                }

                //
                // sort function values
                //
                m_functionValues.Sort();
                return m_functionValues.ToList();
            }
        }


        //public static double DoBinarySearchWithInterpolaionX(
        //    List<FuncRow2D> functionList,
        //    double dblTargetXValue)
        //{
        //    //
        //    // get closest value via binary search
        //    //
        //    int intIndex = DoBinarySearchX(
        //        functionList,
        //        dblTargetXValue);

        //    double dblFoundValueX = functionList[intIndex].X;
        //    double dblFoundValueY = functionList[intIndex].Fx;


        //    //
        //    // get values before and after the array in order to find
        //    // out which position in the array is closer to the target value
        //    //
        //    double dblValueBeforeX = 0;
        //    double dblValueBeforeY = 0;
        //    if (intIndex > 0 && intIndex < functionList.Count - 1)
        //    {
        //        dblValueBeforeX = functionList[intIndex - 1].X;
        //        dblValueBeforeY = functionList[intIndex - 1].Fx;
        //    }
        //    else if (intIndex <= 0)
        //    {
        //        dblValueBeforeX = functionList[0].X;
        //        dblValueBeforeY = functionList[0].Fx;
        //    }
        //    else
        //    {
        //        dblValueBeforeX = functionList[functionList.Count - 2].X;
        //        dblValueBeforeY = functionList[functionList.Count - 2].Fx;
        //    }


        //    double dblValueAfterX = 0;
        //    double dblValueAfterY = 0;
        //    if (intIndex > 0 && intIndex < functionList.Count - 1)
        //    {
        //        dblValueAfterX = functionList[intIndex + 1].X;
        //        dblValueAfterY = functionList[intIndex + 1].Fx;
        //    }
        //    else if (intIndex <= 0)
        //    {
        //        dblValueAfterX = functionList[1].X;
        //        dblValueAfterY = functionList[1].Fx;
        //    }
        //    else
        //    {
        //        dblValueAfterX = functionList[functionList.Count - 1].X;
        //        dblValueAfterY = functionList[functionList.Count - 1].Fx;
        //    }
        //    //
        //    // check which value is closer to the target
        //    //
        //    double dblNextX = 0;
        //    double dblNextY = 0;
        //    if (dblTargetXValue >= Math.Min(dblFoundValueX, dblValueAfterX) &&
        //        dblTargetXValue <= Math.Max(dblFoundValueX, dblValueAfterX))
        //    {
        //        //
        //        // after and found values are between target value
        //        //
        //        dblNextX = dblValueAfterX;
        //        dblNextY = dblValueAfterY;
        //    }
        //    else
        //    {
        //        //
        //        // before and found values are between target value
        //        //
        //        dblNextX = dblValueBeforeX;
        //        dblNextY = dblValueBeforeY;
        //    }

        //    //
        //    // interpolate values
        //    //
        //    double dblDeltaTargetX = dblTargetXValue - dblNextX;
        //    double dblDeltaX = dblFoundValueX - dblNextX;
        //    double dblDeltaY = dblFoundValueY - dblNextY;
        //    double dblDeltaTargetY = dblDeltaTargetX * dblDeltaY / dblDeltaX;
        //    double dblTargetYValue = dblDeltaTargetY + dblNextY;

        //    if (dblTargetYValue > Math.Max(dblNextY, dblFoundValueY) ||
        //        dblTargetYValue < Math.Min(dblNextY, dblFoundValueY))
        //    {

        //        throw new HCException("Interpolaion error.");
        //    }
        //    return dblTargetYValue;
        //}

        #endregion
    }
}
