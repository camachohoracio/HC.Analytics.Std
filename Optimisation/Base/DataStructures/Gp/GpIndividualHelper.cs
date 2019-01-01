using System;
using HC.Core.Logging;

namespace HC.Analytics.Optimisation.Base.DataStructures.Gp
{
    public class GpIndividualHelper
    {
        #region Members

        private int m_intCount;

        #endregion

        public static AbstractGpNode ReturnNodeNumber(
            int numb,
            AbstractGpNode gpNodes)
        {
            return (new GpIndividualHelper()).ReturnNodeNumber1(numb, gpNodes);
        }

        private AbstractGpNode ReturnNodeNumber1(
            int numb,
            AbstractGpNode gpNodes)
        {
            m_intCount = 0;
            return ReturnNodeNumber2(numb, gpNodes);
        }

        private AbstractGpNode ReturnNodeNumber2(
            int numb,
            AbstractGpNode gpNodes)
        {
            m_intCount++;
            if (m_intCount == numb)
            {
                return gpNodes;
            }
            if (gpNodes.IsOperatorNode)
            {
                var children = ((GpOperatorNode) gpNodes).ChildrenArr;
                for (var i = 0; i < children.Length; i++)
                {
                    //
                    // recursive call
                    //
                    var temp = ReturnNodeNumber2(numb, children[i]);
                    if (temp != null)
                    {
                        return temp;
                    }
                }
            }
            else
            {
                return null;
            }
            return null;
        }

        public static int CountOperatorNodes(
            AbstractGpNode gpNodes)
        {
            try
            {
                if (gpNodes == null)
                {
                    return 0;
                }
                if (gpNodes.IsOperatorNode)
                {
                    var intCount = 1;
                    var children = ((GpOperatorNode)gpNodes).ChildrenArr;
                    if (children != null)
                    {
                        for (var i = 0; i < children.Length; i++)
                        {
                            intCount += CountNodes(((GpOperatorNode)gpNodes).ChildrenArr[i]);
                        }
                    }
                    return intCount;
                }
                return 0;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }


        public static int CountNodes(
            AbstractGpNode gpNodes)
        {
            try
            {
                if (gpNodes == null)
                {
                    return 0;
                }
                if (gpNodes.IsOperatorNode)
                {
                    var intCount = 1;
                    var children = ((GpOperatorNode) gpNodes).ChildrenArr;
                    if (children != null)
                    {
                        for (var i = 0; i < children.Length; i++)
                        {
                            intCount += CountNodes(((GpOperatorNode) gpNodes).ChildrenArr[i]);
                        }
                    }
                    return intCount;
                }
                return 1;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return 1;
        }
    }
}
