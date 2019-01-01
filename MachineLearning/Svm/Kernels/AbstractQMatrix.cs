namespace HC.Analytics.MachineLearning.Svm.Kernels
{
    //
    // Kernel evaluation
    //
    // the static method k_function is for doing single kernel evaluation
    // the constructor of Kernel prepares to calculate the l*l kernel matrix
    // the member function get_Q is for getting one column from the Q Matrix
    //
    public abstract class AbstractQMatrix
    {
        public abstract float[] GetQ(int column, int len);
        public abstract float[] GetQd();
        public abstract void SwapIndex(int i, int j);
    }
}

