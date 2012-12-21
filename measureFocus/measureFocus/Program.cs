using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace ImageFocus
{
    class Focus
    {
        // the focus value calcualted by standard deviation 
        

        //method to read image file and convert it to uint8 bitmap
        //this method takes an image as parameter
        private Bitmap ConvertToBitmap(Image source)
        {
            Bitmap image = new Bitmap(source);
            return image;
        }

        //method to read bitmap image file and convert it to grayscale 
        public double GetFValue(Bitmap source)
        {
            int count = 0;
            double total = 0;
            double totalVariance = 0;
            double FM = 0;
            Bitmap bm = new Bitmap(source.Width, source.Height);
          
            // converting to grayscale
            for (int y = 0; y < bm.Height; y++)
            {
                for (int x = 0; x < bm.Width; x++)
                {
                    count++;
                    Color c = source.GetPixel(x, y);
                    int luma = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                    bm.SetPixel(x, y, Color.FromArgb(luma, luma, luma)); // the image is now gray scaled 
                    var pixelval = bm.GetPixel(x, y);
                    int pixelValue = pixelval.G;
                    total += pixelValue;
                    double avg = total / count;
                    totalVariance += Math.Pow(pixelValue - avg, 2);
                    double stDV = Math.Sqrt(totalVariance / count); // the standard deviation, which is also the focus value
                    FM = Math.Round(stDV, 2);           
                }
            }
            return FM;
        }














        // main method that I am using just to test my methods in the class Focus
        static void Main(string[] args)
        {
          Focus testimage = new Focus();

           Image myimage = Image.FromFile(@"C:\Users\payam\Desktop\frame-42.png", true);
           var watch = Stopwatch.StartNew();
           Bitmap bitmapimage = testimage.ConvertToBitmap(myimage);
           var FM = testimage.GetFValue(bitmapimage);
           Console.WriteLine(FM);
           Console.WriteLine(watch.ElapsedMilliseconds);
           Console.ReadLine();
        }
    }
}
