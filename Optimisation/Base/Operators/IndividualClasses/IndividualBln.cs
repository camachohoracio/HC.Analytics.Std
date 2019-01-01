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

        [XmlArray("BlnChromosomeArr")]
        [XmlArrayItem("BlnChromosome", typeof (bool))]
        public bool[] BlnChromosomeArr { get; set; }

        #endregion

        #region Constructors

        public Individual(
            bool[] blnChromosomeArray,
            HeuristicProblem heuristicProblem) :
                this(null,
                     null,
                     blnChromosomeArray,
                     0,
                     heuristicProblem)
        {
        }

        public Individual(
            bool[] blnChromosomeArray,
            double dblFitness,
            HeuristicProblem heuristicProblem) :
                this(null,
                     null,
                     blnChromosomeArray,
                     dblFitness,
                     heuristicProblem)
        {
            IsEvaluated = true;
        }

        #endregion

        public bool[] GetChromosomeCopyBln()
        {
            if (BlnChromosomeArr == null)
            {
                return null;
            }

            return (bool[]) BlnChromosomeArr.Clone();
        }

        public bool GetChromosomeValueBln(int intIndex)
        {
            return BlnChromosomeArr[intIndex];
        }

        public void SetChromosomeValueBln(
            int intIndex,
            bool blnValue)
        {
            ValidateReadOnly();

            if (BlnChromosomeArr[intIndex] == blnValue)
            {
                throw new HCException("Error. Chromosome value already set.");
            }
            m_strDescr = string.Empty;
            BlnChromosomeArr[intIndex] = blnValue;
            IsEvaluated = false;
            Fitness = 0;
        }

        private string ToStringBln()
        {
            var stb = new StringBuilder();
            if (BlnChromosomeArr != null &&
                BlnChromosomeArr.Length > 0)
            {
                stb.Append(BlnChromosomeArr[0] ? "1" : "0");
                for (var i = 1; i < BlnChromosomeArr.Length; i++)
                {
                    stb.Append(", " + (BlnChromosomeArr[i] ? "1" : "0"));
                }
            }

            if (IndividualList != null)
            {
                foreach (Individual individual in IndividualList)
                {
                    stb.Append("_inner_" + individual.ToStringBln());
                }
            }

            return stb.ToString();
        }

        public bool ContainsChromosomeBln()
        {
            return BlnChromosomeArr != null;
        }
    }
}
