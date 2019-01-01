#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleFunctionSqrt : DoubleFunction
    {
        #region DoubleFunction Members

        public double Apply(double a)
        {
            //object[] users = null;

            //Array.Sort(users, delegate(Object user1, Object user2)
            //{
            //    return user1.ToString().CompareTo(user2.ToString());
            //});

            return Math.Sqrt(a);
        }

        #endregion
    }
}
