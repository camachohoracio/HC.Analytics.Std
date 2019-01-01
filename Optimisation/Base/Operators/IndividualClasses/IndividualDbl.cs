#region

using System;
using System.Text;
using System.Xml.Serialization;
using HC.Analytics.Mathematics;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.IndividualClasses
{
    public partial class Individual
    {
        #region Properties

        [XmlArray("DblChromosomeArr")]
        [XmlArrayItem("DblChromosome", typeof (double))]
        public double[] DblChromosomeArr { get; set; }

        #endregion

        #region Constructors

        public Individual(
            double[] dblChromosomeArray,
            HeuristicProblem heuristicProblem) :
                this(dblChromosomeArray,
                     null,
                     null,
                     0,
                     heuristicProblem)
        {
        }

        public Individual(
            double[] dblChromosomeArray,
            double dblFitness,
            HeuristicProblem heuristicProblem) :
                this(dblChromosomeArray,
                     null,
                     null,
                     dblFitness,
                     heuristicProblem)
        {
            IsEvaluated = true;
        }

        #endregion

        public double[] GetChromosomeCopyDbl()
        {
            ValidateChromosomeDbl();
            return (double[]) DblChromosomeArr.Clone();
        }

        public double GetChromosomeValueDbl(int intIndex)
        {
            try
            {
                ValidateChromosomeDbl();
                return DblChromosomeArr[intIndex];
            }
            catch (HCException e)
            {
                //Debugger.Break();
                throw;
            }
        }

        private void ValidateChromosomeDbl()
        {
            if (DblChromosomeArr == null)
            {
                //Debugger.Break();
                throw new HCException("Error. Null chromosome.");
            }
        }

        public void SetChromosomeValueDbl(int intIndex, double dblValue)
        {
            if (dblValue > 1.0 || dblValue < 0.0)
            {
                throw new HCException("Chromosome value = " + dblValue);
            }

            m_strDescr = string.Empty;
            dblValue = Math.Round(dblValue,
                                  MathConstants.ROUND_DECIMALS);

            ValidateChromosomeDbl();
            ValidateReadOnly();
            DblChromosomeArr[intIndex] = dblValue;
            IsEvaluated = false;
            Fitness = 0;
        }

        public void RemoveChromosomeValueDbl(
            int intIndex,
            double dblValue)
        {
            ValidateChromosomeDbl();
            ValidateReadOnly();

            if (dblValue < MathConstants.ROUND_ERROR)
            {
                return;
            }

            AddChromosomeValueDbl(
                intIndex,
                -dblValue);
        }

        public void AddChromosomeValueDbl(
            int intIndex,
            double dblValue)
        {
            ValidateChromosomeDbl();
            ValidateReadOnly();

            var dblNewChromosomeValue =
                dblValue +
                GetChromosomeValueDbl(intIndex);

            if (dblNewChromosomeValue > 1.0 ||
                dblNewChromosomeValue < 0.0)
            {
                //Debugger.Break();
                throw new HCException("Error. Value not valid: " + dblNewChromosomeValue);
            }

            SetChromosomeValueDbl(
                intIndex,
                dblNewChromosomeValue);
        }

        protected void ValidateChromosomeRoundDbl()
        {
            //
            // avoid numerical rounding errors
            //
            if (DblChromosomeArr == null)
            {
                return;
            }

            for (var i = 0; i < DblChromosomeArr.Length; i++)
            {
                DblChromosomeArr[i] = Math.Round(DblChromosomeArr[i],
                                                 MathConstants.ROUND_DECIMALS);
            }
        }

        public string ToStringDbl()
        {
            var stb = new StringBuilder();
            if (DblChromosomeArr != null &&
                DblChromosomeArr.Length > 0)
            {
                stb.Append("Chromosome continuous = ");
                stb.Append(Math.Round(DblChromosomeArr[0], 4));
                for (var i = 1; i < DblChromosomeArr.Length; i++)
                {
                    stb.Append(", " + Math.Round(DblChromosomeArr[i], 4));
                }
            }

            if (IndividualList != null)
            {
                foreach (Individual individual in IndividualList)
                {
                    stb.Append("_inner_" + individual.ToStringDbl());
                }
            }

            return stb.ToString();
        }

        /// <summary>
        ///   Check is the individual contains the requested chromosome
        /// </summary>
        /// <returns></returns>
        public bool ContainsChromosomeDbl()
        {
            return DblChromosomeArr != null;
        }
    }
}
