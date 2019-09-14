using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressedPrimesCalculator
{
    class Program
    {
        static void Main()
        {
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int y))
                {
                    Console.WriteLine(" " + IndexToNr(y));
                }
            }
        }

        /*
         converts BitArray index to a nr value
        */
        static int IndexToNr(int x)
        {
            int t = x / 15;
            int y = x % 15;
            x = t * 30 - 1;

            goto1:
            x += 2;
            if (x % 3 == 0) goto goto1;
            if (x % 5 == 0) goto goto1;
            y--;
            if (y >= 0) goto goto1;

            return x;
        }
    }
}
