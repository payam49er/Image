using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace parallel
{
    class Program
    {
        public static double SumRoot(int root)
        {
            double result = 0;
            for (int i = 0; i < 10000000; i++)
            {
                result += Math.Exp(Math.Log(i) / root);
            }
            return  Math.Round(result,2);
        }
              

        static void Main(string[] args)
        {
            var Watch = Stopwatch.StartNew();
            //for (int i = 2; i < 20; i++)
            //{
            //    var result = SumRoot(i);
            //    Console.WriteLine("root {0} : {1}", i, result);
            //}

            Parallel.For(2, 20, (i) =>
                {
                    var result = SumRoot(i);
                    Console.WriteLine("Root {0} : {1}", i, result);
                });
            Console.WriteLine(Watch.ElapsedMilliseconds);
            Console.ReadLine();
        }
    }
}
