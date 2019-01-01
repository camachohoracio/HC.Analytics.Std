#region

using System;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Colt
{
    /*
    Copyright © 1999 CERN - European Organization for Nuclear Research.
    Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
    is hereby granted without fee, provided that the above copyright notice appear in all copies and 
    that both that copyright notice and this permission notice appear in supporting documentation. 
    CERN makes no representations about the suitability of this software for any purpose. 
    It is provided "as is" without expressed or implied warranty.
    */
    ////package cern.colt;

    /**
     * A handy stopwatch for benchmarking.
     * Like a real stop watch used on ancient running tracks you can start the watch, stop it,
     * start it again, stop it again, display the elapsed time and reset the watch.
     */

    [Serializable]
    public class Timer : PersistentObject
    {
        private static long UNIT = 1000;
        private long m_baseTime;
        private long m_elapsedTime;

        /**
         * Constructs a new timer, initially not started. Use start() to start the timer.
         */

        public Timer()
        {
            reset();
        }

        /**
         * Prints the elapsed time on System.out
         * @return <tt>this</tt> (for convenience only).
         */

        public Timer display()
        {
            PrintToScreen.WriteLine(this);

            return this;
        }

        /**
         * Same as <tt>seconds()</tt>.
         */

        public float elapsedTime()
        {
            return seconds();
        }

        /**
         * Returns the elapsed time in milli seconds; does not stop the timer, if started.
         */

        public long millis()
        {
            long elapsed = m_elapsedTime;
            if (m_baseTime != 0)
            {
                // we are started
                elapsed += (long) (DateTime.MinValue - DateTime.Now).TotalMilliseconds - m_baseTime;
            }
            return elapsed;
        }

        /**
         * <tt>T = this - other</tt>; Constructs and returns a new timer which is the difference of the receiver and the other timer.
         * The new timer is not started.
         * @param other the timer to subtract.
         * @return a new timer.
         */

        public Timer minus(Timer other)
        {
            Timer copy = new Timer();
            copy.m_elapsedTime = millis() - other.millis();
            return copy;
        }

        /**
         * Returns the elapsed time in minutes; does not stop the timer, if started.
         */

        public float minutes()
        {
            return seconds()/60;
        }

        /**
         * <tt>T = this + other</tt>; Constructs and returns a new timer which is the sum of the receiver and the other timer.
         * The new timer is not started.
         * @param other the timer to add.
         * @return a new timer.
         */

        public Timer plus(Timer other)
        {
            Timer copy = new Timer();
            copy.m_elapsedTime = millis() + other.millis();
            return copy;
        }

        /**
         * Resets the timer.
         * @return <tt>this</tt> (for convenience only).
         */

        public Timer reset()
        {
            m_elapsedTime = 0;
            m_baseTime = 0;
            return this;
        }

        /**
         * Returns the elapsed time in seconds; does not stop the timer, if started.
         */

        public float seconds()
        {
            return ((float) millis())/UNIT;
        }

        /**
         * Starts the timer.
         * @return <tt>this</tt> (for convenience only).
         */

        public Timer start()
        {
            m_baseTime = DateTime.Today.Millisecond;
            return this;
        }

        /**
         * Stops the timer. You can start it again later, if necessary.
         * @return <tt>this</tt> (for convenience only).
         */

        public Timer stop()
        {
            if (m_baseTime != 0)
            {
                m_elapsedTime = m_elapsedTime + (DateTime.Today.Millisecond - m_baseTime);
            }
            m_baseTime = 0;
            return this;
        }

        /**
         * Shows how to use a timer in convenient ways.
         */

        public static void test(int size)
        {
            //benchmark this piece
            Timer t = new Timer().start();
            int j = 0;
            for (int i = 0; i < size; i++)
            {
                j++;
            }
            t.stop();
            t.display();
            PrintToScreen.WriteLine("I finished the test using " + t);


            //do something we do not want to benchmark
            j = 0;
            for (int i = 0; i < size; i++)
            {
                j++;
            }


            //benchmark another piece and add to last benchmark
            t.start();
            j = 0;
            for (int i = 0; i < size; i++)
            {
                j++;
            }
            t.stop().display();


            //benchmark yet another piece independently
            t.reset(); //set timer to zero
            t.start();
            j = 0;
            for (int i = 0; i < size; i++)
            {
                j++;
            }
            t.stop().display();
        }

        /**
         * Returns a string representation of the receiver.
         */

        public override string ToString()
        {
            return "Time=" + elapsedTime() + " secs";
        }
    }
}
