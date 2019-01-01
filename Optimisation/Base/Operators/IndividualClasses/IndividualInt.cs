#region

using System.Text;
using System.Xml.Serialization;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.IndividualClasses
{
    public partial class Individual
    {
        #region Properties

        [XmlArray("IntChromosomeArr")]
        [XmlArrayItem("IntChromosome", typeof (int))]
        protected int[] IntChromosomeArr { get; set; }

        #endregion

        #region Constructors

        public Individual(
            int[] intChromosomeArray,
            HeuristicProblem heuristicProblem) :
                this(null,
                     intChromosomeArray,
                     null,
                     0,
                     heuristicProblem)
        {
        }

        public Individual(
            int[] intChromosomeArray,
            double dblFitness,
            HeuristicProblem heuristicProblem) :
                this(null,
                     intChromosomeArray,
                     null,
                     dblFitness,
                     heuristicProblem)
        {
            IsEvaluated = true;
        }

        #endregion

        public int[] GetChromosomeCopyInt()
        {
            ValidateChromosomeInt();
            return (int[]) IntChromosomeArr.Clone();
        }

        private void ValidateChromosomeInt()
        {
            if (IntChromosomeArr == null)
            {
                //Debugger.Break();
                throw new HCException("Error. Null chromosome.");
            }
        }

        public int GetChromosomeValueInt(int intIndex)
        {
            try
            {
                ValidateChromosomeInt();
                return IntChromosomeArr[intIndex];
            }
            catch (HCException e)
            {
                //Debugger.Break();
                throw;
            }
        }

        public void SetChromosomeValueInt(
            int intIndex,
            int intValue)
        {
            m_strDescr = string.Empty;
            ValidateChromosomeInt();

            ValidateReadOnly();
            IntChromosomeArr[intIndex] = intValue;
            IsEvaluated = false;
            Fitness = 0;
        }

        public void AddChromosomeValueInt(
            int intIndex,
            int intValue,
            HeuristicProblem heuristicProblem)
        {
            ValidateChromosomeInt();
            ValidateReadOnly();

            if (intValue == 0)
            {
                return;
            }

            var intValueToAdd =
                intValue +
                GetChromosomeValueInt(intIndex);
            //
            // check that value is in the specified range
            //
            if (intValueToAdd > heuristicProblem.VariableRangesIntegerProbl[intIndex] ||
                intValueToAdd < 0)
            {
                //Debugger.Break();
                throw new HCException("Error. Value not valid: " + intValueToAdd);
            }

            SetChromosomeValueInt(
                intIndex,
                intValueToAdd);
        }

        public void RemoveChromosomeValueInt(
            int intIndex,
            int intValue,
            HeuristicProblem heuristicProblem)
        {
            ValidateChromosomeInt();

            ValidateReadOnly();

            if (intValue == 0)
            {
                return;
            }

            AddChromosomeValueInt(
                intIndex,
                -intValue,
                heuristicProblem);
        }

        public string ToStringInt()
        {
            var stb = new StringBuilder();
            if (IntChromosomeArr != null &&
                IntChromosomeArr.Length > 0)
            {
                stb.Append("Chromosome integer = ");
                stb.Append(IntChromosomeArr[0]);
                for (var i = 1; i < IntChromosomeArr.Length; i++)
                {
                    stb.Append(", " + IntChromosomeArr[i]);
                }
            }

            if (IndividualList != null)
            {
                foreach (Individual individual in IndividualList)
                {
                    stb.Append("_inner_" + individual.ToStringInt());
                }
            }

            return stb.ToString();
        }

        /// <summary>
        ///   Check is the individual contains the requested chromosome
        /// </summary>
        /// <returns></returns>
        public bool ContainsChromosomeInt()
        {
            return IntChromosomeArr != null;
        }
    }
}
