using System;
using System.Collections.Generic;
using System.Text;

namespace RobobuilderLib
{
    class Neuron
    {
        public enum neurontypes { HOPPFIELD };

        int     no_of_layers;
        int     cl;
        matrix[]  weights;

        public Neuron(int layers)
        {
            no_of_layers = layers;
            weights = new matrix[layers];
        }

        public void setcl(int n)
        {
            if (n>0 && n<=no_of_layers) cl=n-1;
        }

        public void mklayer(int n, int sz)
        {
            if (n < 1 || n > no_of_layers) return;
            n--;

            if (n<no_of_layers) 
                weights[n] = new matrix(sz);
            cl = n;
        }

        public double[] convDouble(bool[] x)
        {
            double[] r = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
                r[i] = x[i] ? 1.0 : 0.0;
            return r;
        }

        double afn(double x) { return (x > 0.0) ? 1.0 : 0.0; }

        public void train(bool[] pattern)
        {
            train(matrix.bipolar(pattern));
        }

        public void train(double[] pattern)
        {
            int size = weights[cl].getr();

            matrix invm = new matrix(1, size, pattern);
            matrix inv  = invm.transpose();
            matrix res = invm.multiply(inv);
            res.zeroI();
            weights[cl].add(res);
        }

        public double[] recog(bool[] pattern)
        {
            return recog(convDouble(pattern));
        }

        public double[] recog(double[] pattern)
        {
            matrix inp = new matrix( pattern.Length, 1 , pattern);
            double[] r = new double[weights[cl].getr()];

            for (int node = 0; node < weights[cl].getr(); node++)
            {
                r[node] = afn ( vectors.dotprod(inp.getrow(0), weights[cl].getrow(node)));

                Console.WriteLine("{0} : {1}", node, r[node] );
            }
            return r;
        }

        public void feedforward()
        {
        }

        public void backpropagate()
        {
        }


    }
}
