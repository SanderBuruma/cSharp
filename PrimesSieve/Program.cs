using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimesSieve
{
    class Program
    {
        static void Main()
        {
            var sw = new Stopwatch();
            Console.Write("Input the maximum number up to which to sieve for primes (this program wont go higher than 1 billion): ");
            int maxPrime = Math.Min(100000000,Math.Max(100,int.Parse(Console.ReadLine())));
            sw.Start();
            var primes = new bool[maxPrime];
            for (int i = 0; i < primes.Length; i++)
            {
                primes[i] = true;
            }
            primes[0] = false;
            primes[1] = false;
            for (int i = 0; i < primes.Length; i++)
            {
                if (!primes[i]) continue;
                if (i * i > primes.Length) break;
                for (int j = i*2; j < primes.Length; j += i)
                {
                    primes[j] = false;
                    if (primes.Length < j + i * 20) continue;
                    j += i;
                    primes[j += i] = false;
                    primes[j += i] = false;
                    primes[j += i] = false;
                    primes[j += i] = false;
                    primes[j += i] = false;
                    primes[j += i] = false;
                    primes[j += i] = false;
                    primes[j += i] = false;
                    primes[j += i] = false;
                    primes[j += i] = false;
                    primes[j += i] = false;
                    primes[j += i] = false;
                    primes[j += i] = false;
                    primes[j += i] = false;
                    primes[j += i] = false;
                    primes[j += i] = false;
                    primes[j += i] = false;
                    primes[j += i] = false;
                    primes[j += i] = false;

                }
            }
            Console.WriteLine((sw.ElapsedMilliseconds).ToString("N0") + " milliseconds used to calculate the primes");
            sw.Restart();
            int primesCount = 0;
            foreach (var v in primes)
            {
                if (v) primesCount++;
            }
            Console.WriteLine(primesCount.ToString("N0") + " primes detected");
            Console.WriteLine((sw.ElapsedMilliseconds).ToString("N0") + " milliseconds used to count the primes");
            Console.ReadKey();
        }
    }
}
