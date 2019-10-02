using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressedPrimes2
{
    class Program
    {
        static void Main()
        {
            //the principle manner storing primes here is to have every possible prime number represented by a bit in a BitArray
            //index 0 represents 0, 1 represents 1, and so on.
            //However, in this case, to be more efficient with file sizes, 0 could represent 3, 1 5, 2 7, 3 9 and so on. This would halve the size of the BitArray for a given size by removing numbers divisible by 2.
            //Furthermore, the first indices could represent the numbers 5, 7, 11, 13, 17, 19, 23, 25, 29, 31 (which are all numbers which are missing factors 2 and 3).
            //This would further reduce the bit array size by 1/3. By removing the multiples of 5 the primes BitArray would again be reduced by 1/5, and so on, and so on.
            //In a linked list of numbers not divisible by 2, 3 and 5 a repeating sequence appears where the first number is 7, the next 11, then 13, 17, 19, 23, 29, 31.
            //This sequence has gaps (between 7 and 11) of 4, then 2, 4, 2, 4, 6, 2, 6.
            //Finally, this sequence repeats. By removing all numbers not divisible by primes up to 5 the gap sequence above must add up to 30, which is 2*3*5.
            //If we do this with primes up to 7, the non repeating gap sequence sum is 210 (2*3*5*7), for primes up to 11 it is 2310 (2*3*5*7*11), and so on.
            //The gap sequence sum of numbers not divisible by primes up to any prime is the product of every prime up to the last prime.

            var sw = new Stopwatch() { };
            sw.Start();

            //2 is already moved into the product
            List<int> firstPrimes = new List<int> { 3, 5, 7 };
            int lastOfFirstPrimes = firstPrimes[firstPrimes.Count - 1];
            int firstPrimesProduct = 2;

            foreach (var item in firstPrimes)
                firstPrimesProduct *= item;
            firstPrimesProduct += firstPrimes[firstPrimes.Count - 1] + 2;


            int counter = firstPrimes[firstPrimes.Count - 1];
            List<int> primes = new List<int> { };
            while (counter < firstPrimesProduct)
            {
                goto1: counter += 2;
                foreach (var firstPrime in firstPrimes)
                {
                    if (counter % firstPrime == 0) 
                        goto goto1;
                }
                primes.Add(counter);
            }

            List<int> nrGapSequence = new List<int>() { };
            //
            for (int i = 0; i < primes.Count - 1; i++)
                nrGapSequence.Add(primes[i] - primes[0]);
            int nrGapSequenceSum = firstPrimesProduct - lastOfFirstPrimes - 2;

            BitArray primesBitArray = new BitArray(2000000000) { };

            Console.WriteLine(nrGapSequenceSum.ToString("N0"));
            Console.WriteLine(firstPrimesProduct.ToString("N0"));
            Console.WriteLine(nrGapSequence.Sum().ToString("N0"));
            Console.WriteLine(primes.Count.ToString("N0"));

            for (int i = 0; i < primesBitArray.Length; i++)
            {
                long primeToTest = 
                    4 +
                    lastOfFirstPrimes + 
                    nrGapSequence[i % nrGapSequence.Count] + 
                    i / nrGapSequence.Count * nrGapSequenceSum;
                Console.WriteLine(primeToTest.ToString("N0"));
                Console.ReadKey();
            }
        }
    }
}
