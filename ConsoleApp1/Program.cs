using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main()
        {
            var sw = new Stopwatch { };
            sw.Start();

            List<int> primes = new List<int> { 2, 3, 5, 7, 11, 13 };
            for (int i = 17; i < 1e8; i+=4)
            {
                foreach (var prime in primes)
                {
                    if (prime * prime > i) break;
                    if (i % prime == 0) goto goto1;
                }
                primes.Add(i);
                goto1: i += 2;
                foreach (var prime in primes)
                {
                    if (prime * prime > i) break;
                    if (i % prime == 0) goto goto2;
                }
                primes.Add(i);
                goto2:;
            }

            Console.WriteLine(primes.Count.ToString("N0"));
            Console.WriteLine(sw.ElapsedMilliseconds.ToString("N0"));
            sw.Stop();

            while (int.TryParse(Console.ReadLine(), out int temp))
            {
                if (temp < primes.Count) Console.WriteLine(primes[temp].ToString("N0"));
                else Console.WriteLine("This prime was not calculated");
            }
        }
    }
}
