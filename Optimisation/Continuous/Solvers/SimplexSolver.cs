#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HC.Analytics.Mathematics;
using HC.Analytics.Optimisation.Continuous.Constraints;
using HC.Analytics.Optimisation.Continuous.ObjectiveFunctions;
using HC.Core.Helpers;
using HC.Core.Io;

#endregion

namespace HC.Analytics.Optimisation.Continuous.Solvers
{
    /// <summary>
    ///   Simplex algorithm solver
    /// </summary>
    public class SimplexSolver
    {
        #region Members

        /// <summary>
        ///   Linear programming problem
        /// </summary>
        //private LPProblem m_LpProblem;
        private readonly List<LinearConstraintDbl> m_constraints;

        private readonly LinearObjectiveFunctionDbl m_objectiveFunction;
        private double[] m_dblCoefficients;
        private double m_dblObjectiveValue;

        #endregion

        #region Constructors

        public SimplexSolver(
            List<LinearConstraintDbl> constraints,
            LinearObjectiveFunctionDbl objectiveFunction)
        {
            m_constraints = constraints;
            m_objectiveFunction = objectiveFunction;
            ParseProblem();
        }

        #endregion

        #region Public

        /// <summary>
        ///   Solve
        /// </summary>
        public void Solve()
        {
            FileHelper.RunExecutable(
                "glpsol.exe",
                "--cpxlp simplexModel.mod -o output.txt",
                true,
                true);

            // parse output
            ParseSolution();

            //m_LpProblem = LPProblem.ReadCPLEX(
            //    Constants.STR_SIMPLEX_MODEL_FILENAME);
            //m_LpProblem.ModelClass = MODELCLASS.LP;
            //m_LpProblem.SolveSimplex();
            //m_LpProblem.WriteSol("solution.mod");
            PrintToScreen.WriteLine("finish solving simplex");
        }

        /// <summary>
        ///   Return objective function value
        /// </summary>
        /// <returns>
        ///   Objective function value
        /// </returns>
        public double GetObjectiveFunction()
        {
            return m_dblObjectiveValue;
        }

        /// <summary>
        ///   Get solver name
        /// </summary>
        /// <returns>
        ///   Solver name
        /// </returns>
        public string GetSolverName()
        {
            return "Simplex";
        }

        #endregion

        #region Private

        /// <summary>
        ///   Parse problem in a text file.
        /// </summary>
        private void ParseProblem()
        {
            var sw = new StreamWriter(
                Constants.STR_SIMPLEX_MODEL_FILENAME);
            // write objective function
            sw.WriteLine("maximize");
            sw.WriteLine(ParseObjectiveFunction());
            // write constraints
            sw.WriteLine(Environment.NewLine + "subject to");
            sw.WriteLine(ParseConstraints());
            // write bounds
            sw.WriteLine(Environment.NewLine + "bounds");
            sw.WriteLine(ParseBounds());
            sw.WriteLine(Environment.NewLine + "end");
            sw.Close();
        }

        /// <summary>
        ///   Parse bounds
        /// </summary>
        /// <returns>
        ///   Parsed bounds
        /// </returns>
        private string ParseBounds()
        {
            var sb = new StringBuilder();
            var intVariableCount = m_objectiveFunction.Indexes.Length;
            for (var i = 0; i < intVariableCount; i++)
            {
                sb.AppendLine("0 <= x" + (i + 1) + " <= 1");
            }
            return sb.ToString();
        }

        /// <summary>
        ///   Parse objective function
        /// </summary>
        /// <returns>
        ///   Parsed objective function
        /// </returns>
        private string ParseObjectiveFunction()
        {
            var sb = new StringBuilder();
            var dblReturnsArray =
                m_objectiveFunction.ReturnArray;
            var intIndexes =
                m_objectiveFunction.Indexes;
            sb.Append(dblReturnsArray[0] + " x" + (intIndexes[0] + 1));
            for (var i = 1; i < dblReturnsArray.Length; i++)
            {
                sb.Append(" + " + dblReturnsArray[i] + " x" + (intIndexes[i] + 1));
            }

            return sb.ToString();
        }

        /// <summary>
        ///   Parse constraints
        /// </summary>
        /// <returns>
        ///   Parsed constraints
        /// </returns>
        private string ParseConstraints()
        {
            var sb = new StringBuilder();
            // parse constraints
            foreach (LinearConstraintDbl constraint in m_constraints)
            {
                var dblCoefficients = constraint.Coefficients;
                var intIndexes = constraint.Indexes;

                sb.AppendLine(ParseConstraint(constraint));
            }
            return sb.ToString();
        }

        /// <summary>
        ///   Parse constraint
        /// </summary>
        /// <param name = "constraint">
        ///   Constraint
        /// </param>
        /// <param name = "dblOverallRPLArray">
        ///   RPL threshold array
        /// </param>
        /// <returns></returns>
        private string ParseConstraint(
            LinearConstraintDbl constraint)
        {
            var sb = new StringBuilder();

            var dblCoeffiecients = constraint.Coefficients;
            var intIndexes = constraint.Indexes;

            sb.Append(dblCoeffiecients[0] + " x" + (intIndexes[0] + 1));

            for (var i = 1; i < dblCoeffiecients.Length; i++)
            {
                sb.Append(" + " + dblCoeffiecients[i] + " x" + (intIndexes[i] + 1));
            }
            //
            // add rhs
            //
            sb.Append(" " + MathHelper.GetInequalitySymbol(constraint.Inequality) + " ");
            if (constraint.Boundary == double.MaxValue)
            {
                PrintToScreen.WriteLine("Warning. Limit is set to infinit value. We use: 1,000,000,000,000 instead.");
                sb.Append("1000000000000");
            }
            else
            {
                sb.Append(constraint.Boundary.ToString());
            }
            return sb.ToString();
        }

        private void ParseSolution()
        {
            var blnObjectiveFound = false;
            m_dblCoefficients = new double[m_objectiveFunction.Indexes.Length];
            using (var sr = new StreamReader("output.txt"))
            {
                string strLine;
                while ((strLine = sr.ReadLine()) != null)
                {
                    if (strLine.Contains("Objective:"))
                    {
                        var tokens = GetTokens(ref strLine);
                        m_dblObjectiveValue = Convert.ToDouble(tokens[3]);
                        // go straight into the variable section
                        for (var i = 0; i < 7; i++)
                        {
                            strLine = sr.ReadLine();
                        }
                        blnObjectiveFound = true;
                    }
                    if (blnObjectiveFound)
                    {
                        for (var i = 0; i < m_objectiveFunction.Indexes.Length; i++)
                        {
                            strLine = sr.ReadLine();
                            // clean string
                            var variableTokens = GetTokens(ref strLine);
                            m_dblCoefficients[i] = Convert.ToDouble(variableTokens[4]);
                        }
                        break;
                    }
                }
            }
        }

        private static string[] GetTokens(ref string strLine)
        {
            // load objective function
            while (strLine.Contains("  "))
            {
                strLine = strLine.Replace("  ", " ");
            }
            var tokens = strLine.Split(" ".ToCharArray());
            return tokens;
        }

        #endregion
    }
}
