using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;


namespace ImageFocus
{
    public class Focus
        {
            /// <summary>
            /// Calculates focus of image by standard deviation
            /// http://www.mathworks.com/matlabcentral/fileexchange/27314-focus-measure
            /// </summary>
            /// <param name="image"></param>
            /// <returns></returns>
        public unsafe static double Calculate(System.Drawing.Image image)
        {
            Bitmap source = new Bitmap(image);

            Rectangle rect = new Rectangle(0, 0, source.Width, source.Height);

            BitmapData bmd = source.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int[] pixelData = new int[source.Width*source.Height];

            System.Runtime.InteropServices.Marshal.Copy(bmd.Scan0, pixelData, 0, pixelData.Length);
      

            Parallel.For(0, pixelData.Length, i =>
                {
                    Color c = Color.FromArgb(pixelData[i]);
                    int luma = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                    pixelData[i] = luma;
                });

            double mean = pixelData.AsParallel().Average();
            double FM = pixelData.AsParallel().Aggregate(0.0, (subtotal, item) => subtotal + ((item - mean) * (item - mean)),
                                                         (total, thisThread) => total + thisThread,
                                                         (finalSum) => Math.Sqrt((finalSum / (pixelData.Length - 1))));

            return Math.Round(FM, 4);

        }

      
       //public static double StandardDeviation(int[] source)
       // {
       //     double avg = source.AsParallel().Average();
       //     double d = source.Aggregate(0.0, (total, next) => total += Math.Pow(next - avg, 2));
       //     double variance = d / (source.Count() - 1);
       //     return Math.Sqrt(variance);
       // }
                 

        // main method that I am using just to test my methods in the class Focus
        static void Main(string[] args)
        {
           
            Image myimage = Image.FromFile(@"C:\Users\payam\Desktop\frame-1.png", true);
            var watch2 = Stopwatch.StartNew();
            var fvalue = Calculate(myimage);
           //int[] numbers = new int[9] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
           //double fvalue = StandardDeviation(numbers);
            Console.WriteLine("The second method took {0} miliseconds to be completed", watch2.ElapsedMilliseconds);
            Console.WriteLine("the focus value is {0}", fvalue);
    
            Console.ReadLine();


        }
    }
}



