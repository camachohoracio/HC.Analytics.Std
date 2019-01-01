#region

using System;
using System.Collections.Generic;
using System.Linq;
using HC.Analytics.Mathematics.Functions.DataStructures;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Mathematics.Functions
{
    public class FunctionBinarySearchHelper
    {
        #region Events

        #region Delegates

        public delegate double EvauateFunctionGiven(int intIndex);

        public delegate double EvauateFunctionSearch(int intIndex);

        #endregion

        public event EvauateFunctionSearch OnEvauateFunctionSearch;

        public event EvauateFunctionGiven OnEvauateFunctionGiven;

        #endregion

        public int DoBinarySearch(
            List<FuncRow2D> functionList,
            double dblSearchValue)
        {
            //
            // check if values are in descending or ascending order
            //
            double dblLowValue = InvokeEvaluateFunctionSearch(0);
            double dblHighValue = InvokeEvaluateFunctionSearch(functionList.Count - 1);
            double dblDirection = 1.0;

            if (dblLowValue > dblHighValue)
            {
                dblDirection = -1.0;
                dblSearchValue *= dblDirection;
            }

            int intLow = 0;
            int intHigh = functionList.Count - 1;

            if (dblSearchValue >= dblDirection*InvokeEvaluateFunctionSearch(functionList.Count - 1))
            {
                return functionList.Count - 1;
            }

            while (true)
            {
                int intMid = ((intLow + intHigh)/2);

                if (dblDirection*InvokeEvaluateFunctionSearch(intMid) > dblSearchValue)
                {
                    intHigh = intMid;
                }
                else if (dblDirection*InvokeEvaluateFunctionSearch(intMid + 1) <= dblSearchValue)
                {
                    intLow = intMid + 1;
                }
                else
                {
                    return intMid;
                }
            }
        }


        public double DoBinarySearchWithInterpolaion(
            double dblTargetXValue,
            List<FuncRow2D> functionList)
        {
            //
            // get closest value via binary search
            //
            int intIndex = DoBinarySearch(
                functionList,
                dblTargetXValue);

            double dblFoundValueX = InvokeEvaluateFunctionSearch(intIndex);
            double dblFoundValueY = InvokeEvaluateGivenFunction(intIndex);


            //
            // get values before and after the array in order to find
            // out which position in the array is closer to the target value
            //
            double dblValueBeforeX = 0;
            double dblValueBeforeY = 0;
            if (intIndex > 0 && intIndex < functionList.Count - 1)
            {
                dblValueBeforeX = InvokeEvaluateFunctionSearch(intIndex - 1);
                dblValueBeforeY = InvokeEvaluateGivenFunction(intIndex - 1);
            }
            else if (intIndex <= 0)
            {
                dblValueBeforeX = InvokeEvaluateFunctionSearch(0);
                dblValueBeforeY = InvokeEvaluateGivenFunction(0);
            }
            else
            {
                dblValueBeforeX = InvokeEvaluateFunctionSearch(functionList.Count - 2);
                dblValueBeforeY = InvokeEvaluateGivenFunction(functionList.Count - 2);
            }


            double dblValueAfterX = 0;
            double dblValueAfterY = 0;
            if (intIndex > 0 && intIndex < functionList.Count - 1)
            {
                dblValueAfterX = InvokeEvaluateFunctionSearch(intIndex + 1);
                dblValueAfterY = InvokeEvaluateGivenFunction(intIndex + 1);
            }
            else if (intIndex <= 0)
            {
                dblValueAfterX = InvokeEvaluateFunctionSearch(1);
                dblValueAfterY = InvokeEvaluateGivenFunction(1);
            }
            else
            {
                dblValueAfterX = InvokeEvaluateFunctionSearch(functionList.Count - 1);
                dblValueAfterY = InvokeEvaluateGivenFunction(functionList.Count - 1);
            }
            //
            // check which value is closer to the target
            //
            double dblNextX = 0;
            double dblNextY = 0;
            if (dblTargetXValue >= Math.Min(dblFoundValueX, dblValueAfterX) &&
                dblTargetXValue <= Math.Max(dblFoundValueX, dblValueAfterX))
            {
                //
                // after and found values are between target value
                //
                dblNextX = dblValueAfterX;
                dblNextY = dblValueAfterY;
            }
            else
            {
                //
                // before and found values are between target value
                //
                dblNextX = dblValueBeforeX;
                dblNextY = dblValueBeforeY;
            }

            double dblTargetYValue =
                MathHelper.DoLinearInterpolation(
                    dblTargetXValue,
                    dblFoundValueX,
                    dblFoundValueY,
                    dblNextX,
                    dblNextY);
            return dblTargetYValue;
        }


        private double InvokeEvaluateFunctionSearch(
            int intIndex)
        {
            if (OnEvauateFunctionSearch != null)
            {
                if (OnEvauateFunctionSearch.GetInvocationList().Any())
                {
                    return OnEvauateFunctionSearch.Invoke(intIndex);
                }
            }
            throw new HCException("Error. Function not defined.");
        }

        private double InvokeEvaluateGivenFunction(
            int intIndex)
        {
            if (OnEvauateFunctionGiven != null)
            {
                if (OnEvauateFunctionGiven.GetInvocationList().Any())
                {
                    return OnEvauateFunctionGiven.Invoke(intIndex);
                }
            }
            throw new HCException("Error. Function not defined.");
        }
    }
}
