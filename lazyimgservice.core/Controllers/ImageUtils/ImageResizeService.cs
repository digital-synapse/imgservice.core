using lazyimgservice.core.Models;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace lazyimgservice.core.Controllers
{   
    public class ImageResizeService
    {

        public static bool ResizeImageFile(string inputfile, string outputfile, ImageSize size, out int width, out int height, bool failOnUpSample=true)
        {
            var img = ResizeImageFile(inputfile,size,failOnUpSample);
            width = 0; height = 0;
            if (img == null) return false;
            width = img.Width;
            height = img.Height;
            img.Save(outputfile);
            return true;
        }

        public static Image ResizeImageFile(string inputfile, ImageSize size, bool failOnUpSample=true)
        {
            Image original = Image.FromFile(inputfile);

            // height check
            if (failOnUpSample && original.Size.Height < (int)size) return null;            
            var newSize = new Size((int)size ,(int)size);

            Image resized = ResizeImage(original, newSize);
            MemoryStream memStream = new MemoryStream();
            resized.Save(memStream, ImageFormat.Jpeg);
            return resized;
        }


        public static Image ResizeImageFile(string inputfile, Size size)
        {
            Image original = Image.FromFile(inputfile);
            Image resized = ResizeImage(original, size);
            MemoryStream memStream = new MemoryStream();
            resized.Save(memStream, ImageFormat.Jpeg);
            return resized;
        }

        public static Image ResizeImage(Image image, Size size, bool preserveAspectRatio = true)
        {
            int newWidth;
            int newHeight;
            if (preserveAspectRatio)
            {
                int originalWidth = image.Width;
                int originalHeight = image.Height;
                float percentWidth = (float)size.Width / (float)originalWidth;
                float percentHeight = (float)size.Height / (float)originalHeight;
                float percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                newWidth = (int)Math.Ceiling(originalWidth * percent);
                newHeight = (int)Math.Ceiling(originalHeight * percent);
            }
            else
            {
                newWidth = size.Width;
                newHeight = size.Height;
            }
            Image newImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphicsHandle = Graphics.FromImage(newImage))
            {
                graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }
    }
}
