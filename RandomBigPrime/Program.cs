using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RandomBigPrime
{
    class Program
    {
        [STAThreadAttribute]
        static void Main()
        {
            var primes = new List<int>() { 2 };
            for (int i = 3; i < 100000; i+=2)
            {
                foreach (var prime in primes)
                {
                    if (prime * prime > i) break;
                    if (i % prime == 0) goto goto2;
                }
                primes.Add(i);
                goto2:;
            }

            Console.Write("Input the number of bytes the prime number needs to have: ");
            int primeByteSize = int.Parse(Console.ReadLine());
            var rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[primeByteSize];
            BigInteger p;

            var sw = new Stopwatch();
            sw.Start();
            int correct = 0;
            int mistakes = 1;

            while (true)
            {
                notPrime:;
                sw.Restart();
                rng.GetBytes(bytes);
                p = new BigInteger(bytes) - 1;
                if (p < 3) goto notPrime;
                foreach (var prime in primes)
                {
                    if (p % prime == 0) goto notPrime;
                }
                if (BigInteger.ModPow(2, p - 1, p) == 1)
                {
                    mistakes++;
                    goto notPrime;
                }
                correct++;

                double errorRate = mistakes;
                errorRate /= (double)(mistakes + correct);
                errorRate = Math.Round(errorRate * 100, 5);
                Console.WriteLine(correct.ToString("N0") + " correct guesses, " + mistakes.ToString("N0") + " mistakes. " + errorRate.ToString("N2") + "% error rate. " + primeByteSize.ToString("N0") + " bytesize.");
                sw.Stop();
            }
        }
    }
}
