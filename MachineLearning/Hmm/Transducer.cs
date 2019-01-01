using System;
using System.Text;

namespace HC.Analytics.MachineLearning.Hmm
{
    public class Transducer
    {
        private const double CONVERGENCE = 0.000000000000000000001;
        private const double DEFAULT_PROB = 0.0000000001;
        private const int VOCABULARY = 255;
        private readonly double[] m_frequencyArray;
        private readonly int m_iterations = 100;
        private double[][] m_ab;
        private double[][] m_af;
        private bool m_converge;
        private double[][] m_e;
        private double m_edeletion;
        private double m_eend;
        private double m_ematch;
        private double m_ematchSubs;
        private double m_eunmatch;
        private double m_lastConvergence;
        private double[][] m_p;
        private double m_pDeletion;
        private double m_pEnd;
        private double m_pMatch;
        private double m_pTv;
        private double m_pUnmatch;
        private int m_t;
        private int m_v;

        public Transducer(int iterations, double[] frequencyArray)
        {
            m_iterations = iterations;
            m_frequencyArray = frequencyArray;
            LoadParameters();
        }

        private void LoadParameters()
        {
            ExpectationMaximization();
            ToStringProbabilities();
            Console.WriteLine("finished obtaining probabilities from trainning data");
        }

        public double[][] GetP()
        {
            return m_p;
        }

        #region Em Algorithm

        private void Forward(char[] x, char[] y)
        {
            m_af = new double[m_t + 1][];

            for (int i = 0; i < m_t + 1; i++)
            {
                m_af[i] = new double[m_v + 1];
            }

            // initial conditions
            m_af[0][0] = 1.0;
            for (int i = 0; i < m_t + 1; i++)
            {
                for (int j = 0; j < m_v + 1; j++)
                {
                    if ((i > 1) || (j > 1))
                    {
                        m_af[i][j] = 0;
                    }
                    // deletion alignments
                    if (i > 0)
                    {
                        double prob = m_p[x[i]][VOCABULARY];
                        if (prob == 0)
                        {
                            prob = DEFAULT_PROB;
                        }
                        m_af[i][j] += prob*m_af[i - 1][j];
                    }
                    if (j > 0)
                    {
                        double prob = m_p[y[j]][VOCABULARY];
                        if (prob == 0)
                        {
                            prob = DEFAULT_PROB;
                        }
                        m_af[i][j] += prob*m_af[i][j - 1];
                    }
                    // match alignments
                    if ((i > 0) && (j > 0))
                    {
                        if (x[i] >= y[j])
                        {
                            double prob = m_p[x[i]][y[j]];
                            if (prob == 0.0)
                            {
                                prob = DEFAULT_PROB;
                            }
                            m_af[i][j] += prob*m_af[i - 1][j - 1];
                        }
                        else
                        {
                            double prob = m_p[y[j]][x[i]];
                            if (prob == 0.0)
                            {
                                prob = DEFAULT_PROB;
                            }
                            m_af[i][j] += prob*m_af[i - 1][j - 1];
                        }
                    }
                }
            }
            // get totals
            m_pTv = m_af[m_t][m_v]*m_pEnd;
        }

        private void Backward(char[] x, char[] y)
        {
            m_ab = new double[m_t + 1][];

            for (int i = 0; i < m_t + 1; i++)
            {
                m_ab[i] = new double[m_v + 1];
            }
            // initial conditions
            m_ab[m_t][m_v] = m_pEnd;
            for (int i = m_t; i > -1; i--)
            {
                for (int j = m_v; j > -1; j--)
                {
                    if ((i < m_t) || (j < m_v))
                    {
                        m_ab[i][j] = 0;
                    }
                    if (i < m_t)
                    {
                        double prob = m_p[x[i + 1]][VOCABULARY];
                        if (prob == 0.0)
                        {
                            prob = DEFAULT_PROB;
                        }
                        m_ab[i][j] += prob*m_ab[i + 1][j];
                    }
                    if (j < m_v)
                    {
                        double prob = m_p[y[j + 1]][VOCABULARY];
                        if (prob == 0.0)
                        {
                            prob = DEFAULT_PROB;
                        }
                        m_ab[i][j] += prob*m_ab[i][j + 1];
                    }
                    if ((i < m_t) && (j < m_v))
                    {
                        if (x[i] >= y[j])
                        {
                            double prob = m_p[x[i + 1]][y[j + 1]];
                            if (prob == 0.0)
                            {
                                prob = DEFAULT_PROB;
                            }
                            m_ab[i][j] += prob*m_ab[i + 1][j + 1];
                        }
                        else
                        {
                            double prob = m_p[y[j + 1]][x[i + 1]];
                            if (prob == 0.0)
                            {
                                prob = DEFAULT_PROB;
                            }
                            m_ab[i][j] += prob*m_ab[i + 1][j + 1];
                        }
                    }
                }
            }
            // get totals
            //pTV=Mb[0][0];
        }

