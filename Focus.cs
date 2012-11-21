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
           Bitmap bm = new Bitmap(source.Width,source.Height);
           for (int y=0 ; y<bm.Height; y++)
           {
               for (int x=0 ; x<bm.Width;x++)
               {
                   Color c =source.GetPixel(x,y);
                   int luma = (int)(c.R*0.3 + c.G*0.59+c.B*0.11);
                   bm.SetPixel(x,y,Color.FromArgb(luma,luma,luma));
               }
           }
            return bm;
        }

       //method to read an image and save it in Bitmap
         public Matrix readImage(Image source)
         {   
             Matrix imageMatrix = new Matrix();
             Bitmap image = new Bitmap(source);
            

             for (int x = 0; x < image.Width; x++)
             {
                 imageMatrix[x,] = new Bitmap[image.Height];
                 for (int y = 0; y < image.Height; y++)
                 {
                     imageMatrix[x][y] =(Bitmap) image.Clone();
                 }
             }
             return imageMatrix;
         }
    
        
        static void Main(string[] args)
        {

            Focus testimage = new Focus();

            Image myimage = Image.FromFile(@"C:\Users\payam\Desktop\focusTest.png",true);

        
          // Bitmap result = testimage.ConvertToBitmap(myimage);
          // Bitmap grayimage = testimage.ConvertToGrayScale(result);
            Bitmap[][] matrix = testimage.readImage(myimage);
            var len = matrix.Length;
            
            Console.WriteLine(len);
            Console.ReadLine();
        }
    }
}
