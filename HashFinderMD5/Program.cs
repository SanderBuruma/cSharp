using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace HashFinderMD5
{
    class Program
    {
        [STAThread]
        static void Main()
        {

            while (true)
            {
                Console.Clear();
                Console.Write("Input a password to guess: ");
                string hash = CalculateMD5Hash(Console.ReadLine());
                Console.WriteLine("The hash is " + hash);
                Clipboard.SetText(hash);
                string recoveredPassword = "";
                List<char> newPw;
                for (int j = 0; j < 10; j++)
                {
                    BigInteger powerOf = 256;
                    for (int i = 0; i < j; i++)
                    {
                        powerOf *= 256;
                    }

                    for (BigInteger i = 0; i < powerOf; i++)
                    {
                        newPw = new List<char> { };
                        BigInteger copyOfI = i;
                        string preHashPw = "";
                        for (int k = 0; k <= j; k++)
                        {
                            preHashPw += ((char)BigInteger.ModPow(copyOfI,1,256));
                            copyOfI /= 256;
                        }
                        var testHash = CalculateMD5Hash(preHashPw);
                        if (testHash == hash)
                        {
                            recoveredPassword = preHashPw;
                            goto end;
                        }
                    }
                }
                end:;
                Console.WriteLine();
                Console.WriteLine("The Password is: " + recoveredPassword);
                Console.ReadKey();
            }
        }
        static string CalculateMD5Hash(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