        private void ExpectationMaximization()
        {
            //initial probabilities values
            m_pEnd = 1.0/4.0;
            m_pMatch = 1.0/4.0;
            m_pUnmatch = 1.0/4.0;
            m_pDeletion = 1.0/4.0;
            // p = probability of character i with character j
            // the second array-term includes the space "e" prbability
            m_p = new double[VOCABULARY][];
            for (int i = 0; i < VOCABULARY; i++)
            {
                m_p[i] = new double[VOCABULARY + 1];
            }
            GetInitialProbabilities();
            // token vector contain the train data
            char[][][] charArray = GetCharPairs0();
            // read the training data and iterate E-M
            for (int currentIteration = 0;
                 currentIteration < m_iterations + 1;
                 currentIteration++)
            {
                m_pTv = 0;
                // initial expected values
                m_eend = 0;
                m_ematchSubs = 0;
                m_edeletion = 0;
                m_ematch = 0;
                m_eunmatch = 0;
                m_e = new double[VOCABULARY][];
                for (int i = 0; i < VOCABULARY; i++)
                {
                    m_e[i] = new double[VOCABULARY + 1];
                }
                for (int i = 0; i < charArray.Length; i++)
                {
                    char[] x = charArray[i][0];
                    char[] y = charArray[i][1];
                    double frequency = m_frequencyArray[i];
                    m_t = x.Length - 1;
                    m_v = y.Length - 1;
                    Expectation(x, y, frequency);
                }
                Maximization();
                Console.WriteLine("EM iteration: " + currentIteration + " of " +
                                  m_iterations);
                if (m_converge)
                {
                    break;
                }
            }
        }

        private void ToStringProbabilities()
        {
            // display probabilities
            var outputStringBuffer = new StringBuilder();
            outputStringBuffer.Append("ch_1\tch_2\tprob\tmatch\n");
            for (int a = 0; a < VOCABULARY; a++)
            {
                for (int b = 0; b < VOCABULARY + 1; b++)
                {
                    if (m_p[a][b] > 0)
                    {
                        String outStr;
                        if (b < VOCABULARY)
                        {
                            outStr = (char) a + "\t" + (char) b + "\t" + m_p[a][b] + "\t" +
                                     (a == b ? "1" : "") + "\n";
                        }
                        else
                        {
                            outStr = (char) a + "\t" + " " + "\t" + m_p[a][b] + "\t\n";
                        }
                        outputStringBuffer.Append(outStr);
                    }
                }
            }
        }

        private static char[][][] GetCharPairs0()
        {
            //char[][][] charArray = new char[tokenVector.size()][2][];
            //frequencyArray = new double[tokenVector.size()];
            //Iterator it = tokenVector.iterator();
            //int counter = 0;
            //while (it.hasNext()) {
            //  TokenObject actualTokenObject = (TokenObject) it.next();
            //  String s = actualTokenObject.getS();
            //  String t = actualTokenObject.getT();
            //  frequencyArray[counter] = ( (double) actualTokenObject.getFrequency());
            //  char[] x = getCharArray(s);
            //  char[] y = getCharArray(t);
            //  charArray[counter][0] = x;
            //  charArray[counter][1] = y;
            //  counter++;
            //}
            //return charArray;
            return null;
        }

