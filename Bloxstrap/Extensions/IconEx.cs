using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Bloxstrap.Extensions
{
    public static class IconEx
    {
        public static Icon GetSized(this Icon icon, int width, int height) => new(icon, new Size(width, height));

        public static ImageSource GetImageSource(this Icon icon, bool handleException = true)
        {
            using MemoryStream stream = new();
            icon.Save(stream);
            stream.Seek(0, SeekOrigin.Begin);

            if (handleException)
            {
                try
                {
                    return DecodeIconStream(stream);
                }
                catch (Exception ex)
                {
                    App.Logger.WriteException("IconEx::GetImageSource", ex);
                    Frontend.ShowMessageBox(string.Format(Strings.Dialog_IconLoadFailed, ex.Message));
                    return BootstrapperIcon.IconBloxstrap.GetIcon().GetImageSource(false);
                }
            }
            else
            {
                return DecodeIconStream(stream);
            }
        }

        private static ImageSource DecodeIconStream(MemoryStream stream)
        {
            var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

            if (decoder.Frames.Count > 0)
            {
                // Pick the largest frame for crispest rendering
                return decoder.Frames.OrderByDescending(f => f.PixelWidth).First();
            }

            // Should never happen, but just in case
            stream.Seek(0, SeekOrigin.Begin);
            return BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
        }
    }
}