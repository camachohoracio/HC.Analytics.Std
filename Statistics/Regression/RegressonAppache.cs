using System;
using System.Collections.Generic;
using HC.Analytics.TimeSeries.TsStats;
using HC.Core.Exceptions;
using NUnit.Framework;

namespace HC.Analytics.Statistics.Regression
{
    public class RegressonAppache
    {
        /** Serializable version identifier */

        /** sum of x values */
        private double m_sumX = 0d;

        /** total variation in x (sum of squared deviations from xbar) */
        private double m_sumXx = 0d;

        /** sum of y values */
        private double m_sumY = 0d;

        /** total variation in y (sum of squared deviations from ybar) */
        private double m_sumYy = 0d;

        /** sum of products */
        private double m_sumXy = 0d;

        /** number of observations */
        private long m_n = 0;

        /** mean of accumulated x values, used in updating formulas */
        private double m_xbar = 0;

        /** mean of accumulated y values, used in updating formulas */
        private double m_ybar = 0;

        /** include an intercept or not */
        private readonly bool m_hasIntercept;

        // ---------------------Public methods--------------------------------------
        public RegressonAppache()
        {
            m_hasIntercept = true;
        }

        /**
        * Create a RegressonAppache instance, specifying whether or not to estimate
        * an intercept.
        *
        * <p>Use {@code false} to estimate a model with no intercept.  When the
        * {@code hasIntercept} property is false, the model is estimated without a
        * constant term and {@link #getIntercept()} returns {@code 0}.</p>
        *
        * @param includeIntercept whether or not to include an intercept term in
        * the regression model
        */
        public RegressonAppache(bool includeIntercept)
            : this()
        {
            m_hasIntercept = includeIntercept;
        }

        /**
         * Adds the observation (x,y) to the regression data set.
         * <p>
         * Uses updating formulas for means and sums of squares defined in
         * "Algorithms for Computing the Sample Variance: Analysis and
         * Recommendations", Chan, T.F., Golub, G.H., and LeVeque, R.J.
         * 1983, American Statistician, vol. 37, pp. 242-247, referenced in
         * Weisberg, S. "Applied Linear Regression". 2nd Ed. 1985.</p>
         *
         *
         * @param x independent variable value
         * @param y dependent variable value
         */
        public void addData(double x, double y)
        {
            if (m_n == 0)
            {
                m_xbar = x;
                m_ybar = y;
            }
            else
            {
                if (m_hasIntercept)
                {
                    double fact1 = 1.0 + m_n;
                    double fact2 = m_n / (1.0 + m_n);
                    double dx = x - m_xbar;
                    double dy = y - m_ybar;
                    m_sumXx += dx * dx * fact2;
                    m_sumYy += dy * dy * fact2;
                    m_sumXy += dx * dy * fact2;
                    m_xbar += dx / fact1;
                    m_ybar += dy / fact1;
                }
            }
            if (!m_hasIntercept)
            {
                m_sumXx += x * x;
                m_sumYy += y * y;
                m_sumXy += x * y;
            }
            m_sumX += x;
            m_sumY += y;
            m_n++;
        }

        /**
         * Appends data from another regression calculation to this one.
         *
         * <p>The mean update formulae are based on a paper written by Philippe
         * P&eacute;bay:
         * <a
         * href="http://prod.sandia.gov/techlib/access-control.cgi/2008/086212.pdf">
         * Formulas for Robust, One-Pass Parallel Computation of Covariances and
         * Arbitrary-Order Statistical Moments</a>, 2008, Technical Report
         * SAND2008-6212, Sandia National Laboratories.</p>
         *
         * @param reg model to append data from
         * @since 3.3
         */
        public void append(RegressonAppache reg)
        {
            if (m_n == 0)
            {
                m_xbar = reg.m_xbar;
                m_ybar = reg.m_ybar;
                m_sumXx = reg.m_sumXx;
                m_sumYy = reg.m_sumYy;
                m_sumXy = reg.m_sumXy;
            }
            else
            {
                if (m_hasIntercept)
                {
                    double fact1 = reg.m_n / (double)(reg.m_n + m_n);
                    double fact2 = m_n * reg.m_n / (double)(reg.m_n + m_n);
                    double dx = reg.m_xbar - m_xbar;
                    double dy = reg.m_ybar - m_ybar;
                    m_sumXx += reg.m_sumXx + dx * dx * fact2;
                    m_sumYy += reg.m_sumYy + dy * dy * fact2;
                    m_sumXy += reg.m_sumXy + dx * dy * fact2;
                    m_xbar += dx * fact1;
                    m_ybar += dy * fact1;
                }
                else
                {
                    m_sumXx += reg.m_sumXx;
                    m_sumYy += reg.m_sumYy;
                    m_sumXy += reg.m_sumXy;
                }
            }
            m_sumX += reg.m_sumX;
            m_sumY += reg.m_sumY;
            m_n += reg.m_n;
        }

