namespace HC.Analytics.MachineLearning.Svm.Kernels
{
    public class SvrQ : AbstractKernel
    {
        private int l;
        private SvmCache cache;
        private int[] sign;
        private int[] index;
        private int next_buffer;
        private float[][] buffer;
        private float[] QD;

        public SvrQ(SvmProblem prob, SvmParameters param) :
            base(prob.l, prob.x, param)
        {
            l = prob.l;
            cache = new SvmCache(l, (int)(param.cache_size * (1 << 20)));
            QD = new float[2 * l];
            sign = new int[2 * l];
            index = new int[2 * l];
            for (int k = 0; k < l; k++)
            {
                sign[k] = 1;
                sign[k + l] = -1;
                index[k] = k;
                index[k + l] = k;
                QD[k] = (float)KernelFunction(k, k);
                QD[k + l] = QD[k];
            }
            buffer = new float[2][];
            for (int i = 0; i < 2; i++)
            {
                buffer[i] = new float[2 * l];
            }
            next_buffer = 0;
        }

        public override void SwapIndex(int i, int j)
        {
            do { int _ = sign[i]; sign[i] = sign[j]; sign[j] = _; } while (false);
            do { int _ = index[i]; index[i] = index[j]; index[j] = _; } while (false);
            do { float _ = QD[i]; QD[i] = QD[j]; QD[j] = _; } while (false);
        }

        public override float[] GetQ(int i, int len)
        {
            float[][] data = new float[1][];
            int real_i = index[i];
            if (cache.GetData(real_i, data, l) < l)
            {
                for (int j = 0; j < l; j++)
                {
                    data[0][j] = (float)KernelFunction(real_i, j);
                }
            }

            // reorder and copy
            float[] buf = buffer[next_buffer];
            next_buffer = 1 - next_buffer;
            int si = sign[i];
            for (int j = 0; j < len; j++)
            {
                buf[j] = si * sign[j] * data[0][index[j]];
            }

            return buf;
        }

        public override float[] GetQd()
        {
            return QD;
        }
    }
}

