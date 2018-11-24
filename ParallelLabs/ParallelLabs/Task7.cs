using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace ParallelLabs
{
    public class Task7
    {
        private static int V = 2000;
        private int CORES = Environment.ProcessorCount;
        private int[,] graph=new int[V,V];
        int INT_MAX = Int32.MaxValue;

        bool IsValidEdge(int u, int v, bool[] inMS)
        {
            if (u == v)
            {
                return false;
            }
            if (inMS[u] == false && inMS[v] == false)
            {
                return false;
            }
            else if (inMS[u] == true && inMS[v] == true)
            {
                return false;
            }

            return true;
        }

        void PrimSingleThreaded()
        {
            bool[] inMS=new bool[V];
            inMST[0] = true;
            int edge_count = 0;
            while (edge_count < V - 1)
            {
                int min = INT_MAX, a = -1, b = -1;
                for (int i = 0; i < V; i++)
                {
                    for (int j = 0; j < V; j++)
                    {
                        if (graph[i,j] < min)
                        {
                            if (IsValidEdge(i, j, inMST))
                            {
                                min = graph[i,j];
                                a = i;
                                b = j;
                            }
                        }
                    }
                }
                if (a != -1 && b != -1)
                {
                    inMS[b] = true;
                    inMS[a] = true;
                    edge_count++;
                }
            }
        }

        bool[] inMST = new bool[V];

        int a, b, min;

        void PrimHelper(int start_index, int end_index)
        {
            for (int i = start_index; i < end_index; i++)
            {
                for (int j = 0; j < V; j++)
                {
                    if (graph[i,j] < min)
                    {
                        if (IsValidEdge(i, j, inMST))
                        {                    
                            min = graph[i,j];
                            a = i;
                            b = j;
                        }
                    }
                }
            }
        }

        void PrimMultiThreaded()
        {
            int step = V / CORES;
            int start_index;
            int threads_count = (int)(V * 1.0 / step);
            List<Thread> threads=new List<Thread>(threads_count);

            inMST[0] = true;
            int edge_count = 0;
            while (edge_count < V - 1)
            {
                start_index = 0;
                a = -1;
                b = -1;
                min = INT_MAX;
                for (int i = 0; i < threads_count; i++)
                {
                    Thread t = new Thread(()=>PrimHelper(start_index, Math.Min(start_index + step, V)));
                    threads.Add(t);
                    t.Start();
                    start_index += step;
                }
                for (int i = 0; i < threads_count; i++)
                {
                    threads[i].Join();
                }

                if (a != -1 && b != -1)
                {                    
                    inMST[b] = true;
                    inMST[a] = true;
                    edge_count++;
                }
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
                    arr[i, j] = r.Next(0, 100);
                }
            }
            return arr;
        }
        public void Run()
        {
            graph = Arr(V, V);

            var stopwatch = Stopwatch.StartNew();
            PrimSingleThreaded();
            var res2 = stopwatch.Elapsed;

            Console.WriteLine($"Simple prima:{res2}");

            stopwatch = Stopwatch.StartNew();
            PrimMultiThreaded();
            var res3 = stopwatch.Elapsed;

            Console.WriteLine($"Parallel prima:{res3}");
        }
    }
}
