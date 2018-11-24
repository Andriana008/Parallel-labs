using System;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ParallelLabs
{
    //Adding matrixs
    public class Task1:IRun
    {
        private static int N=8500;
        private int Core = Environment.ProcessorCount;
        private int[,] a;
        private int[,] b;
        private int[,] c=new int[N,N];
        public int[,] Arr(int n, int m)
        {
            Random r = new Random();
            int[,] arr = new int[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    arr[i, j] = r.Next(0, 10000);
                }
            }
            return arr;
        }

        public int[,] Simple(int[,] a, int[,] b, int n)
        {
            int[,] arr = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    arr[i, j] = a[i, j] + b[i, j];
                }
            }
            return arr;
        }

        public void Output(int[,] a,int n)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write(a[i, j]+" ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public void GetSumOFSmallMetrix(int[,] a, int[,] b,int shiftI, int ShiftJ)
        {
            int newSize = N / (Core/2);
            for (int i = 0; i < newSize; i++)
            {
                for (int j = 0; j < newSize; j++)
                {
                    c[i+ shiftI, j+ ShiftJ] = a[i + shiftI, j+ShiftJ]+b[i + shiftI, j + ShiftJ];
                }
            } 
        }

        public void Run()
        {
            //var stopwatch = Stopwatch.StartNew();
            Console.WriteLine(Core);
            Console.WriteLine(N);
            a = Arr(N, N);
            b = Arr(N, N);
            //Output(a, N);
            //Output(b, N);
            var stopwatch = Stopwatch.StartNew();
            int newSize = N / (Core / 2);
            int[,]simpleRes = Simple(a, b, N);
            //Output(simpleRes, N);
            var res1 = stopwatch.Elapsed;
            Console.WriteLine($"Simple adding:{res1}");

            stopwatch = Stopwatch.StartNew();
            Thread t1 = new Thread(() => GetSumOFSmallMetrix(a, b, 0, 0));
            Thread t2 = new Thread(() => GetSumOFSmallMetrix(a, b, newSize, 0));
            Thread t3 = new Thread(() => GetSumOFSmallMetrix(a, b, 0, newSize));
            Thread t4 = new Thread(() => GetSumOFSmallMetrix(a, b,newSize, newSize));
            t1.Start();
            t2.Start();
            t3.Start();
            t4.Start();
            t1.Join();
            t2.Join();
            t3.Join();
            t4.Join();
            var res2 = stopwatch.Elapsed;
            Console.WriteLine($"Parallel adding(threads):{res2}");
            //Output(c, N);

            stopwatch = Stopwatch.StartNew();
            Task task1 = Task.Factory.StartNew(() => GetSumOFSmallMetrix(a, b, 0, 0));
            Task task2 = Task.Factory.StartNew(() => GetSumOFSmallMetrix(a, b, newSize, 0));
            Task task3 = Task.Factory.StartNew(() => GetSumOFSmallMetrix(a, b, 0, newSize));
            Task task4 = Task.Factory.StartNew(() => GetSumOFSmallMetrix(a, b, newSize, newSize));

            //Output(c, N);
            var res3 = stopwatch.Elapsed;
            Console.WriteLine($"Parallel adding(task factory):{res3}");
        }
    }
}
