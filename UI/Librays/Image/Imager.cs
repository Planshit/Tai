using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace Core.Librarys.Image
{
    public class Imager
    {
        public static BitmapImage Load(string filePath, string defaultPath = "pack://application:,,,/Tai;component/Resources/Icons/defaultIcon.png")
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    throw new ArgumentNullException(nameof(filePath));
                }

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                if (filePath.IndexOf("pack://") != -1)
                {
                    StreamResourceInfo info = Application.GetResourceStream(new Uri(filePath, UriKind.RelativeOrAbsolute));
                    using (var st = info.Stream)
                    {
                        bitmap.StreamSource = st;
                        bitmap.EndInit();
                        bitmap.Freeze();
                    }
                }
                else
                {
                    if (filePath.IndexOf(":") == -1)
                    {
                        //  不是绝对路径
                        filePath = Path.Combine(FileHelper.GetRootDirectory(), filePath);
                    }

                    if (!File.Exists(filePath))
                    {
                        throw new FileNotFoundException(filePath);
                    }
                    byte[] imageData;
                    using (var fileStream = new FileStream(@filePath, FileMode.Open, FileAccess.Read))

                    using (var binaryReader = new BinaryReader(fileStream))
                    {
                        imageData = binaryReader.ReadBytes((int)fileStream.Length);
                        bitmap.StreamSource = new MemoryStream(imageData);
                        bitmap.EndInit();
                        bitmap.Freeze();
                    }
                }

                return bitmap;
            }
            catch (Exception ec)
            {
                //  无法处理时返回默认图标

                var bitmap = new BitmapImage();
                bitmap.BeginInit();

                StreamResourceInfo info = Application.GetResourceStream(new Uri(defaultPath, UriKind.RelativeOrAbsolute));
                using (var st = info.Stream)
                {
                    bitmap.StreamSource = st;
                    bitmap.EndInit();
                    bitmap.Freeze();
                }

                Logger.Error("无法读取图片：" + filePath + "。" + ec.Message);

                return bitmap;
            }
        }
    }
}
