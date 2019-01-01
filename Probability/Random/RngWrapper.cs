#region

using System;
using System.Collections.Generic;
using HC.Core.Logging;
using HC.Core.SearchUtils;

#endregion

namespace HC.Analytics.Probability.Random
{
    public class RngWrapper : ICloneable
    {
        #region Members

        private readonly int m_intSeed;
        protected System.Random m_rng;
        private readonly object m_rngLock = new object();

        #endregion

        #region Constructors
        
        public RngWrapper(int intSeed)
        {
            m_rng = RandomFactory.Create(intSeed);
            m_intSeed = intSeed;
        }

        public RngWrapper()
        {
            m_rng = RandomFactory.Create(out m_intSeed);
        }

        #endregion

        /**
         *  Insert the method's description here. Creation date: (1/26/00 11:03:40 AM)
         *
         *@return     double
         *@author:    <Vadum Kutsyy, kutsyy@hotmail.com>
         */

        #region ICloneable Members

        public object Clone()
        {
            return new RngWrapper(m_intSeed);
        }

        #endregion

        public double NextDouble()
        {
            try
            {
                lock (m_rngLock)
                {
                    return m_rng.NextDouble();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }


        public int[] GetShuffledArr(
            int intSize)
        {
            var suffledArr = new int[intSize];
            for (int i = 0; i < intSize; i++)
            {
                suffledArr[i] = i;
            }
            Shuffle(suffledArr);
            return suffledArr;
        }

        public List<int> GetShuffledList(
            int intSize)
        {
            var suffledList = new List<int>(intSize + 1);
            for (int i = 0; i < intSize; i++)
            {
                suffledList.Add(i);
            }
            Shuffle(suffledList);
            return suffledList;
        }

        /**
         *  Insert the method's description here. Creation date: (3/6/00 4:04:19 PM)
         *
         *@param  low   double[]
         *@param  high  double[]
         *@return       double
         */

        public double NextDouble(double[] low, double[] high)
        {
            int n = low.Length;
            var cut = new double[n];
            cut[0] = Math.Max(0, high[0] - low[0]);
            for (int i = 1; i < n; i++)
            {
                cut[i] = cut[i - 1] + Math.Max(0, high[i] - low[i]);
            }
            double x = NextDouble(0, cut[n - 1]);
            if (x < cut[0])
            {
                return x + low[0];
            }
            for (int i = 0; i < n - 1; i++)
            {
                if (x < cut[i + 1])
                {
                    return (x - cut[i]) + low[i + 1];
                }
            }
            return double.NaN;
        }


        /**
         *  Insert the method's description here. Creation date: (1/26/00 11:06:43 AM)
         *
         *@param  a   double
         *@param  b   double
         *@return     double
         *@author:    <Vadum Kutsyy, kutsyy@hotmail.com>
         */

        public double NextDouble(double a, double b)
        {
            if (a > b)
            {
                double tmp = a;
                a = b;
                b = tmp;
            }
            if (a == b)
            {
                return a;
            }
            return (NextDouble())*(b - a) + a;
        }


        /**
         *  Insert the method's description here. Creation date: (1/26/00 11:08:33 AM)
         *
         *@param  n   number of variables to generate
         *@return     double[]
         *@author:    <Vadum Kutsyy, kutsyy@hotmail.com>
         */

        public double[] NextDouble(int n)
        {
            var x = new double[n];
            for (int i = 0; i < n; i++)
            {
                x[i] = NextDouble();
            }
            return x;
        }

        public bool NextBln()
        {
            return NextInt(0, 1) == 1;
        }


        /**
         *  Insert the method's description here. Creation date: (1/26/00 11:09:26 AM)
         *
         *@param  n   number of variables to generate
         *@param  a   double
         *@param  b   double
         *@return     double[]
         *@author:    <Vadum Kutsyy, kutsyy@hotmail.com>
         */

        public double[] NextDouble(int n, double a, double b)
        {
            if (a > b)
            {
                double tmp = a;
                a = b;
                b = tmp;
            }
            var x = new double[n];
            for (int i = 0; i < n; i++)
            {
                x[i] = (NextDouble())*(b - a) + a;
            }
            return x;
        }

        public int[] NextInt(int n, int a, int b)
        {
            if (a > b)
            {
                int tmp = a;
                a = b;
                b = tmp;
            }
            var x = new int[n];
            for (int i = 0; i < n; i++)
            {
                x[i] = NextInt(a, b);
            }
            return x;
        }


        // _WH_, March 12,1999
        // Double BMoutput;                // constant needed by Box-Mueller algorithm
        /**
        @param hi upper limit of range
        @return a random integer in the range 1,2,... ,<STRONG>hi</STRONG>
        */

        public int NextInt(int hi)
        {
            return NextInt(1, hi); // _WH_,
            //return (int) (1+hi*raw()); // does not yield [1,hi]
        }

        /**
        @param lo lower limit of range
        @param hi upper limit of range
        @return a random integer in the range <STRONG>lo</STRONG>, <STRONG>lo</STRONG>+1, ... ,<STRONG>hi</STRONG>
        */
        public int NextInt(int lo, int hi)
        {
            try
            {
                return (int) (lo + (long) ((1L + hi - lo)*NextDouble())); // _WH_, March 12,1999
                //return (int) (lo+(hi-lo+1)*raw()); // does not yield [lo,hi]
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }

        // Returns an array, of length top+1, of unique pseudorandom integers between bottom and top
        // i.e. no integer is repeated and all integers between bottom and top inclusive are present
        public int[] uniqueIntegerArray(int bottom, int top)
        {
            int range = top - bottom;
            int[] array = uniqueIntegerArray(range);
            for (int i = 0; i < range + 1; i++)
            {
                array[i] += bottom;
            }
            return array;
        }


        // Returns an array, of length top+1, of unique pseudorandom integers between 0 and top
        // i.e. no integer is repeated and all integers between 0 and top inclusive are present
        public int[] uniqueIntegerArray(int top)
        {
            int numberOfIntegers = top + 1; // number of unique pseudorandom integers returned
            var array = new int[numberOfIntegers]; // array to contain returned unique pseudorandom integers
            //bool allFound = false;                           // will equal true when all required integers found
            int nFound = 0; // number of required pseudorandom integers found
            var found = new bool[numberOfIntegers]; // = true when integer corresponding to its index is found
            for (int i = 0; i < numberOfIntegers; i++)
            {
                found[i] = false;
            }

            bool test0 = true;
            while (test0)
            {
                int ii = NextInt(top);
                if (!found[ii])
                {
                    array[nFound] = ii;
                    found[ii] = true;
                    nFound++;
                    if (nFound == numberOfIntegers)
                    {
                        test0 = false;
                    }
                }
            }
            return array;
        }

        /// <summary>
        /// Draws a random value from an array of cumumative probabilities.
        /// NB. Assumes that the values are valid cumumative probabilities.
        /// </summary>
        /// <param name="dblSearchSpace"></param>
        /// <returns>The drawn index.</returns>
        public int Draw(List<double> dblSearchSpace)
        {
            double dblDraw = NextDouble();
            return SearchUtilsClass.DoBinarySearch(dblSearchSpace, dblDraw);
        }

        public int Draw(double[] dblSearchSpace)
        {
            double dblDraw = NextDouble();
            return Draw(
                dblSearchSpace,
                dblDraw);
        }

        public int Draw(
            double[] dblSearchSpace,
            double dblDraw)
        {
            return SearchUtilsClass.DoBinarySearch(dblSearchSpace, dblDraw);
        }

        public int Draw(
            List<double> dblSearchSpace,
            double dblDraw)
        {
            return SearchUtilsClass.DoBinarySearch(dblSearchSpace, dblDraw);
        }

        public int NextSymbol()
        {
            return NextDouble() > 0.5 ? 1 : -1;
        }


        // swaps array elements i and j
        private void Exch<T>(List<T> a, int i, int j)
        {
            T swap = a[i];
            a[i] = a[j];
            a[j] = swap;
        }

        private void Exch<T>(T[] a, int i, int j)
        {
            T swap = a[i];
            a[i] = a[j];
            a[j] = swap;
        }


        /*************************************************************************
         *  Reads in N lines, shuffles them
         *  Uses Knuth's shuffle.
         *************************************************************************/

        public void Shuffle<T>(List<T> a)
        {
            try
            {
                int N = a.Count;
                for (int i = 0; i < N; i++)
                {
                    int r = i + (int) (NextDouble()*(N - i)); // between i and N-1
                    Exch(a, i, r);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public void Shuffle<T>(T[] a)
        {
            try
            {
                int N = a.Length;
                for (int i = 0; i < N; i++)
                {
                    int r = i + (int)(NextDouble() * (N - i)); // between i and N-1
                    Exch(a, i, r);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }


        /// <summary>
        /// Shuffle list of numbers in a random order
        /// </summary>
        /// <param name="list">
        /// ArrayList of numbers
        /// </param>
        /// <returns>
        /// Shuffled list
        /// </returns>
        public void ShuffleList<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = NextInt(0, n - 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
