using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace PrimesCalculator
{
    //now with threading
    class Program
    {
        static void Main()
        {
            var sw = new Stopwatch();
            sw.Start();
            var threads = new List<Thread> { };
            List<long> primes = new List<long> { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37 };
            List<List<long>> primesToAddQue = new List<List<long>> { new List<long> { }, new List<long> { }, };
            threads.Add(new Thread(() => CalcPrimesStarter(41, primes)));
            threads.Add(new Thread(() => CalcPrimesStarter(0, primes)));
            threads[0].Start();
            Thread.Sleep(1000);
            if (threads[0].ThreadState == 0) threads[0].Suspend();
            long nextbatchStartPoint = primes[primes.Count - 1] + 2;
            int temp1 = 0;
            while (temp1++ >= 0)
            {
                primesToAddQue[0] = new List<long> { };
                threads[0] = new Thread(() => CalcPrimes(nextbatchStartPoint, primes, primesToAddQue[0]));
                nextbatchStartPoint += 5000000;
                threads[0].Start();
                Thread.Sleep(100);
                primesToAddQue[1] = new List<long> { };
                threads[1] = new Thread(() => CalcPrimes(nextbatchStartPoint, primes, primesToAddQue[1]));
                nextbatchStartPoint += 5000000;
                threads[1].Start();
                while (true)
                {
                    Thread.Sleep(100);
                    if (threads[0].ThreadState != 0 && threads[1].ThreadState != 0)
                    {
                        break;
                    }
                }
                primes.AddRange(primesToAddQue[0]);
                primes.AddRange(primesToAddQue[1]);
                Console.WriteLine(primes.Count.ToString("N0") + " - " + (sw.ElapsedMilliseconds).ToString("N0") + " ms");
            }
        }

        static void CalcPrimes(long x, List<long> primes, List<long> primeToAddQue)
        {
            for (long i = x; i < x + 5000000; i += 2)
            {
                long primesCount = primes.Count;
                for (int j = 0; j < (int)primesCount; j++)
                {
                    if (i % primes[j++] == 0) goto notPrime1;
                    if (i % primes[j++] == 0) goto notPrime1;
                    if (i % primes[j++] == 0) goto notPrime1;
                    if (i % primes[j++] == 0) goto notPrime1;
                    if (i % primes[j++] == 0) goto notPrime1;
                    if (i % primes[j++] == 0) goto notPrime1;
                    if (i % primes[j++] == 0) goto notPrime1;
                    if (i % primes[j++] == 0) goto notPrime1;
                    if (i % primes[j++] == 0) goto notPrime1;
                    if (i % primes[j++] == 0) goto notPrime1;
                    if (i % primes[j++] == 0) goto notPrime1;
                    if (i % primes[j++] == 0) goto notPrime1;
                    if (i % primes[j] == 0) goto notPrime1;
                    if (primes[j] * primes[j] > i) break;
                }
                primeToAddQue.Add(i);
                notPrime1:;
            }
        }
        static void CalcPrimesStarter(long x, List<long> primes)
        {
            for (long i = x; i < x + 10000000; i += 2)
            {
                long primesCount = primes.Count;
                for (int j = 0; j < (int)primesCount; j++)
                {
                    if (i % primes[j++] == 0) goto notPrime1;
                    if (i % primes[j++] == 0) goto notPrime1;
                    if (i % primes[j] == 0) goto notPrime1;
                    if (primes[j] * primes[j] > i) break;
                }
                primes.Add(i);
                notPrime1:;
            }
        }
    }
}
