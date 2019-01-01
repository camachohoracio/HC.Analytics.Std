#region

using HC.Core.Helpers;

#endregion

namespace HC.Analytics.MachineLearning.TextMining.PreProcessing
{
    /* author:   Fotis Lazarinis (actually I translated from C to Java)
       date:     June 1997
       address:  Psilovraxou 12, Agrinio, 30100

       comments: Compile it, import the Porter class into you program and create an instance.
             Then use the stripAffixes method of this method which takes a string as 
                 input and returns the stem of this string again as a string.
    */

    internal class NewString
    {
        public string str;

        public NewString()
        {
            str = "";
        }
    }

    /**
     * The Porter stemmer for reducing words to their base stem form.
     *
     * @author Fotis Lazarinis
     */

    public class PorterStemer
    {
        private string Clean(string str)
        {
            if(string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            int last = str.Length;

            char ch = str[0];
            string temp = "";
            for (int i = 0; i < last; i++)
            {
                if (char.IsLetterOrDigit(str[i]))
                    temp += str[i];
            }

            return temp;
        } //clean

        private bool hasSuffix(string word, string suffix, NewString stem)
        {
            string tmp = "";

            if (word.Length <= suffix.Length)
                return false;
            if (suffix.Length > 1)
                if (word[word.Length - 2] != suffix[suffix.Length - 2])
                    return false;

            stem.str = "";

            for (int i = 0; i < word.Length - suffix.Length; i++)
                stem.str += word[i];
            tmp = stem.str;

            for (int i = 0; i < suffix.Length; i++)
                tmp += suffix[i];

            if (tmp.CompareTo(word) == 0)
                return true;
            else
                return false;
        }

        private bool vowel(char ch, char prev)
        {
            switch (ch)
            {
                case 'a':
                case 'e':
                case 'i':
                case 'o':
                case 'u':
                    return true;
                case 'y':
                    {
                        switch (prev)
                        {
                            case 'a':
                            case 'e':
                            case 'i':
                            case 'o':
                            case 'u':
                                return false;

                            default:
                                return true;
                        }
                    }

                default:
                    return false;
            }
        }

        private int measure(string stem)
        {
            int i = 0, count = 0;
            int Length = stem.Length;

            while (i < Length)
            {
                for (; i < Length; i++)
                {
                    if (i > 0)
                    {
                        if (vowel(stem[i], stem[i - 1]))
                            break;
                    }
                    else
                    {
                        if (vowel(stem[i], 'a'))
                            break;
                    }
                }

                for (i++; i < Length; i++)
                {
                    if (i > 0)
                    {
                        if (!vowel(stem[i], stem[i - 1]))
                            break;
                    }
                    else
                    {
                        if (!vowel(stem[i], '?'))
                            break;
                    }
                }
                if (i < Length)
                {
                    count++;
                    i++;
                }
            } //while

            return (count);
        }

        private bool containsVowel(string word)
        {
            for (int i = 0; i < word.Length; i++)
                if (i > 0)
                {
                    if (vowel(word[i], word[i - 1]))
                        return true;
                }
                else
                {
                    if (vowel(word[0], 'a'))
                        return true;
                }

            return false;
        }

        private bool cvc(string str)
        {
            int Length = str.Length;

            if (Length < 3)
                return false;

            if ((!vowel(str[Length - 1], str[Length - 2]))
                && (str[Length - 1] != 'w') && (str[Length - 1] != 'x') && (str[Length - 1] != 'y')
                && (vowel(str[Length - 2], str[Length - 3])))
            {
                if (Length == 3)
                {
                    if (!vowel(str[0], '?'))
                        return true;
                    else
                        return false;
                }
                else
                {
                    if (!vowel(str[Length - 3], str[Length - 4]))
                        return true;
                    else
                        return false;
                }
            }

            return false;
        }

        private string step1(string str)
        {
            NewString stem = new NewString();

            if (str[str.Length - 1] == 's')
            {
                if ((hasSuffix(str, "sses", stem)) || (hasSuffix(str, "ies", stem)))
                {
                    string tmp = "";
                    for (int i = 0; i < str.Length - 2; i++)
                        tmp += str[i];
                    str = tmp;
                }
                else
                {
                    if ((str.Length == 1) && (str[str.Length - 1] == 's'))
                    {
                        str = "";
                        return str;
                    }
                    if (str[str.Length - 2] != 's')
                    {
                        string tmp = "";
                        for (int i = 0; i < str.Length - 1; i++)
                            tmp += str[i];
                        str = tmp;
                    }
                }
            }

            if (hasSuffix(str, "eed", stem))
            {
                if (measure(stem.str) > 0)
                {
                    string tmp = "";
                    for (int i = 0; i < str.Length - 1; i++)
                        tmp += str[i];
                    str = tmp;
                }
            }
            else
            {
                if ((hasSuffix(str, "ed", stem)) || (hasSuffix(str, "ing", stem)))
                {
                    if (containsVowel(stem.str))
                    {
                        string tmp = "";
                        for (int i = 0; i < stem.str.Length; i++)
                            tmp += str[i];
                        str = tmp;
                        if (str.Length == 1)
                            return str;

                        if ((hasSuffix(str, "at", stem)) || (hasSuffix(str, "bl", stem)) || (hasSuffix(str, "iz", stem)))
                        {
                            str += "e";
                        }
                        else
                        {
                            int Length = str.Length;
                            if ((str[Length - 1] == str[Length - 2])
                                && (str[Length - 1] != 'l') && (str[Length - 1] != 's') && (str[Length - 1] != 'z'))
                            {
                                tmp = "";
                                for (int i = 0; i < str.Length - 1; i++)
                                    tmp += str[i];
                                str = tmp;
                            }
                            else if (measure(str) == 1)
                            {
                                if (cvc(str))
                                    str += "e";
                            }
                        }
                    }
                }
            }

            if (hasSuffix(str, "y", stem))
                if (containsVowel(stem.str))
                {
                    string tmp = "";
                    for (int i = 0; i < str.Length - 1; i++)
                        tmp += str[i];
                    str = tmp + "i";
                }
            return str;
        }

        private string step2(string str)
        {
            string[,] suffixes = {
                                     {"ational", "ate"},
                                     {"tional", "tion"},
                                     {"enci", "ence"},
                                     {"anci", "ance"},
                                     {"izer", "ize"},
                                     {"iser", "ize"},
                                     {"abli", "able"},
                                     {"alli", "al"},
                                     {"entli", "ent"},
                                     {"eli", "e"},
                                     {"ousli", "ous"},
                                     {"ization", "ize"},
                                     {"isation", "ize"},
                                     {"ation", "ate"},
                                     {"ator", "ate"},
                                     {"alism", "al"},
                                     {"iveness", "ive"},
                                     {"fulness", "ful"},
                                     {"ousness", "ous"},
                                     {"aliti", "al"},
                                     {"iviti", "ive"},
                                     {"biliti", "ble"}
                                 };
            NewString stem = new NewString();


            for (int index = 0; index < suffixes.GetLength(0); index++)
            {
                if (hasSuffix(str, suffixes[index, 0], stem))
                {
                    if (measure(stem.str) > 0)
                    {
                        str = stem.str + suffixes[index, 1];
                        return str;
                    }
                }
            }

            return str;
        }

        private string step3(string str)
        {
            string[,] suffixes = {
                                     {"icate", "ic"},
                                     {"ative", ""},
                                     {"alize", "al"},
                                     {"alise", "al"},
                                     {"iciti", "ic"},
                                     {"ical", "ic"},
                                     {"ful", ""},
                                     {"ness", ""}
                                 };
            NewString stem = new NewString();

            for (int index = 0; index < suffixes.GetLength(0); index++)
            {
                if (hasSuffix(str, suffixes[index, 0], stem))
                    if (measure(stem.str) > 0)
                    {
                        str = stem.str + suffixes[index, 1];
                        return str;
                    }
            }
            return str;
        }

        private string step4(string str)
        {
            string[] suffixes = {
                                    "al", "ance", "ence", "er", "ic", "able", "ible", "ant", "ement", "ment", "ent",
                                    "sion", "tion",
                                    "ou", "ism", "ate", "iti", "ous", "ive", "ize", "ise"
                                };

            NewString stem = new NewString();

            for (int index = 0; index < suffixes.Length; index++)
            {
                if (hasSuffix(str, suffixes[index], stem))
                {
                    if (measure(stem.str) > 1)
                    {
                        str = stem.str;
                        return str;
                    }
                }
            }
            return str;
        }

        private string step5(string str)
        {
            if (str[str.Length - 1] == 'e')
            {
                if (measure(str) > 1)
                {
/* measure(str)==measure(stem) if ends in vowel */
                    string tmp = "";
                    for (int i = 0; i < str.Length - 1; i++)
                        tmp += str[i];
                    str = tmp;
                }
                else if (measure(str) == 1)
                {
                    string stem = "";
                    for (int i = 0; i < str.Length - 1; i++)
                        stem += str[i];

                    if (!cvc(stem))
                        str = stem;
                }
            }

            if (str.Length == 1)
                return str;
            if ((str[str.Length - 1] == 'l') && (str[str.Length - 2] == 'l') && (measure(str) > 1))
                if (measure(str) > 1)
                {
/* measure(str)==measure(stem) if ends in vowel */
                    string tmp = "";
                    for (int i = 0; i < str.Length - 1; i++)
                        tmp += str[i];
                    str = tmp;
                }
            return str;
        }

        private string stripPrefixes(string str)
        {
            string[] prefixes = {"kilo", "micro", "milli", "intra", "ultra", "mega", "nano", "pico", "pseudo"};

            int last = prefixes.Length;
            for (int i = 0; i < last; i++)
            {
                if (str.StartsWith(prefixes[i]))
                {
                    string temp = "";
                    for (int j = 0; j < str.Length - prefixes[i].Length; j++)
                        temp += str[j + prefixes[i].Length];
                    return temp;
                }
            }

            return str;
        }


        private string stripSuffixes(string str)
        {
            str = step1(str);
            if (str.Length >= 1)
                str = step2(str);
            if (str.Length >= 1)
                str = step3(str);
            if (str.Length >= 1)
                str = step4(str);
            if (str.Length >= 1)
                str = step5(str);

            return str;
        }

        /**
         * Takes a string as input and returns its stem as a string.
         */

        public string DoStem(string str)
        {
            str = str.ToLower();
            str = Clean(str);

            if ((str != "") && (str.Length > 2))
            {
                str = stripPrefixes(str);

                if (str != "")
                    str = stripSuffixes(str);
            }

            return str;
        } //stripAffixes

        /**
         * For testing, print the stemmed version of a word
         */

        public static void main(string[] args)
        {
            string word = args[0];
            PorterStemer stemmer = new PorterStemer();
            string stem = stemmer.DoStem(word);
            PrintToScreen.WriteLine(stem);
        }
    } //class
}
