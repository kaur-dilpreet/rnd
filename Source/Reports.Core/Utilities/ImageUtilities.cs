using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Web;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;

namespace Reports.Core.Utilities
{
    public class ImageUtilities : IImageUtilities
    {
        private readonly Core.ErrorHandling.IErrorHandler ErrorHandler;
        private readonly Core.Utilities.IUtilities Utilities;

        public ImageUtilities(Core.ErrorHandling.IErrorHandler errorHandler,
                              Core.Utilities.IUtilities utilities)
        {
            ErrorHandler = errorHandler;
            Utilities = utilities;
        }

        public void SavePNGFromBase64 (String image, String targetFile)
        {
            String dataWithoutPngMarker = image.Replace("data:image/png;base64,", String.Empty);
            Byte[] filebytes = Convert.FromBase64String(dataWithoutPngMarker);

            using (FileStream fs = new FileStream(targetFile,
                                            FileMode.OpenOrCreate,
                                            FileAccess.Write,
                                            FileShare.None))
            {
                fs.Write(filebytes, 0, filebytes.Length);
            }
        }

        public void ReadImageLatitudeAndLongitude(string fileAbsolutePath)
        {
            string physicalLocation = fileAbsolutePath;
            if (File.Exists(physicalLocation))
            {
                Bitmap bmp = new Bitmap(physicalLocation);
                // set Variable Values
                double? latitdue = null;
                double? longitude = null;

                foreach (PropertyItem propItem in bmp.PropertyItems)
                {
                    switch (propItem.Type)
                    {
                        case 5:
                            if (propItem.Id == 2) // Latitude Array
                                latitdue = GetLatitudeAndLongitude(propItem);
                            if (propItem.Id == 4) //Longitude Array
                                longitude = GetLatitudeAndLongitude(propItem);
                            break;

                    }
                }

                //Response.Write("Latitude :- " + latitdue);
                //Response.Write("<br/>");
                //Response.Write("Latitude :- " + longitude);
            }
        } // ReadImageLatitudeAndLongitude

        private static double? GetLatitudeAndLongitude(PropertyItem propItem)
        {
            try
            {
                uint degreesNumerator = BitConverter.ToUInt32(propItem.Value, 0);
                uint degreesDenominator = BitConverter.ToUInt32(propItem.Value, 4);
                uint minutesNumerator = BitConverter.ToUInt32(propItem.Value, 8);
                uint minutesDenominator = BitConverter.ToUInt32(propItem.Value, 12);
                uint secondsNumerator = BitConverter.ToUInt32(propItem.Value, 16);
                uint secondsDenominator = BitConverter.ToUInt32(propItem.Value, 20);
                return (Convert.ToDouble(degreesNumerator) / Convert.ToDouble(degreesDenominator)) + (Convert.ToDouble(Convert.ToDouble(minutesNumerator) / Convert.ToDouble(minutesDenominator)) / 60) +
                       (Convert.ToDouble((Convert.ToDouble(secondsNumerator) / Convert.ToDouble(secondsDenominator)) / 3600));
            }
            catch (Exception)
            {

                return null;
            }
        } // GetLatitudeAndLongitude

        public static Boolean IsAnimated (String imageRelativePath)
        {
            Image i = Image.FromFile(System.Web.HttpContext.Current.Server.MapPath(imageRelativePath));

            FrameDimension FrameDimensions = new FrameDimension(i.FrameDimensionsList[0]);

            return i.GetFrameCount(FrameDimensions) > 1;
        }

