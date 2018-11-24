using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelLabs
{
    //Algorith Floyda
    public class Task5 : IRun
    {
        private static int M = 900;
        private int Core = Environment.ProcessorCount;
        private int[,] a;
        private int[,] d;

        public int[,] Arr(int n, int m)
        {
            Random r = new Random();
            int[,] arr = new int[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    arr[i, j] = r.Next(0, 50);
                }
            }
            return arr;
        }

        public void Output(int[,] a, int n)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write(a[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public static void FloydWarshall(int[,] distance)
        {
            for (int k = 0; k < M; ++k)
            {
                for (int i = 0; i < M; ++i)
                {
                    for (int j = 0; j < M; ++j)
                    {
                        if (distance[i, k] + distance[k, j] < distance[i, j])
                            distance[i, j] = distance[i, k] + distance[k, j];
                    }
                }
            }
        }



        void FloydWarshallHelper(int k,int start_inedex,int end_index,int[,]distance)
        {
                for (int i = start_inedex; i < end_index; i++)
                {
                    for (int j = 0; j < M; j++)
                    {
                        if (distance[i, k] + distance[k, j] < distance[i, j])
                            distance[i, j] = distance[i, k] + distance[k, j];
                    }
                }
            
        }

        void FloydWarshallMultiThreaded(int[,]c)
        {
            int step = M / Core;
            List<Thread> t = new List<Thread>(Core);
            for (int k = 0; k < M; k++)
            {
                for (int i = 0; i < Core; i += step)
                {        
                    Thread s = new Thread(() => FloydWarshallHelper(k,i, i + step, c));
                    s.Start();
                    t.Add(s);
                }
                for (int i = 0; i < t.Count; i++)
                {
                    t[i].Join();
                }
            }
            
            
        }
        public void Run()
        {

            
            var stopwatch = Stopwatch.StartNew();
            a = Arr(M, M);
            //Output(a, M);
            FloydWarshall(a);
            var res2 = stopwatch.Elapsed;
            //Output(b, M);
            Console.WriteLine($"Simple floyd:{res2}");

            stopwatch = Stopwatch.StartNew();
            d = a;
            FloydWarshallMultiThreaded(d);
           
            var res3 = stopwatch.Elapsed;
            //Output(c, M);
            Console.WriteLine($"Parallel floyd:{res3}");

        }

    }
}