        private void GetInitialProbabilities()
        {
            // get equal initial probabilities
            const double count = ((VOCABULARY*(VOCABULARY - 1))/2.0) + (2*VOCABULARY);
            const double probSubstitution = 1.0/count;
            double totalProb = 0;
            for (int i = 0; i < VOCABULARY; i++)
            {
                for (int j = 0; j < i + 1; j++)
                {
                    m_p[i][j] = probSubstitution;
                    totalProb += probSubstitution;
                }
            }
            // get the space initial probabilities
            for (int i = 0; i < VOCABULARY; i++)
            {
                m_p[i][VOCABULARY] = probSubstitution;
                totalProb += probSubstitution;
            }
            Console.WriteLine("totalProb " + totalProb);
        }

        private char[] GetCharArray(String str)
        {
            char[] stringChar = str.ToCharArray();
            var array = new char[stringChar.Length + 1];
            for (int i = 0; i < stringChar.Length; i++)
            {
                array[i + 1] = stringChar[i];
            }
            return array;
        }

        private void Expectation(char[] x, char[] y, double frequency)
        {
            Forward(x, y);
            Backward(x, y);
            if (m_pTv <= 0.0)
            {
                Console.WriteLine("pTV " + m_pTv);
                String sToken = "", tToken = "";
                for (int i = 0; i < x.Length; i++)
                {
                    sToken += x[i];
                }
                for (int i = 0; i < y.Length; i++)
                {
                    tToken += y[i];
                }
                Console.WriteLine(sToken + "-" + tToken);
            }
            m_eend++;
            for (int i = 1; i < m_t + 1; i++)
            {
                for (int j = 1; j < m_v + 1; j++)
                {
                    // expectation for matching
                    double tmp;
                    if (x[i] >= y[j])
                    {
                        tmp = (m_af[i - 1][j - 1]*m_p[x[i]][y[j]]*m_ab[i][j])/m_pTv;
                    }
                    else
                    {
                        tmp = (m_af[i - 1][j - 1]*m_p[y[j]][x[i]]*m_ab[i][j])/m_pTv;
                    }
                    tmp = tmp*frequency;
                    m_ematchSubs += tmp;

                    if (x[i] >= y[j])
                    {
                        m_e[x[i]][y[j]] += tmp;
                    }
                    else
                    {
                        m_e[y[j]][x[i]] += tmp;
                    }
                    if (x[i] == y[j])
                    {
                        m_ematch += tmp;
                    }
                    else
                    {
                        m_eunmatch += tmp;
                    }
                    // expectation for insertion and deletion
                    tmp = (m_af[i][j - 1]*m_p[y[j]][VOCABULARY]*m_ab[i][j])/m_pTv;
                    m_edeletion += tmp;
                    m_e[y[j]][VOCABULARY] += tmp;

                    tmp = (m_af[i - 1][j]*m_p[x[i]][VOCABULARY]*m_ab[i][j])/m_pTv;
                    m_edeletion += tmp;
                    m_e[x[i]][VOCABULARY] += tmp;
                }
            }
        }

        private void Maximization()
        {
            double n = 0;
            double dblN = m_eend + m_ematchSubs + m_edeletion;
            m_pEnd = m_eend/dblN;
            m_pMatch = m_ematch/dblN;
            m_pUnmatch = m_eunmatch/dblN;
            m_pDeletion = m_edeletion/dblN;
            for (int a = 0; a < VOCABULARY; a++)
            {
                for (int b = 0; b < a + 1; b++)
                {
                    n += m_e[a][b];
                }
            }
            // counts for the spaces
            for (int a = 0; a < VOCABULARY; a++)
            {
                n += m_e[a][VOCABULARY];
            }

            for (int a = 0; a < VOCABULARY; a++)
            {
                for (int b = 0; b < a + 1; b++)
                {
                    m_p[a][b] = m_e[a][b]/n;
                }
            }
            // maximization of the spaces
            for (int a = 0; a < VOCABULARY; a++)
            {
                m_p[a][VOCABULARY] = m_e[a][VOCABULARY]/n;
            }
            if (Math.Abs(m_lastConvergence - m_pEnd) <= CONVERGENCE)
            {
                m_converge = true;
            }
            Console.WriteLine("convergence: " + Math.Abs(m_lastConvergence - m_pEnd) +
                              "\n");
            m_lastConvergence = m_pEnd;
        }

        #endregion
    }
}
