using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ModularExponantiation
{
    class Program
    {
        static void Main()
        {
            var smallPrimes = new List<int> { 2 };
            for (int k = 3; k < 8000000; k+=2)
            {
                foreach (var v in smallPrimes)
                {
                    if (v * v > k) break;
                    if (k % v == 0) goto notPrimeA;
                }
                smallPrimes.Add(k);
                notPrimeA:;
            }

            Console.WriteLine("Input x for 2 ^ x - 1, which will be the starting number from which to check primes. Must be 2 or more.");
            int startingExponent = int.Parse(Console.ReadLine());
            if (startingExponent < 2) throw new FormatException("Input an integer of at least 2");

            BigInteger i = BigInteger.Pow(2, startingExponent) - 1;
            int kPlus = 0;

            var sw = new Stopwatch();
            sw.Start();

            while (true)
            {
                notPrimeB:;
                kPlus += 2;
                BigInteger k = i + kPlus;
                //Simple relatively inexpensive check
                foreach (int v in smallPrimes)
                {
                    if (k % v == 0) goto notPrimeB;
                }
                for (int j = 9; j < 11; j++)
                {
                    //Fermat test
                    if (BigInteger.ModPow(j,k-1,k) != 1)
                    {
                        goto notPrimeB;
                    }
                }
                sw.Stop();
                Console.WriteLine("2 ^ " + startingExponent.ToString("N0") + " - 1 + " + kPlus.ToString("N0") +  " is prime");
                Console.WriteLine(sw.ElapsedMilliseconds.ToString("N0") + " ms used\n");
                sw.Restart();
            }
        }
        static BigInteger ModularExpo(BigInteger x, BigInteger exponent, BigInteger mod)
        {
            //find wrong input values and throw error if found
            if (x < 1)
            {
                throw new FormatException("ExpoModPowerOf2 function received an x value less than 1");
            }
            if (exponent < 2)
            {
                throw new FormatException("ExpoModPowerOf2 function received an exponent value less than 2");
            }
            if (mod < 1)
            {
                throw new FormatException("ExpoModPowerOf2 function received a mod value less than 1");
            }

            BigInteger returnValue = 1;
            BigInteger twoToThePowerCount = 0;
            while (true)
            {
                if (exponent % 2 == 1)
                {
                    if (twoToThePowerCount == 0)
                    {
                        returnValue = x % mod;
                    }
                    else
                    {
                        returnValue *= ModularExpoPowerOf2(x, BigInteger.Pow(2, (int)twoToThePowerCount), mod);
                        returnValue %= mod;
                    }
                }
                exponent /= 2;
                twoToThePowerCount++;
                if (exponent <= 0)
                {
                    break;
                }
            }
            return returnValue;
        }
        private static BigInteger ModularExpoPowerOf2(BigInteger x, BigInteger exponent, BigInteger mod)
        {
            //find wrong input values and throw error if found
            if (x < 1)
            {
                throw new FormatException("ExpoModPowerOf2 function received an x value less than 1");
            }
            if (exponent < 1)
            {
                throw new FormatException("ExpoModPowerOf2 function received an exponent value less than 1");
            }
            if (mod < 1)
            {
                throw new FormatException("ExpoModPowerOf2 function received a mod value less than 1");
            }

            if (exponent == 1)
            {
                return x % mod;
            }
            else
            {
                long log2ofExponent = 0;
                while (exponent > 1)
                {
                    exponent /= 2;
                    log2ofExponent++;
                }
                x %= mod;
                for (long i = 0; i < log2ofExponent; i++)
                {
                    x *= x;
                    x %= mod;
                }

                return x;
            }
        }
    }
}
