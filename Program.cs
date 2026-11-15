using System.Diagnostics;

namespace DZ4_MultiThreadedProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Параметры системы, CPU: {Environment.ProcessorCount} ядер, OS: {Environment.OSVersion}");
            Console.WriteLine(new string('_', 45));

            int[] size = new int[]{100_000, 1_000_000, 10_000_000 };
            var rnd = new Random(97);

            foreach (int n in size)
            {
                int[] arr = new int[n];
                for (int i = 0; i < n; i++)
                {
                    arr[i] = rnd.Next(0, 1000);
                }
                // 2. последовательно
               var sw = Stopwatch.StartNew();
                long s1 = SumSequential(arr);
                sw.Stop();
                Console.WriteLine($"Seq n={n,10}: {s1,15}  {sw.Elapsed.TotalMilliseconds:F3} ms");

                // 3.Параллельное
                sw.Restart();
                long s2 = SumParallel(arr);
                sw.Stop();
                Console.WriteLine($"Par n={n,10}: {s2,15}  {sw.Elapsed.TotalMilliseconds:F3} ms");

                sw.Restart();
                long s3 = SumLinq(arr);
                sw.Stop();
                Console.WriteLine($"PLINQ n={n,8}: {s3,15}  {sw.Elapsed.TotalMilliseconds:F3} ms");

                Console.WriteLine(new string('_', 45));
            }
        }

        /// <summary>
        /// Последовательное вычисление суммы
        /// </summary>
        /// <param name="arr"></param>
        private static long SumSequential(int[] arr)
        {
           long sum = 0;
            foreach (var x in arr) sum += x;
            return sum;
        }

        /// <summary>
        /// Параллельное вычисление суммы
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        static long SumParallel(int[] a)
        {
            int parts = Environment.ProcessorCount;
            int chunk = a.Length / parts;
            long total = 0;
            var tasks = new List<Task<long>>();

            for (int p = 0; p < parts; p++)
            {
                int start = p * chunk;
                int end = (p == parts - 1) ? a.Length : start + chunk;
                tasks.Add(Task.Run(() =>
                {
                    long local = 0;
                    for (int i = start; i < end; i++) local += a[i];
                    return local;
                }));
            }
            foreach (var t in tasks) total += t.Result;
            return total;
        }

        /// <summary>
        /// Вычисление с помощью LINQ
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        static long SumLinq(int[] a) => a.AsParallel().Sum(x => (long)x);
    }
}
