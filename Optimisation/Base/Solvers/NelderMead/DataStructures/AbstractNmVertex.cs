#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Core;

#endregion

namespace HC.Analytics.Optimisation.Base.Solvers.NelderMead.DataStructures
{
    /// <summary>
    ///   Vertex representation for Nelder-Mead algorithm
    /// </summary>
    public abstract class AbstractNmVertex : IComparable<AbstractNmVertex>, IDisposable
    {
        #region Members

        protected double[] m_dblCoordinatesArr;

        protected HeuristicProblem m_heuristicProblem;

        /// <summary>
        ///   Coordenates
        /// </summary>
        protected Individual m_individual;

        #endregion

        #region Properties

        public Individual Individual_
        {
            get { return m_individual; }
        }


        /// <summary>
        ///   Vertex value
        /// </summary>
        public double Value { get; set; }

        #endregion

        #region Constructors

        public AbstractNmVertex(
            Individual individual,
            HeuristicProblem heuristicProblem)
        {
            m_heuristicProblem = heuristicProblem;
            m_dblCoordinatesArr = GetChromosomeCopy(individual);
            m_individual = individual;
        }

        #endregion

        #region Public

        /// <summary>
        ///   Sort vertex by values
        /// </summary>
        /// <param name = "o">
        ///   Vertext to compare with
        /// </param>
        /// <returns>
        ///   Compare value
        /// </returns>
        public int CompareTo(AbstractNmVertex o)
        {
            var difference = Value - o.Value;
            if (difference < 0)
            {
                return 1;
            }
            if (difference > 0)
            {
                return -1;
            }
            return 0;
        }


        /// <summary>
        ///   Combine a vector based on a weight
        /// </summary>
        /// <param name = "dblOwnt">
        ///   Weight
        /// </param>
        /// <param name = "vertex">
        ///   Vertex
        /// </param>
        /// <returns>
        ///   Combined vertex
        /// </returns>
        public AbstractNmVertex Combine(double dblOwnt, AbstractNmVertex vertex)
        {
            int i;
            var othert = 1.0 - dblOwnt;

            var nv = CreateNmVertex();
            for (i = 0; i < m_dblCoordinatesArr.Length; i++)
            {
                var dblChromosomeValye = GetVertexValue(i)*dblOwnt +
                                         vertex.GetVertexValue(i)*othert;
                nv.SetVertexValue(i, dblChromosomeValye);
            }
            return nv;
        }

        /// <summary>
        ///   Substract vertex
        /// </summary>
        /// <param name = "vertex">
        ///   Vertex
        /// </param>
        /// <returns>
        ///   Vertex
        /// </returns>
        public AbstractNmVertex Sub(AbstractNmVertex vertex)
        {
            var nv = CreateNmVertex();
            for (var i = 0; i < m_dblCoordinatesArr.Length; i++)
            {
                var dblValue = GetVertexValue(i) - vertex.GetVertexValue(i);
                nv.SetVertexValue(i, dblValue);
            }
            return nv;
        }


        /// <summary>
        ///   Add vertex
        /// </summary>
        /// <param name = "vertex">
        ///   Vertex
        /// </param>
        /// <returns>
        ///   Vertex
        /// </returns>
        public AbstractNmVertex Add(AbstractNmVertex vertex)
        {
            var nv = CreateNmVertex();
            for (var i = 0; i < m_dblCoordinatesArr.Length; i++)
            {
                var dblValue = GetVertexValue(i) + vertex.GetVertexValue(i);

                nv.SetVertexValue(i, dblValue);
            }
            return nv;
        }

        /// <summary>
        ///   Distrance to vertex
        /// </summary>
        /// <param name = "vertex">
        ///   Vertex
        /// </param>
        /// <returns>
        ///   Distance to vertex
        /// </returns>
        public double DistanceTo(AbstractNmVertex vertex)
        {
            var dblSum = 0.0;
            double dblDifference;

            for (var i = 0; i < m_dblCoordinatesArr.Length; i++)
            {
                dblDifference = GetVertexValue(i) - vertex.GetVertexValue(i);
                dblSum += dblDifference*dblDifference;
            }
            return Math.Sqrt(dblSum);
        }

        /// <summary>
        ///   String representation of current vertex
        /// </summary>
        /// <returns>
        ///   String representation
        /// </returns>
        public override string ToString()
        {
            var txt = "(";
            for (var i = 0; i < m_dblCoordinatesArr.Length; i++)
            {
                if (i > 0)
                {
                    txt += ",";
                }
                txt += " " + Math.Round(GetVertexValue(i), 4);
            }
            txt += " ) = " + Math.Round(Value, 4);
            return txt;
        }

        ~AbstractNmVertex()
        {
            Dispose();
        }

        public void Dispose()
        {
            EventHandlerHelper.RemoveAllEventHandlers(this);
            m_dblCoordinatesArr = null;
            m_heuristicProblem = null;
            m_individual = null;
        }

        /// <summary>
        ///   Get diameter
        /// </summary>
        /// <param name = "vertexArray">
        ///   Vertex array
        /// </param>
        /// <returns>
        ///   Diameter value
        /// </returns>
        public double GetDiameter(AbstractNmVertex[] vertexArray)
        {
            double dblDiameter;
            var dblDmax = 0.0;

            var m = vertexArray.Length;
            if (m <= 1)
            {
                return 0.0;
            }

            for (var i = 0; i < m; i++)
            {
                for (var j = i + 1; j < m; j++)
                {
                    dblDiameter = vertexArray[i].DistanceTo(vertexArray[j]);
                    if (dblDmax < dblDiameter)
                    {
                        dblDmax = dblDiameter;
                    }
                }
            }
            return dblDmax;
        }

        public double GetVertexValue(
            int intIndex)
        {
            return m_dblCoordinatesArr[intIndex];
        }

        public virtual void SetVertexValue(
            int intIndex,
            double dblValue)
        {
            m_dblCoordinatesArr[intIndex] = dblValue;
            if (dblValue >= 0 && dblValue <= 1)
            {
                SetChromosomeValue(intIndex, dblValue);
            }
            else if (dblValue < 0)
            {
                SetChromosomeValue(intIndex, 0);
            }
            else if (dblValue > 1)
            {
                SetChromosomeValue(intIndex, 1);
            }
        }

        #endregion

        #region Abstract Methods

        protected abstract AbstractNmVertex CreateNmVertex();
        protected abstract void SetChromosomeValue(int intIndex, double dblValue);
        protected abstract double[] GetChromosomeCopy(Individual individual);

        #endregion
    }
}
