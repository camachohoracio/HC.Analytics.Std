using System;

namespace HC.Analytics.MachineLearning.Svm
{
    public class SvmCache
    {
        private int l;
        private int size;
        private class head_t
        {
            public head_t prev, next;	// a cicular list
            public float[] data;
            public int len;		// data[0,len) is cached in this entry
        }
        private head_t[] head;
        private head_t lru_head;

        public SvmCache(int l_, int size_)
        {
            l = l_;
            size = size_;
            head = new head_t[l];
            for (int i = 0; i < l; i++) head[i] = new head_t();
            size /= 4;
            size -= l * (16 / 4);	// sizeof(head_t) == 16
            size = Math.Max(size, 2 * l);  // cache must be large enough for two columns
            lru_head = new head_t();
            lru_head.next = lru_head.prev = lru_head;
        }

        private void LruDelete(head_t h)
        {
            // delete from current location
            h.prev.next = h.next;
            h.next.prev = h.prev;
        }

        private void LruInsert(head_t h)
        {
            // insert to last position
            h.next = lru_head;
            h.prev = lru_head.prev;
            h.prev.next = h;
            h.next.prev = h;
        }

        // request data [0,len)
        // return some position p where [p,len) need to be filled
        // (p >= len if nothing needs to be filled)
        // java: simulate pointer using single-element array
        public int GetData(int index, float[][] data, int len)
        {
            head_t h = head[index];
            if (h.len > 0) LruDelete(h);
            int more = len - h.len;

            if (more > 0)
            {
                // free old space
                while (size < more)
                {
                    head_t old = lru_head.next;
                    LruDelete(old);
                    size += old.len;
                    old.data = null;
                    old.len = 0;
                }

                // allocate new space
                float[] new_data = new float[len];
                if (h.data != null) Array.Copy(h.data, 0, new_data, 0, h.len);
                h.data = new_data;
                size -= more;
                do { int _ = h.len; h.len = len; len = _; } while (false);
            }

            LruInsert(h);
            data[0] = h.data;
            return len;
        }

        public void SwapIndex(int i, int j)
        {
            if (i == j) return;

            if (head[i].len > 0) LruDelete(head[i]);
            if (head[j].len > 0) LruDelete(head[j]);
            do { float[] _ = head[i].data; head[i].data = head[j].data; head[j].data = _; } while (false);
            do { int _ = head[i].len; head[i].len = head[j].len; head[j].len = _; } while (false);
            if (head[i].len > 0) LruInsert(head[i]);
            if (head[j].len > 0) LruInsert(head[j]);

            if (i > j) do { int _ = i; i = j; j = _; } while (false);
            for (head_t h = lru_head.next; h != lru_head; h = h.next)
            {
                if (h.len > i)
                {
                    if (h.len > j)
                        do { float _ = h.data[i]; h.data[i] = h.data[j]; h.data[j] = _; } while (false);
                    else
                    {
                        // give up
                        LruDelete(h);
                        size += h.len;
                        h.data = null;
                        h.len = 0;
                    }
                }
            }
        }
    }
}

