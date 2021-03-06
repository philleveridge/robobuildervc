﻿using System;

namespace SinDemo
{

    public class foo
    {
        public double amp;
        public int offset;
        public int phase;

        public foo(double a, int b, int c) { amp = a; offset = b; phase = c; }
    }

    public class bar 
    {
        private double MutationRate = 0.1;       //mutation rate
        private double RecombinationRate = 0.5;  //recomination rate            
        Random rn;
        int seed = 1234;
        
        static public int fmatch(bar a, bar b)
        {
            return b.fit - a.fit;
        }

        public int fit;
        public foo[] abc;

        public bar(int n)
        {
            fit = 10;
            rn = new Random(seed);
            abc = new foo[n];
        }

        public bar(foo[] z)
        {
            fit = 10;
            rn = new Random(seed);
            abc = z;
        }

        public bar(bar x)
        {
            fit = 10;
            rn = new Random(seed);
            clone(x);
            //breed(x);
        }

        public bar(bar x, bar y)
        {
            fit = 10;
            rn = new Random(seed);
            breed(x,y);
        }

        public void clone(bar parent)
        {
            abc = new foo[parent.abc.Length];

            for (int i = 0; i < abc.Length; i++)
                abc[i] = parent.abc[i];
        }

        public void breed()
        {
            if (abc != null)
                breed(this);
        }

        public void breed(bar parent)
        {
            foo[] nabc = new foo[parent.abc.Length];

            for (int i = 0; i < abc.Length; i++)
            {
                if (rn.NextDouble() < RecombinationRate)
                {
                    //straight clone
                    nabc[i] = parent.abc[i];
                }
                else if (rn.NextDouble() < MutationRate)
                {
                    //mutation
                    nabc[i].amp    = rn.NextDouble()*30.0;  // 0->30
                    nabc[1].offset = rn.Next(90, 160);      // 0->255
                    nabc[1].phase  = rn.Next(15);           // 0->15
                }
                else
                {
                    //clone variation
                    nabc[i].amp = parent.abc[i].amp * (1-rn.NextDouble());

                    nabc[i].offset = parent.abc[i].offset + rn.Next(-7,7);

                    nabc[i].phase = parent.abc[i].phase + rn.Next(-1, 1);
                }
            }
            abc = nabc;
        }

        public void breed(bar parent1, bar parent2)
        {
            abc = new foo[parent1.abc.Length];
        }

        public void print()
        {
            Console.WriteLine("Fit {0}", fit);
            for (int i = 0; i < abc.Length; i++)
                Console.WriteLine("{3}: {0},{1},{2}", abc[i].amp, abc[i].offset, abc[i].phase, i);
        }


    }

}
