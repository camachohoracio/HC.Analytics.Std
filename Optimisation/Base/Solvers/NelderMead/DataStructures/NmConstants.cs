namespace HC.Analytics.Optimisation.Base.Solvers.NelderMead.DataStructures
{
    /// <summary>
    ///   Tye of iteration carried out by NM
    /// </summary>
    public enum NmIterationType
    {
        EXPANSION,
        CONTRACTION,
        REFLECTION,
        SRINK,
        INSIDE_CONTRACTION,
        OUTSIDE_CONTRACTION
    }

    public class NmConstants
    {
        public const int NM_INSTANCES = 2;
    }
}
