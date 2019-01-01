#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Core;
using HC.Core.Exceptions;
using HC.Core.Helpers;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.IndividualClasses
{
    /// <summary>
    ///   Individual Class:
    ///   An individual is a posible solution considered by 
    ///   an evolutionary solver. Each individual hold a chromosome 
    ///   which indicates the distributions (ELTs or CPLTs) included 
    ///   in the portfolio.
    /// 
    ///   Individuals can be evaluated and compared against each other. 
    ///   The comparison is based on the "fitness" value (m_dblFitness).
    ///   If an individual is out of bounds then it can be "repaired". 
    ///   The repair method uses old individuals previously clustered in 
    ///   order to speed up computation.
    /// 
    ///   Individuals can be improved. The improvement is done by a 
    ///   local search operator.
    /// </summary>
    [XmlRoot("Individual")]
    public partial class Individual :
        IComparable<Individual>,
        IEquatable<Individual>,
        IDisposable
    {
        #region Events

        #region Delegates

        /// <summary>
        ///   signals that the idividual has been evaluated
        /// </summary>
        /// <param name = "state"></param>
        public delegate void IndividualReadyEventHandler(EvaluationStateType state);

        #endregion

        /// <summary>
        ///   signals that the idividual has been evaluated
        /// </summary>
        public event IndividualReadyEventHandler OnIndividualReady;

        #endregion

        #region Properties

        [XmlIgnore]
        public bool IsDisposed { get; private set; }

        /// <summary>
        ///   Fitness value.
        ///   Read-only property
        /// </summary>
        [XmlElement("Fitness", typeof(double))]
        public double Fitness { get; set; }

        /// <summary>
        ///   States if the individual is evaluated
        /// </summary>
        [XmlElement("IsEvaluated", typeof(bool))]
        public bool IsEvaluated { get; set; }

        /// <summary>
        ///   Individual id
        /// </summary>
        [XmlElement("IndividualId", typeof(int))]
        public int IndividualId { get; set; }

        [XmlElement("ProblemId", typeof(string))]
        public string ProblemName { get; set; }

        [XmlArray("IndividualList")]
        [XmlArrayItem("Individual", typeof(Individual))]
        public List<Individual> IndividualList { get; set; }

        [XmlArray("FintessArr")]
        [XmlArrayItem("Fintess", typeof(double))]
        public double[] FitnessArr { get; set; }

        #endregion

        #region Members

        protected bool m_blnIsReadOnly;
        private string m_strDescr;
        private object m_descrLock = new object();
        private int m_intHashCode;

        #endregion

        #region Constructors

        public Individual()
        {
        }

        public Individual(
            double[] dblChromosomeArray,
            int[] intChromosomeArray,
            bool[] blnChromosomeArray,
            double dblFitness,
            HeuristicProblem heuristicProblem)
        {
            ProblemName = heuristicProblem.ProblemName;
            IsEvaluated = false;
            DblChromosomeArr = dblChromosomeArray;
            IntChromosomeArr = intChromosomeArray;
            BlnChromosomeArr = blnChromosomeArray;
            Fitness = dblFitness;
            //
            // avoid numerical rounding errors
            //
            ValidateChromosomeRoundDbl();
            InitializeFitnessArr(heuristicProblem);
        }

        /// <summary>
        ///   Create a random solution
        /// </summary>
        public Individual(
            HeuristicProblem heuristicProblem)
        {
            ProblemName = heuristicProblem.ProblemName;
            if (heuristicProblem.GpOperatorsContainer != null)
            {
                m_gpOperatorsContainer = heuristicProblem.GpOperatorsContainer;
            }

            InitializeFitnessArr(heuristicProblem);

            if (m_gpOperatorsContainer != null)
            {
                CreateRandomTree(heuristicProblem);
            }

            double[] dblChromosomeArr;
            int[] intChromosomeArr;
            bool[] blnChromosomeArr;

            ChromosomeFactory.BuildRandomChromosome(
                out dblChromosomeArr,
                out intChromosomeArr,
                out blnChromosomeArr,
                heuristicProblem,
                HeuristicProblem.CreateRandomGenerator());

            DblChromosomeArr = dblChromosomeArr;
            IntChromosomeArr = intChromosomeArr;
            BlnChromosomeArr = blnChromosomeArr;
        }

        #endregion

        #region Public

        public int CompareTo(Individual o)
        {
            int intCompareToValue =
                CompareToStd(o);

            if (intCompareToValue == 0)
            {
                intCompareToValue =
                    CompareToTree(o);
            }

            return intCompareToValue;
        }

        public Individual GetIndividual(string strProblemName)
        {
            if (string.IsNullOrEmpty(strProblemName))
            {
                throw new HCException("Null problem name");
            }

            var q = (from n in IndividualList
                     where n.ProblemName == strProblemName
                     select n);

            if (q.Count() == 0)
            {
                return null;
            }

            return q.First();
        }

        private void InitializeFitnessArr(
            HeuristicProblem heuristicProblem)
        {
            if (heuristicProblem != null &&
                heuristicProblem.ObjectiveCount > 1)
            {
                FitnessArr = new double[
                    heuristicProblem.ObjectiveCount];
            }
        }

        /// <summary>
        ///   Sort individuals by their fitness
        /// </summary>
        /// <param name = "o">
        ///   IIndividual
        /// </param>
        /// <returns>
        ///   Compare value
        /// </returns>
        public int CompareToStd(Individual o)
        {
            if (Fitness < o.Fitness)
            {
                return 1;
            }
            if (Fitness > o.Fitness)
            {
                return -1;
            }
            return 0;
        }

        public double GetFitnesValue(
            int intIndex)
        {
            return FitnessArr[intIndex];
        }

        public double[] GetFitnessArrCopy()
        {
            return (double[])FitnessArr.Clone();
        }

        public double Evaluate(
            HeuristicProblem heuristicProblem)
        {
            return Evaluate(
                heuristicProblem.DoLocalSearch,
                heuristicProblem.DoRepairSolution,
                heuristicProblem.DoClusterSolution,
                heuristicProblem);
        }


        public double Evaluate(bool blnDoLocalSearch,
                               bool blnRepairSolution,
                               bool blnClusterSolution,
                               HeuristicProblem heuristicProblem)
        {
            try
            {
                ValidateEvaluateIndividual();
                //
                // create a pointer to this instance
                //
                var localIndividual = this;

                bool blnSucessRepair = EvaluateOperators(
                    blnDoLocalSearch,
                    blnRepairSolution,
                    localIndividual,
                    heuristicProblem);

                EvaluateFitness(
                    localIndividual,
                    blnSucessRepair,
                    heuristicProblem);


                AckEvaluate(
                    blnClusterSolution,
                    heuristicProblem,
                    localIndividual,
                    blnSucessRepair);
            }
            catch (HCException e2)
            {
                //Logger.GetLogger().Write(e2);
                //Debugger.Break();
                InvokeIndividualReadyEventHandler(EvaluationStateType.FAILURE_EVALUATION);
                PrintToScreen.WriteLine(e2.Message);
            }
            return Fitness;
        }

        public void AckEvaluate(
            HeuristicProblem heuristicProblem)
        {
            AckEvaluate(true, heuristicProblem, this, true);
        }

        public void AckEvaluate(
            bool blnClusterSolution,
            HeuristicProblem heuristicProblem,
            Individual localIndividual,
            bool blnSucessRepair)
        {

            ClusterIndividual(
                blnClusterSolution,
                localIndividual,
                heuristicProblem);

            ValidateEvaluate(localIndividual);

            //
            // invoke individual evaluate event
            //
            InvokeIndividualReadyEventHandler(EvaluationStateType.SUCCESS_EVALUATION);
        }

        private void ValidateEvaluate(Individual localIndividual)
        {
            //
            // validate operators
            //
            if (localIndividual.Fitness != Fitness &&
                !(double.IsNaN(localIndividual.Fitness) &&
                  double.IsNaN(Fitness)))
            {
                throw new HCException("Fitness not equal.");
            }
        }

        private void ClusterIndividual(
            bool blnClusterSolution,
            Individual localIndividual,
            HeuristicProblem heuristicProblem)
        {
            if (blnClusterSolution)
            {
                heuristicProblem.Population.AddIndividualToPopulation(localIndividual);

                if (heuristicProblem.Reproduction != null)
                {
                    heuristicProblem.Reproduction.ClusterInstance(localIndividual);
                }
            }
        }

        private void EvaluateFitness(
            Individual localIndividual,
            bool blnSucessRepair,
            HeuristicProblem heuristicProblem)
        {
            //
            // evaluate individual
            //
            Fitness =
                heuristicProblem.ObjectiveFunction.Evaluate(
                localIndividual,
                heuristicProblem);

            if (IndividualList != null &&
                IndividualList.Count > 0)
            {
                foreach (Individual individual in IndividualList)
                {
                    individual.Fitness =
                        Fitness;
                }
            }

            if (!blnSucessRepair)
            {
                PenaliseRepair();
            }
            // set the individual as evaluated
            IsEvaluated = true;
        }

        private bool EvaluateOperators(
            bool blnDoLocalSearch,
            bool blnRepairSolution,
            Individual localIndividual,
            HeuristicProblem heuristicProblem)
        {
            //
            // repair solution
            //
            bool blnSucessRepair = true;
            if (blnRepairSolution &&
                heuristicProblem.RepairIndividual != null)
            {
                if (heuristicProblem.RepairProb > 0 &&
                    heuristicProblem.RepairProb > HeuristicProblem.CreateRandomGenerator().NextDouble() &&
                    !heuristicProblem.Constraints.CheckConstraints(localIndividual))
                {
                    blnSucessRepair = false;
                }
                else
                {
                    blnSucessRepair = heuristicProblem.
                        RepairIndividual.
                        DoRepair(localIndividual);

                    if (blnSucessRepair &&
                        heuristicProblem.DoCheckConstraints &&
                        !heuristicProblem.Constraints.CheckConstraints(localIndividual))
                    {
                        throw new HCException("Repair individual failed.");
                    }
                }
            }

            //
            // Do local search
            //
            if (blnSucessRepair &&
                blnDoLocalSearch &&
                heuristicProblem.LocalSearch != null)
            {
                var intMaxLocalSearchInstance = (int)(heuristicProblem.PopulationSize *
                                                       OptimisationConstants.LOCAL_SEARCH_POPULATON_FACTOR);

                if (heuristicProblem.LocalSearchProb > 0 &&
                    heuristicProblem.LocalSearchProb <
                    HeuristicProblem.CreateRandomGenerator().NextDouble() &&
                    heuristicProblem.CurrentLocalSearchInstances < intMaxLocalSearchInstance)
                {
                    //
                    // take control of the number of local search iterations in progress
                    //
                    var intCurrentLocalSearchInstances = heuristicProblem.CurrentLocalSearchInstances;
                    Interlocked.Increment(ref intCurrentLocalSearchInstances);
                    heuristicProblem.CurrentLocalSearchInstances = intCurrentLocalSearchInstances;

                    //
                    // update grid
                    //
                    heuristicProblem.ProblemStats.SetIntValue(
                        EnumHeuristicProblem.CurrentLocalSearchInstances,
                        intCurrentLocalSearchInstances);
                    heuristicProblem.PublishGridStats();

                    heuristicProblem.LocalSearch.DoLocalSearch(localIndividual);

                    //
                    // take control of the number of local search iterations in progress
                    //
                    intCurrentLocalSearchInstances = heuristicProblem.CurrentLocalSearchInstances;
                    Interlocked.Decrement(ref intCurrentLocalSearchInstances);
                    heuristicProblem.CurrentLocalSearchInstances = intCurrentLocalSearchInstances;

                    //
                    // update grid
                    //
                    heuristicProblem.ProblemStats.SetIntValue(
                        EnumHeuristicProblem.CurrentLocalSearchInstances,
                        intCurrentLocalSearchInstances);
                    heuristicProblem.PublishGridStats();
                }
            }
            return blnSucessRepair;
        }

        private void ValidateEvaluateIndividual()
        {
            ValidateReadOnly();
            //
            // check if the solution is evaluated
            //
            if (IsEvaluated)
            {
                //Debugger.Break();
                throw new HCException("IIndividual already evaluated");
            }
        }

        private void PenaliseRepair()
        {
            //
            // penalise individual since it did not satisfy the contrains
            //
            var dblLargeNegativeValue = -double.MaxValue / 2.0;

            if (dblLargeNegativeValue > Fitness ||
                Math.Abs(dblLargeNegativeValue) < Fitness)
            {
                //Debugger.Break();
                throw new HCException("Error. Infinite fitness value.");
            }

            var dblTmpFitness = Fitness - dblLargeNegativeValue;
            Fitness = dblTmpFitness - double.MaxValue;
            ////Debugger.Break();
        }

        /// <summary>
        ///   Call this method once the individual is finish with its evaluation
        /// </summary>
        /// <param name = "state">
        ///   Evaluation state (Success or Failure)
        /// </param>
        public void InvokeIndividualReadyEventHandler(EvaluationStateType state)
        {
            if (OnIndividualReady != null)
            {
                if (OnIndividualReady.GetInvocationList().Length > 0)
                {
                    OnIndividualReady.Invoke(state);
                }
            }
        }

        public void SetFitnessValue(
            double dblFitness,
            int intIndex)
        {
            m_strDescr = string.Empty;
            ValidateReadOnly();
            IsEvaluated = true;
            FitnessArr[intIndex] = dblFitness;
        }

        public void SetFitnessValue(double dblFitness)
        {
            m_strDescr = string.Empty;
            ValidateReadOnly();
            IsEvaluated = true;
            Fitness = dblFitness;
        }

        public bool IsReadOnly()
        {
            return m_blnIsReadOnly;
        }

        public void SetReadOnly()
        {
            m_blnIsReadOnly = true;
        }

        protected void ValidateReadOnly()
        {
            if (m_blnIsReadOnly)
            {
                //Debugger.Break();
                throw new HCException("Individual is read-only");
            }
        }

        #endregion

        #region IEquatable<Individual> Members

        /// <summary>
        ///   Override object.Equals.
        /// </summary>
        /// <param name = "obj">Object to compare. 
        ///   Compare only the vectors and not the weights</param>
        /// <returns>True if equal, false otherwise.</returns>
        public bool Equals(Individual obj)
        {
            if (GetHashCode() != obj.GetHashCode())
            {
                return false;
            }
            //
            // compare integer and continous chromosomes
            //
            if (DblChromosomeArr != null)
            {
                for (var i = 0; i < DblChromosomeArr.Length; i++)
                {
                    if (Math.Abs(GetChromosomeValueDbl(i) -
                                 obj.GetChromosomeValueDbl(i)) > 1E-6)
                    {
                        return false;
                    }
                }
            }
            if (IntChromosomeArr != null)
            {
                for (var i = 0; i < IntChromosomeArr.Length; i++)
                {
                    if (GetChromosomeValueInt(i) !=
                        obj.GetChromosomeValueInt(i))
                    {
                        return false;
                    }
                }
            }
            if (Root != null &&
                !Root.ToString().Equals(obj.Root.ToString()))
            {
                return false;
            }
            return true;
        }

        private void CheckIndDescr()
        {
            if (string.IsNullOrEmpty(m_strDescr))
            {
                lock (m_descrLock)
                {
                    if (string.IsNullOrEmpty(m_strDescr))
                    {
                        m_strDescr = GetStrDescr();
                        m_intHashCode = m_strDescr.GetHashCode();
                    }
                }
            }
        }

        public override int GetHashCode()
        {
            CheckIndDescr();
            return m_intHashCode;
        }

        //~Individual()
        //{
        //    Dispose();
        //}

        public void Dispose()
        {
            try
            {
                IsDisposed = true;
                if (IndividualList != null)
                {
                    for (int i = 0; i < IndividualList.Count; i++)
                    {
                        IndividualList[i].Dispose();
                    }
                    IndividualList.Clear();
                    IndividualList = null;
                }
                FitnessArr = null;
                m_descrLock = null;
                BlnChromosomeArr = null;
                IntChromosomeArr = null;
                DblChromosomeArr = null;
                if (Root != null)
                {
                    Root.Dispose();
                }
                Root = null;
                EventHandlerHelper.RemoveAllEventHandlers(this);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        #endregion

        /// <summary>
        ///   String description of current individual
        /// </summary>
        /// <returns>
        ///   String description
        /// </returns>
        public string PrintSolution()
        {
            var sb = new StringBuilder();
            sb.Append("Objective function: " + Fitness + Environment.NewLine);
            if (DblChromosomeArr != null)
            {
                for (var i = 0; i < DblChromosomeArr.Length; i++)
                {
                    sb.Append("ELT " + (i + 1) + "=" + DblChromosomeArr[i] + Environment.NewLine);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        ///   String representation of current Indivudal
        /// </summary>
        /// <returns>
        ///   String representation of current Indivudal
        /// </returns>
        public override string ToString()
        {
            CheckIndDescr();
            return m_strDescr;
        }

        private string GetStrDescr()
        {
            var sb = new StringBuilder();
            var blnAddSep = false;

            if (ContainsChromosomeDbl())
            {
                sb.Append(ToStringDbl());
                blnAddSep = true;
            }

            if (ContainsChromosomeInt())
            {
                if (blnAddSep)
                {
                    sb.Append(" || ");
                }
                sb.Append(ToStringInt());
                blnAddSep = true;
            }

            if (ContainsChromosomeBln())
            {
                if (blnAddSep)
                {
                    sb.Append(" || ");
                }
                sb.Append(ToStringBln());
                blnAddSep = true;
            }

            if (ContainsChromosomeTree())
            {
                if (blnAddSep)
                {
                    sb.Append(" || ");
                }
                ToStringTree(sb);
                //sb.Append();
            }

            if (IndividualList != null)
            {
                foreach (Individual individual in IndividualList)
                {
                    sb.Append("_innr_" + individual.ProblemName + "_" + individual);
                }
            }

            sb.Append("\nIndividual Fitness = " + Fitness);
            return sb.ToString();
        }

        public Individual Clone(
            HeuristicProblem heuristicProblem)
        {
            //
            // get chromosomes
            //
            var dblChromosomeArr =
                DblChromosomeArr == null
                    ? null
                    : GetChromosomeCopyDbl();

            var intChromosomeArr =
                IntChromosomeArr == null
                    ? null
                    : GetChromosomeCopyInt();

            var blnChromosomeArr =
                BlnChromosomeArr == null
                    ? null
                    : GetChromosomeCopyBln();

            var newIndividual =
                new Individual(
                    dblChromosomeArr,
                    intChromosomeArr,
                    blnChromosomeArr,
                    Fitness,
                    heuristicProblem);

            // multi-objective 
            newIndividual.FitnessArr =
                FitnessArr == null ? null : (double[])FitnessArr.Clone();

            if (Root != null)
            {
                CloneIndividualTree(
                    newIndividual,
                    heuristicProblem);
            }
            newIndividual.ProblemName = ProblemName;

            if (IndividualList != null &&
                IndividualList.Count > 0)
            {
                newIndividual.IndividualList =
                    new List<Individual>();

                foreach (Individual curruentIndividual in IndividualList)
                {
                    var newInnerIndividual =
                        curruentIndividual.Clone(heuristicProblem);
                    newIndividual.IndividualList.Add(
                        newInnerIndividual);
                }
            }

            return newIndividual;
        }

        public double[] GetChromosomeCopy()
        {
            if (DblChromosomeArr != null)
            {
                return GetChromosomeCopyDbl();
            }
            if (IntChromosomeArr != null)
            {
                var intChromosomeArr = GetChromosomeCopyInt();
                var dblChromosomeArr = new double[intChromosomeArr.Length];

                for (var i = 0; i < intChromosomeArr.Length; i++)
                {
                    dblChromosomeArr[i] = intChromosomeArr[i];
                }
                return dblChromosomeArr;
            }
            throw new HCException("Error. Chromosome not found.");
        }

    }
}
