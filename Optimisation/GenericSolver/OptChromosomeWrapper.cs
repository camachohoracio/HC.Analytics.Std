#region

using System;
using System.Collections.Generic;

#endregion

namespace HC.Analytics.Optimisation.GenericSolver
{
    public class OptChromosomeWrapper :
        IComparer<OptChromosomeWrapper>,
        IEquatable<OptChromosomeWrapper>,
        IComparable<OptChromosomeWrapper>
    {
        #region Properties

        public double[] DblChromosome { get; private set; }
        public int[] IntChromosome { get; private set; }
        public bool[] BlnChromosome { get; private set; }
        public OptStatsCache OptStatsCache { get; set; }

        #endregion

        #region Constructors

        public OptChromosomeWrapper(
            double[] dblChromosome,
            int[] intChromosome,
            bool[] blnChromosome)
        {
            DblChromosome = dblChromosome;
            IntChromosome = intChromosome;
            BlnChromosome = blnChromosome;
        }

        #endregion

        #region IComparable<OptChromosomeWrapper> Members

        public int CompareTo(OptChromosomeWrapper other)
        {
            return Compare(this, other);
        }

        #endregion

        #region IComparer<OptChromosomeWrapper> Members

        public int Compare(OptChromosomeWrapper x, OptChromosomeWrapper y)
        {
            if (DblChromosome != null)
            {
                for (var i = 0; i < x.DblChromosome.Length; i++)
                {
                    if (x.DblChromosome[i] > y.DblChromosome[i])
                    {
                        return 1;
                    }
                    if (x.DblChromosome[i] < y.DblChromosome[i])
                    {
                        return -1;
                    }
                }
            }

            if (IntChromosome != null)
            {
                for (var i = 0; i < x.IntChromosome.Length; i++)
                {
                    if (x.IntChromosome[i] > y.IntChromosome[i])
                    {
                        return 1;
                    }
                    if (x.IntChromosome[i] < y.IntChromosome[i])
                    {
                        return -1;
                    }
                }
            }

            if (BlnChromosome != null)
            {
                for (var i = 0; i < x.BlnChromosome.Length; i++)
                {
                    if (x.BlnChromosome[i] != y.BlnChromosome[i])
                    {
                        return 1;
                    }
                }
            }
            return 0;
        }

        #endregion

        #region IEquatable<OptChromosomeWrapper> Members

        public bool Equals(OptChromosomeWrapper other)
        {
            if (DblChromosome != null)
            {
                for (var i = 0; i < DblChromosome.Length; i++)
                {
                    if (DblChromosome[i] != other.DblChromosome[i])
                    {
                        return false;
                    }
                }
            }

            if (IntChromosome != null)
            {
                for (var i = 0; i < IntChromosome.Length; i++)
                {
                    if (IntChromosome[i] != other.IntChromosome[i])
                    {
                        return false;
                    }
                }
            }

            if (BlnChromosome != null)
            {
                for (var i = 0; i < BlnChromosome.Length; i++)
                {
                    if (BlnChromosome[i] != other.BlnChromosome[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion
    }
}
