using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelLabs
{
    //Multiplication of matrixs
    public class Task2 : IRun
    {
        private static int N=500;
        private int Core = Environment.ProcessorCount;
        private int[,] a;
        private int[,] b;
        private int[,] c=new int[N,N];

        public int[,] SimpleMult(int[,] a, int[,] b)
        {
            int[,] arr = new int[N, N];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    arr[i, j] = 0;
                    for (int k = 0; k < N; k++)
                    {
                        arr[i, j] += a[i, k] * b[k, j];
                    }
                }
            }
            return arr;
        }

       

        public int[,] ParalleledMatrixMultiplicationMS(int[,] a, int[,] b)
        {
            int[,] d = new int[N, N];
            int s = a.GetLength(0);

            Parallel.For(0, s, delegate (int i)
            {
                for (int j = 0; j < s; j++)
                {
                    int v = 0;

                    for (int k = 0; k < s; k++)
                    {
                        v += a[i, k] * b[k, j];
                    }

                    d[i, j] = v;
                }
            });
            return d;
        }

        public void ParalleledMatrixMultiplication(int[,] a, int[,] b, int thread)
        {
            int elements = (N * N);
            //Console.WriteLine($"elements {elements}");
            int operations = elements / Core;
            //Console.WriteLine($"operations {operations}");
            int rest = elements % Core;
            //Console.WriteLine($"rest {rest}");

            int start, end;

            if (thread == 0)
            {
                start = operations * thread;
                end = (operations * (thread + 1)) + rest;
                //Console.WriteLine($"{thread}:{start}___{end}");
            }
            else
            {
                start = operations * thread + rest;
                end = (operations * (thread + 1)) + rest;
                //Console.WriteLine($"{thread}:{start}___{end}");
            }

            for (int op = start; op < end; ++op)
            {
                int row = op % N;
                int col = op / N;
                int r = 0;
                for (int i = 0; i < N; ++i)
                {
                    int e1 = a[row,i];
                    int e2 = b[i,col];
                    r += e1 * e2;
                    //Console.WriteLine($"{r}");
                }
                c[row,col] = r;
                //Console.WriteLine($"{row}____{col}:{c[col, row]}");
            }
        }

        public int[,] Arr(int n, int m)
        {
            Random r = new Random();
            int[,] arr = new int[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    arr[i, j] = r.Next(0, 4);
                }
            }
            return arr;
        }

        public void Output(int[,] a, int n,int m)
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
        public void Run()
        {
            a = Arr(N, N);
            b = Arr(N, N);
            //Output(a, N, N);
            //Output(b, N, N);
            var stopwatch = Stopwatch.StartNew();
            int[,] simpleRes = SimpleMult(a, b);
            //Output(simpleRes, N, N);
            var res1 = stopwatch.Elapsed;
            Console.WriteLine($"Simple mult:{res1}");

            stopwatch = Stopwatch.StartNew();
            int[,] paralFor = ParalleledMatrixMultiplicationMS(a, b);
            var res2 = stopwatch.Elapsed;
            Console.WriteLine($"Parallel muit(for):{res2}");
            //Output(paralFor, N,N);

            stopwatch = Stopwatch.StartNew();

            Thread t1 = new Thread(() => ParalleledMatrixMultiplication(a, b,0));
            Thread t2 = new Thread(() => ParalleledMatrixMultiplication(a, b, 1));
            Thread t3 = new Thread(() => ParalleledMatrixMultiplication(a, b, 2));
            Thread t4 = new Thread(() => ParalleledMatrixMultiplication(a, b, 3));

            t1.Start();
            t2.Start();
            t3.Start();
            t4.Start();

            t1.Join();
            t2.Join();
            t3.Join();
            t4.Join();


            //Output(c, N,N);
            var res3 = stopwatch.Elapsed;
            Console.WriteLine($"Parallel mult(threads):{res3}");

        }
    }
}
