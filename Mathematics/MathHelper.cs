#region

using System;
using System.Collections.Generic;
using HC.Core.Exceptions;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Mathematics
{
    public static class MathHelper
    {
        public static List<double> GetRanges(
            double dblLow, 
            double dblHigh,
            int intRanges)
        {
            try
            {
                if (dblHigh <= dblLow)
                {
                    throw new HCException("Low is higher");
                }
                double dblRange = (dblHigh - dblLow)/(intRanges - 1);
                var result = new List<double>();
                result.Add(dblLow);
                double dblCurrVal = dblLow;
                for (int i = 0; i < intRanges - 1; i++)
                {
                    dblCurrVal = dblCurrVal + dblRange;
                    result.Add(dblCurrVal);
                }
                result.Add(dblHigh);
                return result;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<double>();
        }

        /// <summary>
        /// Returns true if search value is between values 1 and 2
        /// </summary>
        /// <param name="dblValue1"></param>
        /// <param name="dblValue2"></param>
        /// <param name="dblSearchValue"></param>
        /// <returns></returns>
        public static bool ValueIsBetween(double dblValue1, double dblValue2, double dblSearchValue)
        {
            if (dblValue2 < dblValue1)
            {
                double dblTmp = dblValue1;
                dblValue1 = dblValue2;
                dblValue2 = dblTmp;
            }
            return (dblSearchValue >= dblValue1 && dblSearchValue <= dblValue2);
        }


        public static double RawCopySign(double magnitude, double sign)
        {
            return magnitude*Math.Sign(sign);
        }

        public static string GetInequalitySymbol(
            InequalityType inequalityType)
        {
            if (inequalityType == InequalityType.EQUALS)
            {
                return "=";
            }
            if (inequalityType == InequalityType.GREATER_OR_EQUAL)
            {
                return ">=";
            }
            if (inequalityType == InequalityType.GREATER_THAN)
            {
                return ">";
            }
            if (inequalityType == InequalityType.LESS_OR_EQUAL)
            {
                return "<=";
            }
            if (inequalityType == InequalityType.LESS_THAN)
            {
                return "<";
            }
            if (inequalityType == InequalityType.LIKE)
            {
                return "LIKE";
            }
            throw new HCException("Inequality not supported");
        }

        public static bool CheckInequality(
            InequalityType inequalityType,
            double dblValue1,
            double dblValue2)
        {
            dblValue1 = Math.Round(dblValue1,
                                   MathConstants.ROUND_DECIMALS);
            dblValue2 = Math.Round(dblValue2,
                                   MathConstants.ROUND_DECIMALS);

            if (inequalityType == InequalityType.EQUALS)
            {
                return Math.Abs(inequalityType - InequalityType.EQUALS) <=
                       MathConstants.ROUND_ERROR;
            }
            if (inequalityType == InequalityType.GREATER_OR_EQUAL)
            {
                return dblValue1 >= dblValue2; // -Base.Constants.ROUND_ERROR;
            }
            if (inequalityType == InequalityType.GREATER_THAN)
            {
                return dblValue1 > dblValue2; // -Base.Constants.ROUND_ERROR;
            }
            if (inequalityType == InequalityType.LESS_OR_EQUAL)
            {
                return dblValue1 <= dblValue2; // +Base.Constants.ROUND_ERROR;
            }
            if (inequalityType == InequalityType.LESS_THAN)
            {
                return dblValue1 < dblValue2; // +Base.Constants.ROUND_ERROR;
            }
            if (inequalityType == InequalityType.LIKE)
            {
                return Math.Abs(inequalityType - InequalityType.EQUALS) <=
                       MathConstants.ROUND_ERROR;
            }
            throw new HCException("Inequality not supported");
        }

        public static double Sqr(double dblX)
        {
            return dblX*dblX;
        }

        /** sqrt(a^2 + b^2) without under/overflow. **/

        public static double Hypot(double a, double b)
        {
            double r;
            if (Math.Abs(a) > Math.Abs(b))
            {
                r = b/a;
                r = Math.Abs(a)*Math.Sqrt(1 + r*r);
            }
            else if (b != 0)
            {
                r = a/b;
                r = Math.Abs(b)*Math.Sqrt(1 + r*r);
            }
            else
            {
                r = 0.0;
            }
            return r;
        }

        public static double GetHyperCube(double[,] dblIntegrationLimits)
        {
            double dblVolume = 1.0;
            for (int i = 0; i < dblIntegrationLimits.GetLength(0); i++)
            {
                dblVolume *= (dblIntegrationLimits[i, 1] - dblIntegrationLimits[i, 0]);
            }
            return dblVolume;
        }

        public static double Round(double dblValue)
        {
            if (double.IsPositiveInfinity(dblValue) ||
                double.IsNegativeInfinity(dblValue) ||
                -double.MaxValue < dblValue - 1)
            {
                return dblValue;
            }
            if (dblValue == 0)
            {
                return 0.0;
            }
            if (dblValue > 1.0)
            {
                return Math.Round(dblValue, 2);
            }
            int intDecimals = 0;
            double dblValue2 = dblValue;
            while (dblValue2 < 1.0)
            {
                dblValue2 *= 10.0;
                intDecimals++;
            }
            return Math.Round(dblValue, intDecimals);
        }

        /// <summary>
        /// Linear interpolation. Return a value in Y for a given set of previous/next values
        /// </summary>
        /// <param name="dblTargetXValue"></param>
        /// <param name="dblPreviousX"></param>
        /// <param name="dblPreviousY"></param>
        /// <param name="dblNextX"></param>
        /// <param name="dblNextY"></param>
        /// <returns></returns>
        public static double DoLinearInterpolation(
            double dblTargetXValue,
            double dblPreviousX,
            double dblPreviousY,
            double dblNextX,
            double dblNextY)
        {
            //
            // interpolate values
            //
            double dblDeltaTargetX = dblTargetXValue - dblNextX;
            if (dblDeltaTargetX == 0)
            {
                return dblPreviousY;
            }
            double dblDeltaX = dblPreviousX - dblNextX;
            double dblDeltaY = dblPreviousY - dblNextY;
            double dblDeltaTargetY = dblDeltaTargetX*dblDeltaY/dblDeltaX;
            double dblTargetYValue = dblDeltaTargetY + dblNextY;

            if (dblTargetYValue > Math.Max(dblNextY, dblPreviousY) ||
                dblTargetYValue < Math.Min(dblNextY, dblPreviousY))
            {
                throw new HCException("Interpolaion error.");
            }
            return dblTargetYValue;
        }

        public static InequalityType ParseInequalitySymbol(string strConstraint)
        {
            if (strConstraint.Contains("="))
            {
                return InequalityType.EQUALS;
            }
            if (strConstraint.Contains(">="))
            {
                return InequalityType.GREATER_OR_EQUAL;
            }
            if (strConstraint.Contains(">"))
            {
                return InequalityType.GREATER_THAN;
            }
            if(strConstraint.Contains("<="))
            {
                return InequalityType.LESS_OR_EQUAL;
            }
            if (strConstraint.Contains("<"))
            {
                return InequalityType.LESS_THAN;
            }
            if (strConstraint.Contains("LIKE"))
            {
                return InequalityType.LIKE;
            }
            throw new HCException("Inequality not supported");
        }

        public static bool IsAValidNumber(double dblNum)
        {
            return !Double.IsNaN(dblNum) && !Double.IsInfinity(dblNum);
        }

        public static bool IsBetween(double dblWeight, double i, double j)
        {
            return dblWeight >= i && dblWeight <= j;
        }
    }
}
