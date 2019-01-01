#region

using System;

#endregion

namespace HC.Analytics.ConvertClasses
{
    public class ArrConv
    {
        public float[] subarray_as_float(
            int start,
            int end,
            double[] intputArray)
        {
            int intLength = intputArray.Length;
            if (end >= intLength)
            {
                throw new ArgumentException("end, " + end + ", is greater than the highest index, " + (intLength - 1));
            }
            float[] retArray = new float[end - start + 1];

            for (int i = start; i <= end; i++)
            {
                retArray[i - start] =
                    Converter.convert_double_to_float(intputArray[i]);
            }
            return retArray;
        }

        public static float[] subarray_as_float(
            int start,
            int end,
            float[] intputArray)
        {
            int intLength = intputArray.Length;
            if (end >= intLength)
            {
                throw new ArgumentException("end, " + end + ", is greater than the highest index, " + (intLength - 1));
            }
            float[] retArray = new float[end - start + 1];

            for (int i = start; i <= end; i++)
            {
                retArray[i - start] =
                    Converter.convert_double_to_float(intputArray[i]);
            }
            return retArray;
        }

        public static decimal[] subarray_as_decimal(
            int start,
            int end,
            decimal[] intputArray)
        {
            int intLength = intputArray.Length;
            if (end >= intLength)
            {
                throw new ArgumentException("end, " + end + ", is greater than the highest index, " + (intLength - 1));
            }
            decimal[] retArray = new decimal[end - start + 1];

            for (int i = start; i <= end; i++)
            {
                retArray[i - start] = intputArray[i];
            }
            return retArray;
        }


        public static double[] subarray_as_double(
            int start,
            int end,
            int[] intputArray)
        {
            int intLength = intputArray.Length;
            if (end >= intLength)
            {
                throw new ArgumentException("end, " + end + ", is greater than the highest index, " + (intLength - 1));
            }
            double[] retArray = new double[end - start + 1];

            for (int i = start; i <= end; i++)
            {
                retArray[i - start] = intputArray[i];
            }
            return retArray;
        }


        public static decimal[] array_as_decimal(
            long[] intputArray)
        {
            decimal[] retArray = new decimal[intputArray.Length];

            for (int i = 0; i <= intputArray.Length; i++)
            {
                retArray[i] = intputArray[i];
            }
            return retArray;
        }
    }
}
