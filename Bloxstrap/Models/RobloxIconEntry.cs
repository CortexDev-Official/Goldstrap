using System.Windows.Media;

namespace Bloxstrap.Models
{
    public class RobloxIconEntry
    {
        private static readonly Dictionary<RobloxIcon, ImageSource> _cache = new();

        public RobloxIcon IconType { get; set; }

        public ImageSource ImageSource
        {
            get
            {
                if (IconType == RobloxIcon.IconCustom)
                    return IconType.GetIcon().GetImageSource();

                if (!_cache.TryGetValue(IconType, out ImageSource? imageSource))
                {
                    imageSource = IconType.GetIcon().GetImageSource();
                    _cache[IconType] = imageSource;
                }

                return imageSource;
            }
        }
    }
}
