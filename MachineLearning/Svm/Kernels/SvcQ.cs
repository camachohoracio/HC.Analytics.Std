namespace HC.Analytics.MachineLearning.Svm.Kernels
{
    //
    // Q matrices for various formulations
    //
    public class SvcQ : AbstractKernel
    {
        private int[] y;
        private SvmCache cache;
        private float[] QD;

        public SvcQ(
            SvmProblem prob, 
            SvmParameters param, 
            int[] y_) :
            base(prob.l, prob.x, param)
        {
            y = (int[])y_.Clone();
            cache = new SvmCache(prob.l, (int)(param.cache_size * (1 << 20)));
            QD = new float[prob.l];
            for (int i = 0; i < prob.l; i++)
            {
                QD[i] = (float)KernelFunction(i, i);
            }
        }

        public override float[] GetQ(int i, int len)
        {
            float[][] data = new float[1][];
            int start;
            if ((start = cache.GetData(i, data, len)) < len)
            {
                for (int j = start; j < len; j++)
                    data[0][j] = (float)(y[i] * y[j] * KernelFunction(i, j));
            }
            return data[0];
        }

        public override float[] GetQd()
        {
            return QD;
        }

        public override void SwapIndex(int i, int j)
        {
            cache.SwapIndex(i, j);
            base.SwapIndex(i, j);
            do { int _ = y[i]; y[i] = y[j]; y[j] = _; } while (false);
            do { float _ = QD[i]; QD[i] = QD[j]; QD[j] = _; } while (false);
        }
    }

}

