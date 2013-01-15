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

 
        public static double Calculate(System.Drawing.Image image)
        {
            Bitmap source = new Bitmap(image);
                                   
            Rectangle rect = new Rectangle(0, 0, source.Width, source.Height);

            BitmapData bmd = source.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int[] pixelData = new int[source.Width*source.Height];

            System.Runtime.InteropServices.Marshal.Copy(bmd.Scan0, pixelData, 0, pixelData.Length);

            
             // convert to gray scale
            Parallel.For(0, pixelData.Length, i =>
                {
                    Color c = Color.FromArgb(pixelData[i]);
                    int luma = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                    pixelData[i] = luma;
                });

          
            // calculate the standard deviation
            double mean = pixelData.AsParallel().Average();
            double FM = pixelData.AsParallel().Aggregate(0.0, (subtotal, item) => subtotal + ((item - mean) * (item - mean)),
                                                         (total, thisThread) => total + thisThread,
                                                         (finalSum) => Math.Sqrt((finalSum / (pixelData.Length - 1))));

            return Math.Round(FM, 4);

        }


        public unsafe static double CalculatebyPointer(Image image)
        {
            Bitmap source = new Bitmap(image);

            Rectangle rect = new Rectangle(0, 0, source.Width, source.Height);

            BitmapData bmd = source.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int totalPixels = rect.Height * rect.Width;
            int[] pixelData = new int[totalPixels];
        
                Parallel.For(0,totalPixels, i =>
                 {

                     byte* pixel = (byte*)bmd.Scan0;
                     pixel = pixel + (i * 4);

                     byte b = pixel[0];
                     byte g = pixel[1];
                     byte r = pixel[2];

                     int luma = (int)(r * 0.3 + g * 0.59 + b * 0.11);
                     pixelData[i] = luma;
                 });

            double mean = pixelData.AsParallel().Average();
            double FM = pixelData.AsParallel().Aggregate(0.0, (subtotal, item) => subtotal + ((item - mean) * (item - mean)),
                                                         (total, thisThread) => total + thisThread,
                                                         (finalSum) => Math.Sqrt((finalSum / (pixelData.Length - 1))));

            return Math.Round(FM, 4);
        }


        public static double GetBrightness(Image image)
        {
            Bitmap source = new Bitmap(image);

            Rectangle rect = new Rectangle(0,0, source.Width, source.Height);

            BitmapData bmd = source.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int[] pixelData = new int[source.Width * source.Height];

            System.Runtime.InteropServices.Marshal.Copy(bmd.Scan0, pixelData, 0, pixelData.Length);
            double brightness = 0.0;

            //calculating brightness of the image
            for (int i = 0; i < pixelData.Length; i++)
            {
                Color c = Color.FromArgb(pixelData[i]);
                double temp = ((c.R * c.R * .241f) + (c.G * c.G * .691f) + (c.B * c.B * .068f));
                brightness = Math.Sqrt(temp);
            }
            return brightness;
        }


        // main method that I am using just to test my methods in the class Focus
        static void Main(string[] args)
        {
           
            Image myimage = Image.FromFile(@"C:\Users\payam\Desktop\frame-1.png", true);
           
            var watch2 = Stopwatch.StartNew();
            Focus testImage = new Focus();
            double fvalue = Calculate(myimage);
            
            Console.WriteLine("The second method took {0} miliseconds to be completed", watch2.ElapsedMilliseconds);
            Console.WriteLine("the focus value is {0}", fvalue);
            var watch1 = Stopwatch.StartNew();
            double fvalue2 = CalculatebyPointer(myimage);

            Console.WriteLine("the brightness of your image is {0} at {1} miliseconds", fvalue2,watch1.ElapsedMilliseconds);

    
            Console.ReadLine();


        }
    }
}



