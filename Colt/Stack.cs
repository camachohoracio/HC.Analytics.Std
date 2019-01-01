#region

using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Colt
{
    /*
    Copyright � 1999 CERN - European Organization for Nuclear Research.
    Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
    is hereby granted without fee, provided that the above copyright notice appear in all copies and 
    that both that copyright notice and this permission notice appear in supporting documentation. 
    CERN makes no representations about the suitability of this software for any purpose. 
    It is provided "as is" without expressed or implied warranty.
    */
    //package cern.jet.random;

    /**
     * Not yet commented.
     */

    internal class Stack
    {
        private readonly int N; /* Max number of elts on stack */
        private readonly int[] v; /* array of values on the stack */
        private int i; /* index of top of stack */
/**
 * Constructs a new stack with the given capacity.
 */

        public Stack(int capacity)
        {
            N = capacity;
            i = -1; // indicates stack is empty
            v = new int[N];
/*
static stack_t *
new_stack(int N) {
	stack_t *s;
	s = (stack_t *)malloc(sizeof(stack_t));
	s->N = N;
	s->i = -1;                  // indicates stack is empty 
	s->v = (int *)malloc(sizeof(int)*N);
	return s;
}
static void
push_stack(stack_t *s, int v)
{
	s->i += 1;
	if ((s->i) >= (s->N)) {
		fprintf(stderr,"Cannot push stack!\n");
		exit(0);                // fatal!! 
	}
	(s->v)[s->i] = v;
}
static int pop_stack(stack_t *s)
{
	if ((s->i) < 0) {
		fprintf(stderr,"Cannot pop stack!\n");
		exit(0);
	}
	s->i -= 1;
	return ((s->v)[s->i + 1]);
}
static inline int size_stack(const stack_t *s)
{
	return s->i + 1;
}
static void free_stack(stack_t *s)
{
	free((char *)(s->v));
	free((char *)s);
}
*/
        }

/**
 * Returns the topmost element.
 */

        public int pop()
        {
            if (i < 0)
            {
                throw new HCException("Cannot pop stack!");
            }
            i--;
            return v[i + 1];
        }

/**
 * Places the given value on top of the stack.
 */

        public void push(int value)
        {
            i++;
            if (i >= N)
            {
                throw new HCException("Cannot push stack!");
            }
            v[i] = value;
        }

/**
 * Returns the number of elements contained.
 */

        public int Size()
        {
            return i + 1;
        }
    }
}
