using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace FCSPlayout.AppInfrastructure
{
    public class PhotoResizer
    {
        // Helper Functions
        public static byte[] GetImageBytes(byte[] imageBytes, int targetSize)
        {
            using (var m = new MemoryStream(imageBytes))
            {
                return GetImageBytes(m, targetSize);
            }
        }

        public static byte[] GetImageBytes(string imageFilename, int targetSize)
        {
            using (var fs = new FileStream(imageFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return GetImageBytes(fs, targetSize);
            }
        }

        public static byte[] GetImageBytes(Stream imageStream, int targetSize)
        {
            using (MemoryStream m = new MemoryStream())
            {
                SaveImage(imageStream, targetSize, m);
                return m.GetBuffer();
            }
        }

        public static byte[] GetImageBytes(Image oldImage, int targetSize)
        {
            using (MemoryStream m = new MemoryStream())
            {
                SaveImage(oldImage, targetSize, m);
                return m.GetBuffer();
            }
        }

        public static void SaveImage(Stream imageStream, int targetSize, Stream targetStream)
        {
            using (Image oldImage = Image.FromStream(imageStream))
            {
                SaveImage(oldImage, targetSize, targetStream);
            }
        }

        

        public static void SaveImage(Image oldImage, int targetSize, Stream targetStream)
        {
            using (Bitmap newImage = ResizeImage(oldImage, targetSize))
            {
                newImage.Save(targetStream, ImageFormat.Jpeg);
            }
        }

        public static Bitmap ResizeImage(Image oldImage, int targetSize)
        {
            Size newSize = CalculateDimensions(oldImage.Size, targetSize);
            Bitmap newImage = new Bitmap(newSize.Width, newSize.Height, PixelFormat.Format24bppRgb);
            using (Graphics canvas = Graphics.FromImage(newImage))
            {
                canvas.SmoothingMode = SmoothingMode.AntiAlias;
                canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                canvas.PixelOffsetMode = PixelOffsetMode.HighQuality;
                canvas.DrawImage(oldImage, new Rectangle(new Point(0, 0), newSize));
                return newImage;
            }
        }

        //public static byte[] ResizeImageFile(byte[] imageFile, PhotoSize size)
        //{
        //    return ResizeImageFile(imageFile, GetSize(size));
        //}

        //public static byte[] ResizeImageFile(string imageFile, PhotoSize size)
        //{
        //    return ResizeImageFile(imageFile, GetSize(size));
        //}

        //public static byte[] ResizeImageFile(Stream imageStream, PhotoSize size)
        //{
        //    return ResizeImageFile(imageStream, GetSize(size));
        //}

        //public static void ResizeImageFile(Stream imageStream, PhotoSize size, Stream targetStream)
        //{
        //    ResizeImageFile(imageStream, GetSize(size), targetStream);
        //}


        //private static int GetSize(PhotoSize size)
        //{
        //    switch (size)
        //    {
        //        case PhotoSize.Large:
        //            return 600;
        //        case PhotoSize.Medium:
        //            return 198;
        //        case PhotoSize.Small:
        //            return 100;
        //        default:
        //            throw new ArgumentException();
        //    }
        //}

        private static Size CalculateDimensions(Size oldSize, int targetSize)
        {
            Size newSize = new Size();
            if (oldSize.Height > oldSize.Width)
            {
                newSize.Width = (int)(oldSize.Width * ((float)targetSize / (float)oldSize.Height));
                newSize.Height = targetSize;
            }
            else
            {
                newSize.Width = targetSize;
                newSize.Height = (int)(oldSize.Height * ((float)targetSize / (float)oldSize.Width));
            }
            return newSize;
        }
    }
}
