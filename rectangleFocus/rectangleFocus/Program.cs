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

namespace ImageFocus
{
    class Focus
    {
        // the focus value calcualted by standard deviation 


        //method to read bitmap image file and convert it to grayscale 
        public double GetFValue(Image image)
        {
            Bitmap source = new Bitmap(image);
            int count = 0;
            double total = 0;
            double totalVariance = 0;
            double FM = 0;
             Bitmap bm = new Bitmap(source.Width, source.Height);
             Rectangle rect = new Rectangle(0,0,source.Width,source.Height);
              Bitmap targetRect = new Bitmap(rect.Width, rect.Height);

            // converting to grayscale
            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    count++;
                    Color c = source.GetPixel(x, y);
                    int luma = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                    source.SetPixel(x, y, Color.FromArgb(luma, luma, luma)); // the image is now gray scaled 
                    var pixelval = source.GetPixel(x, y);
                  //  targetRect.Save(@"C:\Users\payam\Desktop\frame-42-rectangle.png", System.Drawing.Imaging.ImageFormat.Png);
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

        public double CalculateF(Image image)
        {

            Bitmap myimage = new Bitmap(image);
            Graphics g = Graphics.FromImage(myimage);
            var upbound = image.Height;

            ColorMatrix colorMatrix = new ColorMatrix(
                new float[][]
                {
                    new float[] {0.3f, 0.3f, 0.3f, 0, 0},
                    new float[] {0.59f,0.59f, 0.59f, 0, 0},
                    new float[] {0.11f, 0.11f, 0.11f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
                });
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(colorMatrix);
            g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, myimage.Width, myimage.Height, GraphicsUnit.Pixel, attributes);
            BitmapData Data = myimage.LockBits(new Rectangle(0, 0, myimage.Width, myimage.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, myimage.PixelFormat);
            byte[] myimageData = new byte[Data.Stride * Data.Height];
            
            myimage.UnlockBits(Data);
            g.Dispose();

            



            double total = 0;
            int count = 0;
            double totalVariance = 0;
            double FM = 0;
             for (int y = 0; y < myimage.Height; y++)
            {
                for (int x = 0; x < myimage.Width; x++)
                {
                    count++;
                    var pixelval = myimage.GetPixel(x, y);
                    int pixelValue = pixelval.G;
                    total += pixelValue;
                    double avg = total / count;
                    totalVariance += Math.Pow(pixelValue - avg, 2);
                    double stDV = Math.Sqrt(totalVariance / count); // the standard deviation, which is also the focus value
                    FM = Math.Round(stDV, 2);
                }
            }
             return Math.Round(FM, 3);
        }

        

           



        // main method that I am using just to test my methods in the class Focus
        static void Main(string[] args)
        {
            Focus testimage = new Focus();
            Image myimage = Image.FromFile(@"C:\Users\payam\Desktop\frame-42.png", true);
            //var watch = Stopwatch.StartNew();
            //var FM = testimage.GetFValue(myimage);
            //Console.WriteLine(FM);
            //Console.WriteLine("The time it takes to do this is {0} miliseconds", watch.ElapsedMilliseconds);

            var watch2 = Stopwatch.StartNew();
            var focusval = testimage.CalculateF(myimage);
            Console.WriteLine("the value of Focus is :{0}", focusval);
            Console.WriteLine("The second method too {0} miliseconds to be completed", watch2.ElapsedMilliseconds);
            //grayimage.Save(@"C:\Users\payam\Desktop\mygrayImage.png");
            


            //var watch3 = Stopwatch.StartNew();
            //var grayimage2 = testimage.ConvertToGrayFast(myimage);
            //Console.WriteLine("The third method too {0} miliseconds to be completed", watch3.ElapsedMilliseconds);
            //grayimage2.Save(@"C:\Users\payam\Desktop\mygrayImage3.png");
            Console.ReadLine();


        }
    }
}



