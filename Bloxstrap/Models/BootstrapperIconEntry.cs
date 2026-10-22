using System.Windows.Media;

namespace Bloxstrap.Models
{
    public class BootstrapperIconEntry
    {
        private static readonly Dictionary<BootstrapperIcon, ImageSource> _cache = new();

        public BootstrapperIcon IconType { get; set; }

        public ImageSource ImageSource
        {
            get
            {
                if (IconType == BootstrapperIcon.IconCustom)
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
