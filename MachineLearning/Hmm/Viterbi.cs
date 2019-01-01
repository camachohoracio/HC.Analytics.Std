using System;
using System.Text;

namespace HC.Analytics.MachineLearning.Hmm
{
    public class Viterbi
    {
        private const int M_INT_VOCABULARY = 255;
        private const double M_DBL_SCALE_FACTOR = 10000.0;
        private double m_dblMatched;
        private double m_dblPInsertion;
        private double m_dblPMatch;
        private double m_dblPUnmatch;
        private int m_intSlen;
        private int m_intTlen;
        private double[] m_maxP;
        private double[][] m_p;
        private double[][] m_scoreArray;

        public Viterbi()
        {
            var transducer = new Transducer(50000, null);
            m_p = transducer.GetP();

            ScaleProb();
            GetMaxProbabilities();
        }

        private void ScaleProb()
        {
            m_dblPMatch = 1;
            m_dblPUnmatch = 1;
            m_dblPInsertion = 1;
            Console.WriteLine("pMatch " + m_dblPMatch);
            Console.WriteLine("pUnmatch " + m_dblPUnmatch);
            Console.WriteLine("pInsertion " + m_dblPInsertion);
            // scale individual probabilities
            for (int i = 0; i < m_p.Length; i++)
            {
                for (int j = 0; j < i + 1; j++)
                {
                    if (m_p[i][j] >= 0.000001)
                    {
                        m_p[i][j] = -Math.Log(1.0 - m_p[i][j]);
                    }
                    else
                    {
                        m_p[i][j] = 0.0;
                    }
                }
            }
            // scale the spaces
            for (int i = 0; i < m_p.Length; i++)
            {
                if (m_p[i][M_INT_VOCABULARY] >= 0.000001)
                {
                    m_p[i][M_INT_VOCABULARY] = -Math.Log(1.0 - m_p[i][M_INT_VOCABULARY]);
                }
                else
                {
                    m_p[i][M_INT_VOCABULARY] = 0.0;
                }
            }

            // obtain the minimum match probability
            double minProb = 2;
            for (int i = 0; i < M_INT_VOCABULARY; i++)
            {
                if ((m_p[i][i] < minProb) && (m_p[i][i] > 0))
                {
                    minProb = m_p[i][i];
                }
            }

            // scale the match characters
            for (int i = 0; i < M_INT_VOCABULARY; i++)
            {
                if (m_p[i][i] > 0)
                {
                    m_p[i][i] = 1;
                }
            }
            // scale the other characters
            for (int i = 0; i < M_INT_VOCABULARY; i++)
            {
                for (int j = 0; j < i + 1; j++)
                {
                    if (i != j)
                    {
                        if (m_p[i][j] > 0)
                        {
                            m_p[i][j] = (m_p[i][j]/minProb)*M_DBL_SCALE_FACTOR;
                        }
                        if (m_p[i][j] > 1)
                        {
                            m_p[i][j] = 1;
                        }
                    }
                }
            }
            // scale the space characters
            for (int i = 0; i < M_INT_VOCABULARY; i++)
            {
                if (m_p[i][M_INT_VOCABULARY] > 0)
                {
                    //p[i][vocabulary]=(p[i][vocabulary]/minProb)*scaleFactor;
                    m_p[i][M_INT_VOCABULARY] = 0;
                }
                if (m_p[i][M_INT_VOCABULARY] > 1)
                {
                    m_p[i][M_INT_VOCABULARY] = 1;
                }
            }

            // display probabilities
            var outputStringBuffer = new StringBuilder();
            outputStringBuffer.Append("ch_1\tch_2\tprob\tmatch\n");
            for (int a = 0; a < M_INT_VOCABULARY; a++)
            {
                for (int b = 0; b < M_INT_VOCABULARY + 1; b++)
                {
                    if (m_p[a][b] > 0)
                    {
                        String outStr;
                        if (b < M_INT_VOCABULARY)
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

        private void GetInitialProbabilities()
        {
            m_p = new double[M_INT_VOCABULARY][];
            for (int i = 0; i < M_INT_VOCABULARY; i++)
            {
                m_p[i] = new double[M_INT_VOCABULARY + 1];
            }
            // get equal initial probabilities
            const double probSubstitution = 1.0/(M_INT_VOCABULARY);
            for (int i = 0; i < M_INT_VOCABULARY; i++)
            {
                for (int j = 0; j < M_INT_VOCABULARY; j++)
                {
                    if (i == j)
                    {
                        m_p[i][j] = probSubstitution;
                    }
                }
            }
        }

        private void GetMaxProbabilities()
        {
            m_maxP = new double[M_INT_VOCABULARY];
            for (int i = 0; i < M_INT_VOCABULARY; i++)
            {
                double max = -1;
                for (int j = 0; j < M_INT_VOCABULARY + 1; j++)
                {
                    if (m_p[i][j] > max)
                    {
                        max = m_p[i][j];
                    }
                }
                m_maxP[i] = max;
            }
        }

        // Methods required to fit into second string
        public String ExplainScore(object s, object t)
        {
            return "";
        }

        public String ExplainStringMetric(String s, String t)
        {
            return "";
        }


        private new string ToString()
        {
            return "[Vierbi_Learned]";
        }
    }
}