        /**
         * Removes the observation (x,y) from the regression data set.
         * <p>
         * Mirrors the addData method.  This method permits the use of
         * RegressonAppache instances in streaming mode where the regression
         * is applied to a sliding "window" of observations, however the caller is
         * responsible for maintaining the set of observations in the window.</p>
         *
         * The method has no effect if there are no points of data (i.e. n=0)
         *
         * @param x independent variable value
         * @param y dependent variable value
         */
        public void removeData(double x, double y)
        {
            if (m_n > 0)
            {
                if (m_hasIntercept)
                {
                    double fact1 = m_n - 1.0;
                    double fact2 = m_n / (m_n - 1.0);
                    double dx = x - m_xbar;
                    double dy = y - m_ybar;
                    m_sumXx -= dx * dx * fact2;
                    m_sumYy -= dy * dy * fact2;
                    m_sumXy -= dx * dy * fact2;
                    m_xbar -= dx / fact1;
                    m_ybar -= dy / fact1;
                }
                else
                {
                    double fact1 = m_n - 1.0;
                    m_sumXx -= x * x;
                    m_sumYy -= y * y;
                    m_sumXy -= x * y;
                    m_xbar -= x / fact1;
                    m_ybar -= y / fact1;
                }
                m_sumX -= x;
                m_sumY -= y;
                m_n--;
            }
        }

