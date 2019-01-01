namespace HC.Analytics.Optimisation.Base.Operators.MultiObjective
{
    public class RankObj
    {
        public double Rank { get; set; }
        public int Index { get; set; }
        public bool IsValid { get; set; }

        public double[] Objs { get; set; }
    }
}

