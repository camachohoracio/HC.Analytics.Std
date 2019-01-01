namespace HC.Analytics.Probability.Random
{
    /// <summary>
    /// Random number generator factory
    /// </summary>
    public static class RandomFactory
    {
        #region Members

        /// <summary>
        /// This object is wsed to seed new 
        /// random number generators
        /// </summary>
        private static readonly System.Random m_globalRandom = new System.Random();

        private static readonly object m_lockObject = new object();

        #endregion

        #region Public

        public static System.Random Create()
        {
            int intSeed;
            return Create(out intSeed);
        }

        /// <summary>
        /// Create new random number generator
        /// </summary>
        /// <returns>
        /// Random number generator
        /// </returns>
        public static System.Random Create(out int intSeed)
        {
            lock (m_lockObject)
            {
                intSeed = m_globalRandom.Next();
            }
            var newRandom =
                new System.Random(intSeed);
            return newRandom;
        }

        public static System.Random Create(int intSeed)
        {
            var newRandom = new System.Random(intSeed);
            return newRandom;
        }

        #endregion
    }
}
