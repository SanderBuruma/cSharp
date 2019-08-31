using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MemoryAndPerformanceTests
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("write the the number of bits to be used by the array: ");
            int maxNr = Math.Min(2000000000, Math.Max(100, int.Parse(Console.ReadLine())));
            var sw = new Stopwatch();
            sw.Start();
            var bits = new BitArray(maxNr);
            bits.SetAll(true);
            bits[0] = false;
            bits[1] = false;
            int maxToSieve = (int)Math.Sqrt(bits.Length) + 1;
            for (int i = 2; i < maxToSieve; i++)
            {
                if (bits[i].Equals(false)) continue;
                for (int j = i * 2; j < bits.Length; j+= i)
                {
                    bits[j] = false;
                    if (j + i * 20 > bits.Length) continue;
                    bits[j += i] = false;
                    bits[j += i] = false;
                    bits[j += i] = false;
                    bits[j += i] = false;
                    bits[j += i] = false;
                    bits[j += i] = false;
                    bits[j += i] = false;
                    bits[j += i] = false;
                    bits[j += i] = false;
                    bits[j += i] = false;
                    bits[j += i] = false;
                    bits[j += i] = false;
                    bits[j += i] = false;
                    bits[j += i] = false;
                    bits[j += i] = false;
                    bits[j += i] = false;
                    bits[j += i] = false;
                    bits[j += i] = false;
                    bits[j += i] = false;

                }
            }

            Console.WriteLine(bits.Length.ToString("N0"));
            Console.WriteLine(sw.ElapsedMilliseconds.ToString("N0") + " ms taken to sieve");
            sw.Restart();
            int primesCount = 0;
            for (int i = bits.Length - 1;; i--)
            {
                if (bits[i].Equals(true))
                {
                    Console.WriteLine(i.ToString("N0"));
                    break;
                }
            } 
            foreach (var bit in bits)
            {
                if (bit.Equals(true)) primesCount++;
            }
            Console.WriteLine(primesCount.ToString("N0") + " primes found");
            Console.WriteLine(sw.ElapsedMilliseconds.ToString("N0") + " ms taken to count primes");
            sw.Restart();
            byte[] bytes = new byte[bits.Length / 8 + (bits.Length % 8 == 0 ? 0 : 1)];
            bits.CopyTo(bytes, 0);
            File.WriteAllBytes(primesCount + "primes.bin", bytes);
            Console.WriteLine(sw.ElapsedMilliseconds.ToString("N0") + " ms taken to save to file");
            Console.ReadKey();
        }
    }
}
