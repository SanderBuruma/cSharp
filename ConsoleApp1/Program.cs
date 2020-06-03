using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
   class Program
   {
      static void Main(string[] args)
      {
         Something1 smt = new Something1(5, 87, 25, "Susan");
         String str1 = smt.Name;
         smt.Name = "Andrea";
      }
   }

   public class Something1
   {
      public int Age { get; }
      public int Length { get; }
      public int Width { get; }
      public String Name { get; set; }

      public Something1(int age, int length, int width, String name)
      {
         Age =  age;
         Length = length;
         Width = width;
         Name = name;
      }
   }
}
