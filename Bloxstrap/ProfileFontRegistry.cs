using System.Windows;
using System.Windows.Media;

using Bloxstrap.Models.Entities;

namespace Bloxstrap
{
    internal static class ProfileFontRegistry
    {
        public static IReadOnlyList<ProfileFont> Fonts { get; } = new List<ProfileFont>
        {
            new()
            {
                Id = "default",
                DisplayName = "Default",
                IsDefault = true
            },
            new()
            {
                Id = "rubik",
                DisplayName = "Rubik",
                FamilyName = "Rubik Light",
                ResourcePath = "Resources/Fonts/Rubik-VariableFont_wght.ttf"
            },
            new()
            {
                Id = "cascadia",
                DisplayName = "Cascadia Code",
                FamilyName = "Cascadia Code",
                ResourcePath = "Resources/Fonts/CascadiaCode.ttf"
            }
        };

        public static ProfileFont Default => Fonts.FirstOrDefault(x => x.IsDefault) ?? Fonts[0];

        public static ProfileFont Get(string? id) => Fonts.FirstOrDefault(x => x.Id == id) ?? Default;

        public static System.Windows.Media.FontFamily? Resolve(string? id)
        {
            var font = Get(id);

            if (font.IsDefault || String.IsNullOrEmpty(font.FamilyName) || String.IsNullOrEmpty(font.ResourcePath))
                return null;

            try
            {
                string resourceUri = "pack://application:,,,/" + font.ResourcePath;

                if (Application.GetResourceStream(new Uri(resourceUri)) is null)
                {
                    App.Logger.WriteLine("ProfileFontRegistry::Resolve", $"Font resource missing for '{font.Id}', falling back to default");
                    return null;
                }

                string folder = resourceUri[..(resourceUri.LastIndexOf('/') + 1)];

                return new System.Windows.Media.FontFamily(new Uri(folder), "./#" + font.FamilyName);
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("ProfileFontRegistry::Resolve", $"Failed to load font '{font.Id}', falling back to default");
                App.Logger.WriteException("ProfileFontRegistry::Resolve", ex);
                return null;
            }
        }
    }
}