        public Boolean IsUploadedFileAnImage(HttpPostedFileBase file)
        {
            try
            {
                // check the extention
                String extention = Path.GetExtension(file.FileName).ToLower();
                String pattern = @"^(.jpg|.jpeg|.png|.gif)$";
                Regex regex = new Regex(pattern);

                if (!regex.Match(extention).Success)
                    return false;

                // check the data stream 
                Bitmap img = new Bitmap(file.InputStream);

                img.Dispose();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public Boolean IsFileAnImage(String filePath)
        {
            try
            {
                // check the extention
                String extention = Path.GetExtension(filePath);
                String pattern = @"^(.jpg|.jpeg|.png|.gif)$";
                Regex regex = new Regex(pattern);

                if (!regex.Match(extention).Success)
                    return false;

                // check the data stream 
                Bitmap img = new Bitmap(filePath);

                img.Dispose();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public Size GetImageSize(HttpPostedFileBase file)
        {
            try
            {
                Bitmap img = new Bitmap(file.InputStream);

                Size size = img.Size;

                img.Dispose();
                img = null;

                return size;
            }
            catch (Exception ex)
            {
                throw this.ErrorHandler.HandleError(ex, String.Format("Core.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, null);
            }
        }

        public Size GetImageSize(String imagePath)
        {
            try
            {
                System.IO.FileStream bitmapFile = new System.IO.FileStream(imagePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
                Image img = new Bitmap(bitmapFile);

                Size size = img.Size;

                bitmapFile.Close();
                bitmapFile.Dispose();
                bitmapFile = null;
                img.Dispose();
                img = null;

                return size;
            }
            catch (Exception ex)
            {
                NameValueCollection methodParam = new NameValueCollection();
                methodParam.Add("imagePath", imagePath);
                throw this.ErrorHandler.HandleError(ex, String.Format("Core.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }

        private Image CropImage(Image img, Rectangle cropArea)
        {
            try
            {
                Image croppedImg = new Bitmap(cropArea.Width, cropArea.Height);

                Graphics gfx = Graphics.FromImage(croppedImg);
                gfx.CompositingQuality = CompositingQuality.HighQuality;
                gfx.SmoothingMode = SmoothingMode.HighQuality;

                gfx.DrawImage(img, new Rectangle(0, 0, cropArea.Width, cropArea.Height), cropArea, GraphicsUnit.Pixel);

                gfx.Dispose();

                return croppedImg;
            }
            catch (Exception ex)
            {
                NameValueCollection methodParam = new NameValueCollection();
                methodParam.Add("cropArea", Utilities.Serialize(cropArea));
                throw this.ErrorHandler.HandleError(ex, String.Format("Core.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }

        private Image ResizeImage(Image imgToResize, Size size)
        {
            try
            {
                int sourceWidth = imgToResize.Width;
                int sourceHeight = imgToResize.Height;

                float nPercent = 0;
                float nPercentW = 0;
                float nPercentH = 0;

                nPercentW = ((float)size.Width / (float)sourceWidth);
                nPercentH = ((float)size.Height / (float)sourceHeight);

                if (nPercentH < nPercentW)
                    nPercent = nPercentH;
                else
                    nPercent = nPercentW;

                int destWidth = (int)(sourceWidth * nPercent);
                int destHeight = (int)(sourceHeight * nPercent);

                Bitmap b = new Bitmap(destWidth, destHeight);
                Graphics g = Graphics.FromImage((Image)b);
                g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                g.CompositingQuality = CompositingQuality.HighQuality;

                g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
                g.Dispose();

                return (Image)b;
            }
            catch (Exception ex)
            {
                NameValueCollection methodParam = new NameValueCollection();
                methodParam.Add("size", Utilities.Serialize(size));
                throw this.ErrorHandler.HandleError(ex, String.Format("Core.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }

        public void ResizeSaveImage(String sourcePath, String destinationPath, Size imageSize, Boolean deleteSource)
        {
            try
            {
                System.IO.FileStream bitmapFile = new System.IO.FileStream(sourcePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
                Image img = new Bitmap(bitmapFile);

                img = ResizeImage(img, imageSize);

                img.Save(destinationPath, System.Drawing.Imaging.ImageFormat.Jpeg);

                bitmapFile.Close();
                bitmapFile.Dispose();
                bitmapFile = null;
                img.Dispose();
                img = null;

                if (deleteSource)
                {
                    try
                    {
                        System.IO.File.Delete(sourcePath);
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                NameValueCollection methodParam = new NameValueCollection();
                methodParam.Add("sourcePath", sourcePath);
                methodParam.Add("destinationPath", destinationPath);
                methodParam.Add("imageSize", Utilities.Serialize(imageSize));
                methodParam.Add("deleteSource", deleteSource.ToString());
                throw this.ErrorHandler.HandleError(ex, String.Format("Core.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }

        public void CropResizeSaveImage(String sourcePath, String destinationPath, Size newSize, Rectangle cropRect, Boolean deleteSource)
        {
            try
            {
                System.IO.FileStream bitmapFile = new System.IO.FileStream(sourcePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
                Image img = new Bitmap(bitmapFile);

                //img = ResizeImage(img, new Size(405, 405));
                img = ResizeImage(img, newSize);
                img = CropImage(img, cropRect);

                img.Save(destinationPath, System.Drawing.Imaging.ImageFormat.Png);

                bitmapFile.Close();
                bitmapFile.Dispose();
                bitmapFile = null;
                img.Dispose();
                img = null;

                if (deleteSource)
                {
                    try
                    {
                        System.IO.File.Delete(sourcePath);
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                NameValueCollection methodParam = new NameValueCollection();
                methodParam.Add("sourcePath", sourcePath);
                methodParam.Add("destinationPath", destinationPath);
                methodParam.Add("imageSize", Utilities.Serialize(newSize));
                methodParam.Add("cropRect", Utilities.Serialize(cropRect));
                methodParam.Add("deleteSource", deleteSource.ToString());
                throw this.ErrorHandler.HandleError(ex, String.Format("Core.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }

        public void CropResizeSaveImage(String sourcePath, String destinationPath, Size imageSize, Boolean deleteSource)
        {
            try
            {
                System.IO.FileStream bitmapFile = new System.IO.FileStream(sourcePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
                Image img = new Bitmap(bitmapFile);

                //img = ResizeImage(img, new Size(405, 405));
                img = ResizeImage(img, imageSize);
                img = CropImage(img, new Rectangle(0, 0, Math.Min(imageSize.Width, img.Width), Math.Min(imageSize.Height, img.Height)));

                img.Save(destinationPath, System.Drawing.Imaging.ImageFormat.Jpeg);

                bitmapFile.Close();
                bitmapFile.Dispose();
                bitmapFile = null;
                img.Dispose();
                img = null;

                if (deleteSource)
                {
                    try
                    {
                        System.IO.File.Delete(sourcePath);
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                NameValueCollection methodParam = new NameValueCollection();
                methodParam.Add("sourcePath", sourcePath);
                methodParam.Add("destinationPath", destinationPath);
                methodParam.Add("imageSize", Utilities.Serialize(imageSize));
                methodParam.Add("deleteSource", deleteSource.ToString());
                throw this.ErrorHandler.HandleError(ex, String.Format("Core.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }
    }
}

