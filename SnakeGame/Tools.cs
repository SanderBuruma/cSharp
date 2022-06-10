using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame
{
    public class Tools
    {
        public static double[,] RotateMatrix(double[,] matrix, int n)
        {
            double[,] ret = new double[n, n];

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    ret[i, j] = matrix[n - j - 1, i];
                }
            }

            return ret;
        }
    }
}
