using System;
using System.CodeDom;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace MarkovTextGenerator
{
   class Program
   {
      static void Main()
      {
         string path = @"C:\Users\sander.buruma\Desktop\coding\cSharp\MarkovTextGenerator\maintext2.txt";
         string mainText = File.ReadAllText(path);

         string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890. ,:";

         mainText = new Regex($"[^{chars}]").Replace(mainText,"");

         int[,] markovCounts = new int[66,66];
         for (Int32 i = 0; i < 66; i++)
         for (Int32 j = 0; j < 66; j++)
         {
               markovCounts[i,j] = 0;
         }

         for (Int32 i = 0; i < mainText.Length-1; i++)
         {
            char char1 = mainText.Substring(i,   1)[0];
            char char2 = mainText.Substring(i+1, 1)[0];
            int ind1 = chars.IndexOf(char1);
            int ind2 = chars.IndexOf(char2);

            markovCounts[
               ind1,
               ind2
            ]++;
         }

         var totalSums = new int[66];
         for (Int32 i = 0; i < 66; i++)
         {
            for (Int32 j = 0; j < 66; j++)
            {
               totalSums[i] += markovCounts[i,j];
            }
         }

         while (true)
         {
            Console.Clear();
            Console.Write("Length of text to produce: ");
            if (!Int32.TryParse(Console.ReadLine(), out int @length)) continue;
            Console.Write("First Character: ");
            if (!char.TryParse(Console.ReadKey().KeyChar.ToString(), out char initialChar)) continue;

            string text = "" + initialChar;
            var rnd = new Random();
            for (int _ = 0; _ < @length; _++)
            {
               int indexOfCurrentChar = chars.IndexOf(text[text.Length-1]);
               int numOfThingToGet = rnd.Next(0,totalSums[indexOfCurrentChar]);
               for (Int32 i = 0; i < 66; i++)
               {
                  numOfThingToGet -= markovCounts[indexOfCurrentChar,i];
                  if (numOfThingToGet < 0)
                  {
                     text += chars[i];
                     break;
                  }
               }
            }

            Console.WriteLine(text);
            Console.ReadKey();

         }
      }
   }
}
