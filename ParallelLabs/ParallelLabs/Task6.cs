using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelLabs
{
    //Deixtra algorithm
    public class Task6 : IRun
    {
        private static int M = 900;
        private int Core = Environment.ProcessorCount;
        private int[,] a;

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

        private static int MinimumDistance(int[] distance, bool[] shortestPathTreeSet, int verticesCount)
        {
            int min = int.MaxValue;
            int minIndex = 0;

            for (int v = 0; v < verticesCount; ++v)
            {
                if (shortestPathTreeSet[v] == false && distance[v] <= min)
                {
                    min = distance[v];
                    minIndex = v;
                }
            }

            return minIndex;
        }

        private static void Print(int[] distance, int verticesCount)
        {
            Console.WriteLine("Vertex    Distance from source");

            for (int i = 0; i < verticesCount; ++i)
            {
                Console.WriteLine("{0}\t  {1}", i, distance[i]);
            }
        }

        public static void DijkstraAlgo(int[,] graph, int source, int verticesCount)
        {
            int[] distance = new int[verticesCount];
            bool[] shortestPathTreeSet = new bool[verticesCount];

            for (int i = 0; i < verticesCount; ++i)
            {
                distance[i] = int.MaxValue;
                shortestPathTreeSet[i] = false;
            }

          distance[source] = 0;

            for (int count = 0; count < verticesCount - 1; ++count)
            {
                int u = MinimumDistance(distance, shortestPathTreeSet, verticesCount);
                shortestPathTreeSet[u] = true;

                for (int v = 0; v < verticesCount; ++v)
                {
                    if (!shortestPathTreeSet[v] && Convert.ToBoolean(graph[u, v]) && distance[u] != int.MaxValue && distance[u] + graph[u, v] < distance[v])
                    {
                        distance[v] = distance[u] + graph[u, v];
                    }
                }
            }

            //Print(distance, verticesCount);
        }


        void SingleThreaded(int[,]arr)
        {
            for (int i = 1; i < M/2; i++)
            {
                DijkstraAlgo(arr,0,i);
            }
        }

        void Helper(int[,]arr,int start_index, int end_index)
        {
            for (int k = start_index; k < end_index; k++)
            {
                DijkstraAlgo(arr, 0, k);
            }
        }

        void MultiThreaded(int[,]arr)
        {
            int step = M/4;
            int start_index = 1;
            List<Thread> t = new List<Thread>(Core);
            for (int i = 0; i < Core; i += step)
            {
                Thread r = new Thread(() => Helper(arr, start_index, Math.Min(start_index + step, M )));
                t.Add(r);
                r.Start();
                for (int j = 0; j < t.Count; j++)
                {
                    t[j].Join();
                }
            }
           
        }
        public void Run()
        {
            
            a = Arr(M, M);
            //Output(a, M);
            var stopwatch = Stopwatch.StartNew();
            SingleThreaded(a);
            var res2 = stopwatch.Elapsed;

            Console.WriteLine($"Simple deixtra:{res2}");

            stopwatch = Stopwatch.StartNew();
            MultiThreaded(a);
            var res3 = stopwatch.Elapsed;

            Console.WriteLine($"Parallel deixtra:{res3}");
        }
    }
}