        /**
         * Adds the observations represented by the elements in
         * <code>data</code>.
         * <p>
         * <code>(data[0][0],data[0][1])</code> will be the first observation, then
         * <code>(data[1][0],data[1][1])</code>, etc.</p>
         * <p>
         * This method does not replace data that has already been added.  The
         * observations represented by <code>data</code> are added to the existing
         * dataset.</p>
         * <p>
         * To replace all data, use <code>clear()</code> before adding the new
         * data.</p>
         *
         * @param data array of observations to be added
         * greater than or equal to 2
         */
        public void addData(double[][] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].Length < 2)
                {
                    throw new HCException("INVALID_REGRESSION_OBSERVATION");
                }
                addData(data[i][0], data[i][1]);
            }
        }

        /**
         * Adds one observation to the regression model.
         *
         * @param x the independent variables which form the design matrix
         * @param y the dependent or response variable
         * the number of independent variables in the model
         */
        public void addObservation(double[] x, double y)
        {
            if (x == null || x.Length == 0)
            {
                throw new HCException("INVALID_REGRESSION_OBSERVATION");
            }
            addData(x[0], y);
        }

        /**
         * Adds a series of observations to the regression model. The lengths of
         * x and y must be the same and x must be rectangular.
         *
         * @param x a series of observations on the independent variables
         * @param y a series of observations on the dependent variable
         * The length of x and y must be the same
         * the length of {@code y} or does not contain sufficient data to estimate the model
         */
        public void addObservations(double[][] x, double[] y)
        {
            if ((x == null) || (y == null) || (x.Length != y.Length))
            {
                throw new HCException("DIMENSIONS_MISMATCH_SIMPLE");
            }
            bool obsOk = true;
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] == null || x[i].Length == 0)
                {
                    obsOk = false;
                }
            }
            if (!obsOk)
            {
                throw new HCException("NOT_ENOUGH_DATA_FOR_NUMBER_OF_PREDICTORS");
            }
            for (int i = 0; i < x.Length; i++)
            {
                addData(x[i][0], y[i]);
            }
        }

        /**
         * Removes observations represented by the elements in <code>data</code>.
          * <p>
         * If the array is larger than the current n, only the first n elements are
         * processed.  This method permits the use of RegressonAppache instances in
         * streaming mode where the regression is applied to a sliding "window" of
         * observations, however the caller is responsible for maintaining the set
         * of observations in the window.</p>
         * <p>
         * To remove all data, use <code>clear()</code>.</p>
         *
         * @param data array of observations to be removed
         */
        public void removeData(double[][] data)
        {
            for (int i = 0; i < data.Length && m_n > 0; i++)
            {
                removeData(data[i][0], data[i][1]);
            }
        }

        /**
         * Clears all data from the model.
         */
        public void clear()
        {
            m_sumX = 0d;
            m_sumXx = 0d;
            m_sumY = 0d;
            m_sumYy = 0d;
            m_sumXy = 0d;
            m_n = 0;
        }

        /**
         * Returns the number of observations that have been added to the model.
         *
         * @return n number of observations that have been added.
         */
        public long getN()
        {
            return m_n;
        }

        /**
         * Returns the "predicted" <code>y</code> value associated with the
         * supplied <code>x</code> value,  based on the data that has been
         * added to the model when this method is activated.
         * <p>
         * <code> predict(x) = intercept + slope * x </code></p>
         * <p>
         * <strong>Preconditions</strong>: <ul>
         * <li>At least two observations (with at least two different x values)
         * must have been added before invoking this method. If this method is
         * invoked before a model can be estimated, <code>Double,NaN</code> is
         * returned.
         * </li></ul></p>
         *
         * @param x input <code>x</code> value
         * @return predicted <code>y</code> value
         */
        public double predict(double x)
        {
            double b1 = getSlope();
            if (m_hasIntercept)
            {
                return getIntercept(b1) + b1 * x;
            }
            return b1 * x;
        }

        /**
         * Returns the intercept of the estimated regression line, if
         * {@link #hasIntercept()} is true; otherwise 0.
         * <p>
         * The least squares estimate of the intercept is computed using the
         * <a href="http://www.xycoon.com/estimation4.htm">normal equations</a>.
         * The intercept is sometimes denoted b0.</p>
         * <p>
         * <strong>Preconditions</strong>: <ul>
         * <li>At least two observations (with at least two different x values)
         * must have been added before invoking this method. If this method is
         * invoked before a model can be estimated, <code>Double,NaN</code> is
         * returned.
         * </li></ul></p>
         *
         * @return the intercept of the regression line if the model includes an
         * intercept; 0 otherwise
         * @see #RegressonAppache(bool)
         */
        public double getIntercept()
        {
            return m_hasIntercept ? getIntercept(getSlope()) : 0.0;
        }

        /**
         * Returns true if the model includes an intercept term.
         *
         * @return true if the regression includes an intercept; false otherwise
         * @see #RegressonAppache(bool)
         */
        public bool setHasIntercept()
        {
            return m_hasIntercept;
        }

        /**
        * Returns the slope of the estimated regression line.
        * <p>
        * The least squares estimate of the slope is computed using the
        * <a href="http://www.xycoon.com/estimation4.htm">normal equations</a>.
        * The slope is sometimes denoted b1.</p>
        * <p>
        * <strong>Preconditions</strong>: <ul>
        * <li>At least two observations (with at least two different x values)
        * must have been added before invoking this method. If this method is
        * invoked before a model can be estimated, <code>double.NaN</code> is
        * returned.
        * </li></ul></p>
        *
        * @return the slope of the regression line
        */
        public double getSlope()
        {
            if (m_n < 2)
            {
                return double.NaN; //not enough data
            }
            if (Math.Abs(m_sumXx) < 10 * Double.MinValue)
            {
                return double.NaN; //not enough variation in x
            }
            return m_sumXy / m_sumXx;
        }

        /**
         * Returns the <a href="http://www.xycoon.com/SumOfSquares.htm">
         * sum of squared errors</a> (SSE) associated with the regression
         * model.
         * <p>
         * The sum is computed using the computational formula</p>
         * <p>
         * <code>SSE = SYY - (SXY * SXY / SXX)</code></p>
         * <p>
         * where <code>SYY</code> is the sum of the squared deviations of the y
         * values about their mean, <code>SXX</code> is similarly defined and
         * <code>SXY</code> is the sum of the products of x and y mean deviations.
         * </p><p>
         * The sums are accumulated using the updating algorithm referenced in
         * {@link #addData}.</p>
         * <p>
         * The return value is constrained to be non-negative - i.e., if due to
         * rounding errors the computational formula returns a negative result,
         * 0 is returned.</p>
         * <p>
         * <strong>Preconditions</strong>: <ul>
         * <li>At least two observations (with at least two different x values)
         * must have been added before invoking this method. If this method is
         * invoked before a model can be estimated, <code>Double,NaN</code> is
         * returned.
         * </li></ul></p>
         *
         * @return sum of squared errors associated with the regression model
         */
        public double getSumSquaredErrors()
        {
            return Math.Max(0d, m_sumYy - m_sumXy * m_sumXy / m_sumXx);
        }

        /**
         * Returns the sum of squared deviations of the y values about their mean.
         * <p>
         * This is defined as SSTO
         * <a href="http://www.xycoon.com/SumOfSquares.htm">here</a>.</p>
         * <p>
         * If <code>n < 2</code>, this returns <code>double.NaN</code>.</p>
         *
         * @return sum of squared deviations of y values
         */
        public double getTotalSumSquares()
        {
            if (m_n < 2)
            {
                return double.NaN;
            }
            return m_sumYy;
        }

        /**
         * Returns the sum of squared deviations of the x values about their mean.
         *
         * If <code>n < 2</code>, this returns <code>double.NaN</code>.</p>
         *
         * @return sum of squared deviations of x values
         */
        public double getXSumSquares()
        {
            if (m_n < 2)
            {
                return double.NaN;
            }
            return m_sumXx;
        }

        /**
         * Returns the sum of crossproducts, x<sub>i</sub>*y<sub>i</sub>.
         *
         * @return sum of cross products
         */
        public double getSumOfCrossProducts()
        {
            return m_sumXy;
        }

        /**
         * Returns the sum of squared deviations of the predicted y values about
         * their mean (which equals the mean of y).
         * <p>
         * This is usually abbreviated SSR or SSM.  It is defined as SSM
         * <a href="http://www.xycoon.com/SumOfSquares.htm">here</a></p>
         * <p>
         * <strong>Preconditions</strong>: <ul>
         * <li>At least two observations (with at least two different x values)
         * must have been added before invoking this method. If this method is
         * invoked before a model can be estimated, <code>double.NaN</code> is
         * returned.
         * </li></ul></p>
         *
         * @return sum of squared deviations of predicted y values
         */
        public double getRegressionSumSquares()
        {
            return getRegressionSumSquares(getSlope());
        }

        /**
         * Returns the sum of squared errors divided by the degrees of freedom,
         * usually abbreviated MSE.
         * <p>
         * If there are fewer than <strong>three</strong> data pairs in the model,
         * or if there is no variation in <code>x</code>, this returns
         * <code>double.NaN</code>.</p>
         *
         * @return sum of squared deviations of y values
         */
        public double getMeanSquareError()
        {
            if (m_n < 3)
            {
                return double.NaN;
            }
            return m_hasIntercept ? (getSumSquaredErrors() / (m_n - 2)) : (getSumSquaredErrors() / (m_n - 1));
        }

        /**
         * Returns <a href="http://mathworld.wolfram.com/CorrelationCoefficient.html">
         * Pearson's product moment correlation coefficient</a>,
         * usually denoted r.
         * <p>
         * <strong>Preconditions</strong>: <ul>
         * <li>At least two observations (with at least two different x values)
         * must have been added before invoking this method. If this method is
         * invoked before a model can be estimated, <code>Double,NaN</code> is
         * returned.
         * </li></ul></p>
         *
         * @return Pearson's r
         */
        public double getR()
        {
            double b1 = getSlope();
            double result = Math.Sqrt(getRSquare());
            if (b1 < 0)
            {
                result = -result;
            }
            return result;
        }

        /**
         * Returns the <a href="http://www.xycoon.com/coefficient1.htm">
         * coefficient of determination</a>,
         * usually denoted r-square.
         * <p>
         * <strong>Preconditions</strong>: <ul>
         * <li>At least two observations (with at least two different x values)
         * must have been added before invoking this method. If this method is
         * invoked before a model can be estimated, <code>Double,NaN</code> is
         * returned.
         * </li></ul></p>
         *
         * @return r-square
         */
        public double getRSquare()
        {
            double ssto = getTotalSumSquares();
            return (ssto - getSumSquaredErrors()) / ssto;
        }

        /**
         * Returns the <a href="http://www.xycoon.com/standarderrorb0.htm">
         * standard error of the intercept estimate</a>,
         * usually denoted s(b0).
         * <p>
         * If there are fewer that <strong>three</strong> observations in the
         * model, or if there is no variation in x, this returns
         * <code>double.NaN</code>.</p> Additionally, a <code>double.NaN</code> is
         * returned when the intercept is constrained to be zero
         *
         * @return standard error associated with intercept estimate
         */
        public double getInterceptStdErr()
        {
            if (!m_hasIntercept)
            {
                return double.NaN;
            }
            return Math.Sqrt(
                getMeanSquareError() * ((1d / m_n) + (m_xbar * m_xbar) / m_sumXx));
        }

        /**
         * Returns the <a href="http://www.xycoon.com/standerrorb(1).htm">standard
         * error of the slope estimate</a>,
         * usually denoted s(b1).
         * <p>
         * If there are fewer that <strong>three</strong> data pairs in the model,
         * or if there is no variation in x, this returns <code>double.NaN</code>.
         * </p>
         *
         * @return standard error associated with slope estimate
         */
        public double getSlopeStdErr()
        {
            return Math.Sqrt(getMeanSquareError() / m_sumXx);
        }

        /**
         * Returns the half-width of a 95% confidence interval for the slope
         * estimate.
         * <p>
         * The 95% confidence interval is</p>
         * <p>
         * <code>(getSlope() - getSlopeConfidenceInterval(),
         * getSlope() + getSlopeConfidenceInterval())</code></p>
         * <p>
         * If there are fewer that <strong>three</strong> observations in the
         * model, or if there is no variation in x, this returns
         * <code>double.NaN</code>.</p>
         * <p>
         * <strong>Usage Note</strong>:<br>
         * The validity of this statistic depends on the assumption that the
         * observations included in the model are drawn from a
         * <a href="http://mathworld.wolfram.com/BivariateNormalDistribution.html">
         * Bivariate Normal Distribution</a>.</p>
         *
         * @return half-width of 95% confidence interval for the slope estimate
         */
        //public double getSlopeConfidenceInterval() 
        //{
        //    return getSlopeConfidenceInterval(0.05);
        //}

        /**
         * Returns the half-width of a (100-100*alpha)% confidence interval for
         * the slope estimate.
         * <p>
         * The (100-100*alpha)% confidence interval is </p>
         * <p>
         * <code>(getSlope() - getSlopeConfidenceInterval(),
         * getSlope() + getSlopeConfidenceInterval())</code></p>
         * <p>
         * To request, for example, a 99% confidence interval, use
         * <code>alpha = .01</code></p>
         * <p>
         * <strong>Usage Note</strong>:<br>
         * The validity of this statistic depends on the assumption that the
         * observations included in the model are drawn from a
         * <a href="http://mathworld.wolfram.com/BivariateNormalDistribution.html">
         * Bivariate Normal Distribution</a>.</p>
         * <p>
         * <strong> Preconditions:</strong><ul>
         * <li>If there are fewer that <strong>three</strong> observations in the
         * model, or if there is no variation in x, this returns
         * <code>double.NaN</code>.
         * </li>
         * <li><code>(0 < alpha < 1)</code>; otherwise an
         * <code>OutOfRangeException</code> is thrown.
         * </li></ul></p>
         *
         * @param alpha the desired significance level
         * @return half-width of 95% confidence interval for the slope estimate
         */
        //public double getSlopeConfidenceInterval(double alpha)
        //{
        //    if (n < 3) {
        //        return double.NaN;
        //    }
        //    if (alpha >= 1 || alpha <= 0) {
        //        throw new Exception("LocalizedFormats.SIGNIFICANCE_LEVEL");
        //    }
        //    // No advertised NotStrictlyPositiveException here - will return NaN above
        //    TDistribution distribution = new TDistribution(n - 2);
        //    return getSlopeStdErr() *
        //        distribution.inverseCumulativeProbability(1d - alpha / 2d);
        //}

        /**
         * Returns the significance level of the slope (equiv) correlation.
         * <p>
         * Specifically, the returned value is the smallest <code>alpha</code>
         * such that the slope confidence interval with significance level
         * equal to <code>alpha</code> does not include <code>0</code>.
         * On regression output, this is often denoted <code>Prob(|t| > 0)</code>
         * </p><p>
         * <strong>Usage Note</strong>:<br>
         * The validity of this statistic depends on the assumption that the
         * observations included in the model are drawn from a
         * <a href="http://mathworld.wolfram.com/BivariateNormalDistribution.html">
         * Bivariate Normal Distribution</a>.</p>
         * <p>
         * If there are fewer that <strong>three</strong> observations in the
         * model, or if there is no variation in x, this returns
         * <code>double.NaN</code>.</p>
         *
         * @return significance level for slope/correlation
         * if the significance level can not be computed.
         */
        //public double getSignificance() {
        //    if (n < 3) {
        //        return double.NaN;
        //    }
        //    // No advertised NotStrictlyPositiveException here - will return NaN above
        //    TDistribution distribution = new TDistribution(n - 2);
        //    return 2d * (1.0 - distribution.cumulativeProbability(
        //                FastMath.abs(getSlope()) / getSlopeStdErr()));
        //}

        // ---------------------Private methods-----------------------------------

        /**
        * Returns the intercept of the estimated regression line, given the slope.
        * <p>
        * Will return <code>NaN</code> if slope is <code>NaN</code>.</p>
        *
        * @param slope current slope
        * @return the intercept of the regression line
        */
        private double getIntercept(double slope)
        {
            if (m_hasIntercept)
            {
                return (m_sumY - slope * m_sumX) / m_n;
            }
            return 0.0;
        }

        /**
         * Computes SSR from b1.
         *
         * @param slope regression slope estimate
         * @return sum of squared deviations of predicted y values
         */
        private double getRegressionSumSquares(double slope)
        {
            return slope * slope * m_sumXx;
        }

        /**
         * Performs a regression on data present in buffers and outputs a RegressionResults object.
         *
         * <p>If there are fewer than 3 observations in the model and {@code hasIntercept} is true
         * a {@code NoDataException} is thrown.  If there is no intercept term, the model must
         * contain at least 2 observations.</p>
         *
         * @return RegressionResults acts as a container of regression output
         * estimate the regression parameters
         */
        public RegressionResults regress()
        {
            if (m_hasIntercept)
            {
                if (m_n < 3)
                {
                    throw new HCException("NOT_ENOUGH_DATA_REGRESSION");
                }
                if (Math.Abs(m_sumXx) > 1e-6)
                {
                    double[] params1 = new double[] { getIntercept(), getSlope() };
                    double mse = getMeanSquareError();
                    double _syy = m_sumYy + m_sumY * m_sumY / m_n;
                    double[] vcv = new double[]{
                mse * (m_xbar *m_xbar /m_sumXx + 1.0 / m_n),
                -m_xbar*mse/m_sumXx,
                mse/m_sumXx };
                    return new RegressionResults(
                            params1, new double[][] { vcv }, true, m_n, 2,
                            m_sumY, _syy, getSumSquaredErrors(), true, false);
                }
                else
                {
                    double[] params1 = new double[] { m_sumY / m_n, double.NaN };
                    //double mse = getMeanSquareError();
                    double[] vcv = new double[]{
                m_ybar / (m_n - 1.0),
                double.NaN,
                double.NaN };
                    return new RegressionResults(
                            params1,
                            new double[][] { vcv },
                            true,
                            m_n,
                            1,
                            m_sumY,
                            m_sumYy,
                            getSumSquaredErrors(),
                            true,
                            false);
                }
            }
            else
            {
                if (m_n < 2)
                {
                    throw new HCException("NOT_ENOUGH_DATA_REGRESSION");
                }
                if (!Double.IsNaN(m_sumXx))
                {
                    double[] vcv = new double[] { getMeanSquareError() / m_sumXx };
                    double[] params1 = new double[] { m_sumXy / m_sumXx };
                    return new RegressionResults(
                                params1, new double[][] { vcv }, true, m_n, 1,
                                m_sumY, m_sumYy, getSumSquaredErrors(), false, false);
                }
                else
                {
                    double[] vcv = new double[] { double.NaN };
                    double[] params1 = new double[] { double.NaN };
                    return new RegressionResults(
                                params1, new double[][] { vcv }, true, m_n, 1,
                                double.NaN, double.NaN, double.NaN, false, false);
                }
            }
        }

        /**
         * Performs a regression on data present in buffers including only regressors
         * indexed in variablesToInclude and outputs a RegressionResults object
         * @param variablesToInclude an array of indices of regressors to include
         * @return RegressionResults acts as a container of regression output
         */
        public RegressionResults regress(int[] variablesToInclude)
        {
            if (variablesToInclude == null || variablesToInclude.Length == 0)
            {
                throw new HCException("ARRAY_ZERO_LENGTH_OR_NULL_NOT_ALLOWED");
            }
            if (variablesToInclude.Length > 2 || (variablesToInclude.Length > 1 && !m_hasIntercept))
            {
                throw new HCException(
                        "ARRAY_SIZE_EXCEEDS_MAX_VARIABLES");
            }

            if (m_hasIntercept)
            {
                if (variablesToInclude.Length == 2)
                {
                    if (variablesToInclude[0] == 1)
                    {
                        throw new HCException("NOT_INCREASING_SEQUENCE");
                    }
                    else if (variablesToInclude[0] != 0)
                    {
                        throw new HCException("variablesToInclude");
                    }
                    if (variablesToInclude[1] != 1)
                    {
                        throw new HCException("variablesToInclude[0], 0,1");
                    }
                    return regress();
                }
                else
                {
                    if (variablesToInclude[0] != 1 && variablesToInclude[0] != 0)
                    {
                        throw new HCException("variablesToInclude[0],0,1");
                    }
                    double _mean = m_sumY * m_sumY / m_n;
                    double _syy = m_sumYy + _mean;
                    if (variablesToInclude[0] == 0)
                    {
                        //just the mean
                        double[] vcv = new double[] { m_sumYy / (((m_n - 1) * m_n)) };
                        double[] params1 = new double[] { m_ybar };
                        return new RegressionResults(
                          params1, new double[][] { vcv }, true, m_n, 1,
                          m_sumY, _syy + _mean, m_sumYy, true, false);

                    }
                    else if (variablesToInclude[0] == 1)
                    {
                        //double _syy = sumYY + sumY * sumY / ((double) n);
                        double _sxx = m_sumXx + m_sumX * m_sumX / m_n;
                        double _sxy = m_sumXy + m_sumX * m_sumY / m_n;
                        double _sse = Math.Max(0d, _syy - _sxy * _sxy / _sxx);
                        double _mse = _sse / ((m_n - 1));
                        if (!Double.IsNaN(_sxx))
                        {
                            double[] vcv = new double[] { _mse / _sxx };
                            double[] params1 = new double[] { _sxy / _sxx };
                            return new RegressionResults(
                                        params1, new double[][] { vcv }, true, m_n, 1,
                                        m_sumY, _syy, _sse, false, false);
                        }
                        else
                        {
                            var vcv = new[] { double.NaN };
                            var params1 = new[] { double.NaN };
                            return new RegressionResults(
                                        params1, new double[][] { vcv }, true, m_n, 1,
                                        double.NaN, double.NaN, double.NaN, false, false);
                        }
                    }
                }
            }
            else
            {
                if (variablesToInclude[0] != 0)
                {
                    throw new HCException("variablesToInclude[0],0,0");
                }
                return regress();
            }

            return null;
        }

        [Test]
        public static void DoTest()
        {
            var arr = RollingWindowRegression.GetTestData();
            var x = new List<double[]>();
            var y = new List<double>();
            for (int i = 0; i < arr.Length; i++)
            {
                var currArr = arr[i].Split(',');
                x.Add(new[]
                {
                    double.Parse(currArr[0])
                });
                y.Add(double.Parse(currArr[1]));
            }
            var simpleRegression = new RegressonAppache(false);
            simpleRegression.addObservations(
                    x.ToArray(),
                    y.ToArray());

            Console.WriteLine("done");

        }
    }
}
