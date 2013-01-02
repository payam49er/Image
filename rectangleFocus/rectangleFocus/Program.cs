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
    class Focus
    {
        // the focus value calcualted by standard deviation 

        //method to read bitmap image file and convert it to grayscale 
        //public double GetFValue(Image image)
        //{
        //    Bitmap source = new Bitmap(image);
        //    int count = 0;
        //    double total = 0;
        //    double totalVariance = 0;
        //    double FM = 0;
        //    Bitmap bm = new Bitmap(source.Width, source.Height);
        //    Rectangle rect = new Rectangle(0,0,source.Width,source.Height);
        //    Bitmap targetRect = new Bitmap(rect.Width, rect.Height);

        //    // converting to grayscale
        //    for (int y = 0; y < source.Height; y++)
        //    {
        //        for (int x = 0; x < source.Width; x++)
        //        {
        //            count++;
        //            Color c = source.GetPixel(x, y);
        //            int luma = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
        //            source.SetPixel(x, y, Color.FromArgb(luma, luma, luma)); // the image is now gray scaled 
        //            var pixelval = source.GetPixel(x, y);
        //          //  targetRect.Save(@"C:\Users\payam\Desktop\frame-42-rectangle.png", System.Drawing.Imaging.ImageFormat.Png);
        //            int pixelValue = pixelval.G;
        //            total += pixelValue;
        //            double avg = total / count;
        //            totalVariance += Math.Pow(pixelValue - avg, 2);
        //            double stDV = Math.Sqrt(totalVariance / count); // the standard deviation, which is also the focus value
        //            FM = Math.Round(stDV, 2);
        //        }
        //    }
        //    return FM;
        //}

        // this is the fast method that uses standard deviation for calculating focus 
        public Bitmap ConvertToGrayScale(Image image)
        {

            Bitmap myimage = new Bitmap(image);
            Graphics g = Graphics.FromImage(myimage);
            var upbound = image.Height;
            // converting the image to gray scale
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
           return myimage;  // gray scale image 
        }

        public double GetFocusValue(Image image)
        {
            Bitmap source = new Bitmap(image);
            int count = 0;
            double total = 0;
            double totalVariance = 0;
            double FM = 0;
            Bitmap bm = new Bitmap(source.Width,source.Height);
            Rectangle rect = new Rectangle(0,0,source.Width,source.Height);

            BitmapData bmd = source.LockBits(rect,ImageLockMode.ReadWrite,source.PixelFormat);
            int[] pixelData = new int[(rect.Height * rect.Width)-1];
            System.Runtime.InteropServices.Marshal.Copy(bmd.Scan0,pixelData,0,pixelData.Length);

            Parallel.For(0, pixelData.Length, i =>
            {
                count++;
                Color c = Color.FromArgb(pixelData[i]);
                int luma = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                pixelData[i] = Color.FromArgb(luma, luma, luma).ToArgb();
                total += luma;
                double avg = total / count;
                totalVariance += Math.Pow(luma - avg, 2);
                double stDV = Math.Sqrt(totalVariance / count);
                FM = Math.Round(stDV, 2);
            });
            source.UnlockBits(bmd);
            return FM;
        }

  


                
      

        // defining my own bitmap class to have faster access to bitmap data
        public class LockBitMap
        {

            Bitmap source = null;
            IntPtr Iptr = IntPtr.Zero;
            BitmapData bitmapData = null;

            public byte[] Pixels { get; set; }
            public int Depth { get; private set; }
            public int Width { get; private set; }
            public int Height { get; private set; }

            // the constructor of my the LockBitMap class
            public LockBitMap(Bitmap source)
            {
                this.source = source;
            }
            //lock bitmap data

            public void Lockbits()
            {
                try
                {
                    Width = source.Width;
                    Height = source.Height;

                    int Pixelcount = Width * Height;
                    Rectangle rect = new Rectangle(0, 0, Width, Height);
                    Depth = System.Drawing.Bitmap.GetPixelFormatSize(source.PixelFormat);
                    bitmapData = source.LockBits(rect, ImageLockMode.ReadWrite, source.PixelFormat);
                    int step = Depth / 8;
                    Pixels = new byte[Pixelcount * step];
                    Iptr = bitmapData.Scan0;
                    Marshal.Copy(Iptr, Pixels, 0, Pixels.Length);
                }
                catch (Exception exp)
                {
                    throw (exp);
                }
            }
            //unlock bitmap data

            public void UnlockBits()
            {
                try
                {
                    Marshal.Copy(Pixels, 0, Iptr, Pixels.Length);
                    source.UnlockBits(bitmapData);
                }
                catch (Exception exp)
                {
                    throw (exp);
                }
            }

            //getting color of specific pixel, .Net function is damn slow

            public Color GetPixel(int x, int y)
            {
                Color clr = Color.Empty;

                //get color component count
                int cCount = Depth / 8;
                //index of specific pixel

                int i = ((y * Width) + x) * cCount;
                // I am going to do this for 32bit pixels, other cases are slightly different
                byte b = Pixels[i];
                byte g = Pixels[i + 1];
                byte r = Pixels[i + 2];
                byte a = Pixels[i + 3]; // alpha for opacity

                clr = Color.FromArgb(a, r, g, b);
                return clr;
            }

            // now setting the color of an specific pixel

            public void SetPixel(int x, int y, Color color)
            {
                int cCount = Depth / 8;
                int i = ((y * Width) + x) * cCount;
                //I just do it for 32 bit pixel
                Pixels[i] = color.B;
                Pixels[i + 1] = color.G;
                Pixels[i + 2] = color.R;
                Pixels[i + 3] = color.A;
            }

        } //end of LockBitMap class 
    


        // main method that I am using just to test my methods in the class Focus
        static void Main(string[] args)
        {
           
            Focus testimage = new Focus();
            
            Image myimage = Image.FromFile(@"C:\Users\payam\Desktop\frame-1.png", true);
            var watch2 = Stopwatch.StartNew();
            double fvalue = testimage.GetFocusValue(myimage);
       
            Console.WriteLine("The second method too {0} miliseconds to be completed", watch2.ElapsedMilliseconds);
            Console.WriteLine("the focus value is {0}", fvalue);
    
            Console.ReadLine();


        }
    }
}



