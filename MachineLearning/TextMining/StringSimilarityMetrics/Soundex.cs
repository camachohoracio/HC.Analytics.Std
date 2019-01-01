#region

using HC.Core.Text;

#endregion

namespace HC.Analytics.MachineLearning.TextMining.StringSimilarityMetrics
{
    public class Soundex
    {
        private static int m_soundexlength = 6;
        private readonly TagLinkTokenCheap m_stringMetric;

        public Soundex()
        {
            m_stringMetric = new TagLinkTokenCheap();
        }

        public double GetStringMetric(
            TokenWrapper t,
            TokenWrapper u)
        {
            var code1 = new TokenWrapper(CalcSoundEx(t.Token));
            var code2 = new TokenWrapper(CalcSoundEx(u.Token));
            return m_stringMetric.GetRawMetric(code1, code2);
        }

        public static string CalcSoundEx(string wordString)
        {
            //ensure soundexLen is in a valid range
            if (m_soundexlength > 10)
            {
                m_soundexlength = 10;
            }
            if (m_soundexlength < 4)
            {
                m_soundexlength = 4;
            }

            //check for empty input
            if (wordString.Length == 0)
            {
                return ("");
            }

            //remove case
            wordString = wordString.ToUpper();

            /* Clean and tidy
        */
            string wordStr = wordString;
            wordStr = wordStr.Replace("[^A-Z]", " "); // rpl non-chars w space
            wordStr = wordStr.Replace("\\s+", ""); // remove spaces

            //check for empty input again the previous clean and tidy could of shrunk it to zero.
            if (wordStr.Length == 0)
            {
                return ("");
            }

            /* The above improvements
         * may change this first letter
        */
            char firstLetter = wordStr[0];

            // uses the assumption that enough valid characters are in the first 4 times the soundex required length
            if (wordStr.Length > (m_soundexlength*4) + 1)
            {
                wordStr = "-" + wordStr.Substring(1, m_soundexlength*4);
            }
            else
            {
                wordStr = "-" + wordStr.Substring(1);
            }
            // Begin Classic SoundEx
            /*
        1) B,P,F,V
        2) C,S,K,G,J,Q,X,Z
        3) D,T
        4) L
        5) M,N
        6) R
        */
            wordStr = wordStr.Replace("[AEIOUWH]", "0");
            wordStr = wordStr.Replace("[BPFV]", "1");
            wordStr = wordStr.Replace("[CSKGJQXZ]", "2");
            wordStr = wordStr.Replace("[DT]", "3");
            wordStr = wordStr.Replace("[L]", "4");
            wordStr = wordStr.Replace("[MN]", "5");
            wordStr = wordStr.Replace("[R]", "6");

            // Remove extra equal adjacent digits
            int wsLen = wordStr.Length;
            char lastChar = '-';
            string tmpStr = "-";
            for (int i = 1; i < wsLen; i++)
            {
                char curChar = wordStr[i];
                if (curChar != lastChar)
                {
                    tmpStr += curChar;
                    lastChar = curChar;
                }
            }
            wordStr = tmpStr;
            wordStr = wordStr.Substring(1); /* Drop first letter code   */
            wordStr = wordStr.Replace("0", ""); /* remove zeros             */
            wordStr += "000000000000000000"; /* pad with zeros on right  */
            wordStr = firstLetter + "-" + wordStr; /* Add first letter of word */
            wordStr = wordStr.Substring(0, m_soundexlength); /* size to taste     */
            return (wordStr);
        }
    }
}
