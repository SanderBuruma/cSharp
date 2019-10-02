using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Parallel_Test
{
    class Program
    {
        static async Task Main()
        {
            List<int> primes = new List<int>() { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41 };
            while(true)
            {
                Console.WriteLine("Press any key to start");
                Console.ReadKey();
                Console.Clear();
                await DoSomethingWithPrimesAsync(primes);
            }
        }
        static async Task DoSomethingWithPrimesAsync(List<int> primes)
        {
            var tasks = new List<Task>() { };
            foreach (var v in primes)
            {
                tasks.Add(Task.Run(() => SlowSquarePrime(v)));
            }
            await Task.WhenAll(tasks);
            Console.WriteLine("All primes processed");
        }
        static void SlowSquarePrime(int prime)
        {
            var r = new Random();
            Thread.Sleep(1000/r.Next(1,10));
            Console.WriteLine(prime * prime);
        }
    }
}
