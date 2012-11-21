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
            Bitmap bitmap = new Bitmap(source);
            return bitmap;
        }

        //method to read bitmap image file and convert it to grayscale 
        public Bitmap ConvertToGrayScale(Bitmap source)
        {
            Bitmap bm = new Bitmap(source.Width, source.Height);
            for (int y = 0; y < bm.Height; y++)
            {
                for (int x = 0; x < bm.Width; x++)
                {
                    Color c = source.GetPixel(x, y);
                    int luma = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                    bm.SetPixel(x, y, Color.FromArgb(luma, luma, luma));
                }
            }
            return bm;
        }

        public Color[][] GetImageMatrix(Image source)
        {
            Bitmap bm = new Bitmap(source);
            int Hsize = bm.Width;
            int Vsize = bm.Height;

            Color[][] imageMatrix = new Color[Hsize][];
            for (int i = 0; i < Hsize; i++)
            {
                imageMatrix[i] = new Color[Vsize];
                for (int j = 0; j < Vsize; j++)
                {
                    Color c = bm.GetPixel(j, j);
                    int luma = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                    bm.SetPixel(i, j, Color.FromArgb(luma, luma, luma));
                    imageMatrix[i][j] = bm.GetPixel(i, j);
                }
            }
            return imageMatrix;
        }

        // main method that I am using just to test my methods in the class Focus
        static void Main(string[] args)
        {

            Focus testimage = new Focus();

            Image myimage = Image.FromFile(@"C:\Users\payam\Desktop\focusTest.png", true);
            

            var result = testimage.GetImageMatrix(myimage);
            // Bitmap grayimage = testimage.ConvertToGrayScale(result);
            Console.Write(result);
            Console.ReadLine();
        }
    }
}
