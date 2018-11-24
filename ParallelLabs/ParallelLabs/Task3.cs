using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelLabs
{
    //Gaus algorithm
    public class Task3 : IRun
    {
        private static int N = 500;
        private static int M = 500;
        private int Core = Environment.ProcessorCount;
        private double[,] a;
        private double[,] Simple;
        private double[,] Parall;

        public double[,] Arr(int n, int m)
        {
            Random r = new Random();
            double[,] arr = new double[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    arr[i, j] = r.Next(0, 10000);
                }
            }
            return arr;
        }

        public void Output(double[,] a, int n, int m)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    Console.Write(a[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public double[,] Gaus(double[,] n)
        {
            for (int i = 0; i < M - 1; i++)
            {
                double divisor = n[i, i];
                for (int col = 0; col < M; col++)
                {
                    n[i, col] = n[i, col] / divisor;
                }
                //Console.WriteLine("1");
                //Output(n, N, M);
                for (int row = 0; row < N; row++)
                {
                    if (row != i)
                    {
                        double factor = n[row, i] * -1;
                        for (int col = 0; col < M; col++)
                        {
                            n[row, col] = n[row, col] + n[i, col] * factor;
                        }
                    }
                }
                //Console.WriteLine("2");
                //Output(n, N, M);
            }
            return n;
        }

        public double[,] GausPar(double[,] n)
        {
            Parallel.For(0, M - 1, i =>
            {
                double divisor = n[i, i];
                for (int col = 0; col < M; col++)
                {
                    n[i, col] = n[i, col] / divisor;
                }
                for (int row = 0; row < N; row++)
                {
                    if (row != i)
                    {
                        double factor = n[row, i] * -1;
                        for (int col = 0; col < M; col++)
                        {
                            n[row, col] = n[row, col] + n[i, col] * factor;
                        }
                    }
                }
            });
            return n;
        }
        public double[,] Solution(double[,] b)
        {
            List<double[,]> transformMatr = new List<double[,]>();
            Thread halfTransfMatr = new Thread((object o) =>
            {
                foreach (double item in (List<double>)o)
                {
                    for (int i = 0; i < N; i++)
                    {
                        for (int j = 0; j < N; j++)
                        {
                            b[i, j] = item * b[i, j];
                        }
                    }
                }
            });
            double [,] t=new double[N,M];
            for (int k = 0; k < N; k++)
            {
                for (int i = 0; i < N; i++)
                {
                    t[i, i] = 1;
                    if (i == k)
                    {
                        t[i, i] = 1 / a[k, k];
                    }
                    else if (i > k)
                    {
                        t[i, k] = -a[i, k] / a[k, k];
                    }
                }
                transformMatr.Add(t);
                //a = t * a;

            }
            halfTransfMatr.Start(transformMatr);
            transformMatr = new List<double[,]>();
            for (int k = N - 1; k > 0; k--)
            {
                t = new double[N, M];
                for (int i = 0; i < N; i++)
                {
                    t[i, i] = 1;
                    if (i < k)
                    {
                        t[i, k] = -a[i, k];
                    }
                }
                transformMatr.Add(t);
            }
            halfTransfMatr.Join();
            halfTransfMatr = new Thread((object o) =>
            {
                foreach (double item in (List<double>)o)
                {
                    for (int i = 0; i < N; i++)
                    {
                        for (int j = 0; j < N; j++)
                        {
                            b[i, j] = item * b[i, j];
                        }
                    }
                }
            });
            halfTransfMatr.Start(transformMatr);
            halfTransfMatr.Join();
            return b;
        }


        public void Run()
        {
            a = Arr(N, M);
            //Output(a, N,M);
            var stopwatch = Stopwatch.StartNew();
            Simple = Gaus(a);
            //Output(e, N, M);
            var res1 = stopwatch.Elapsed;
            Console.WriteLine($"Simple:{res1}");


            stopwatch = Stopwatch.StartNew();
            Parall = GausPar(a);
            //Output(e, N,M);
            var res2 = stopwatch.Elapsed;
            Console.WriteLine($"Parallel:{res2}");

            stopwatch = Stopwatch.StartNew();
            Parall = Solution(a);
            //Output(e, N,M);
            var res3 = stopwatch.Elapsed;
            Console.WriteLine($"Parallel:{res3}");

        }
    }
}
