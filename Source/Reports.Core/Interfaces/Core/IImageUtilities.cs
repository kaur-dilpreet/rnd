using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Drawing;

namespace Reports.Core.Utilities
{
    public interface IImageUtilities
    {
        void SavePNGFromBase64(String image, String targetFile);
        Boolean IsUploadedFileAnImage(HttpPostedFileBase file);
        Boolean IsFileAnImage(String filePath);
        Size GetImageSize(HttpPostedFileBase file);
        Size GetImageSize(String imagePath);
        void ResizeSaveImage(String sourcePath, String destinationPath, Size imageSize, Boolean deleteSource);
        void CropResizeSaveImage(String sourcePath, String destinationPath, Size newSize, Rectangle cropRect, Boolean deleteSource);
        void CropResizeSaveImage(String sourcePath, String destinationPath, Size imageSize, Boolean deleteSource);
    }
}
