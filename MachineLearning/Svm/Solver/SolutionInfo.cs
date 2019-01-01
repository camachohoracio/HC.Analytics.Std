namespace HC.Analytics.MachineLearning.Svm.Solver
{
    // java: information about solution except alpha,
    // because we cannot return multiple values otherwise...
    public class SolutionInfo
    {
        public double obj;
        public double rho;
        public double upper_bound_p;
        public double upper_bound_n;
        public double r;	// for Solver_NU
    }
}

