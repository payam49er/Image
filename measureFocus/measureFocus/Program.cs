using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing.Drawing2D;

namespace ImageFocus
{
    class Focus
    {
        //Gets the width and height in pixels of the image        


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
            Bitmap bm = new Bitmap(source.Width, source.Height);
            double brenner = 0.0;
            for (int y = 0; y < bm.Height; y++)
            {
                for (int x = 0; x < bm.Width; x++)
                {
                    Color c = source.GetPixel(x, y);
                    int luma = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                    bm.SetPixel(x, y, Color.FromArgb(luma, luma, luma));
                    var p = bm.GetPixel(x, y);
                    var p1 = bm.GetPixel(x + 2, y);
                    var firstValue =(double) p.B;
                    var secondValue =(double) p1.G;

                    brenner += ((secondValue - firstValue) * (secondValue - firstValue));

                    
                }
            }
            return brenner;
        }

        


        //public double GetBrennerFocus(Image source)
        //{
        //    Bitmap bm = new Bitmap(source);
        //    int Hsize = bm.Width;
        //    int Vsize = bm.Height;
        //    double brenner = 0;
        //    Color[][] imageMatrix = new Color[Hsize][];
        //    for (int i = 0; i < Hsize; i++)
        //    {
        //        imageMatrix[i] = new Color[Vsize];
        //        for (int j = 0; j < Vsize; j++)
        //        {
        //            Color c = bm.GetPixel(j, j);
        //            int luma = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
        //            bm.SetPixel(i, j, Color.FromArgb(luma, luma, luma));
        //            imageMatrix[i][j] = bm.GetPixel(i, j);
        //         }
        //    }
            
        //     for(int j=0 ; j<Hsize; j++)
        //    {
        //        for (int i=0; i<Vsize; i++)
        //        {
        //            int[] p = imageMatrix[i];
        //            int[] p1 = imageMatrix.Getvalue(i+2,j);
        //            double brenner += (p1-p)*(p1-p);
                    
                    
        //        }
        //    }

        //   return brenner; 
        //}

             

        // main method that I am using just to test my methods in the class Focus
        static void Main(string[] args)
        {
          Focus testimage = new Focus();

           Image myimage = Image.FromFile(@"C:\Users\payam\Desktop\focusTest.png", true);

           Bitmap bitmapimage = testimage.ConvertToBitmap(myimage);
           var FM = testimage.GetFValue(bitmapimage);
           Console.WriteLine(FM);
            Console.ReadLine();
        }
    }
}
