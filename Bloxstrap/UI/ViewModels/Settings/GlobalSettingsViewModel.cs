using System.Windows;

using Bloxstrap.Enums.GBSPresets;

namespace Bloxstrap.UI.ViewModels.Settings
{
    public class GlobalSettingsViewModel : NotifyPropertyChangedViewModel
    {
        public bool ReadOnly
        {
            get => App.GlobalSettings.GetReadOnly();
            set => App.GlobalSettings.SetReadOnly(value);
        }

        public int FramerateCap
        {
            get
            {
                if (int.TryParse(App.GlobalSettings.GetPreset("Rendering.FramerateCap"), out int framerate))
                {
                    
                    if (framerate < 1)
                        return 60;
                    else
                        return framerate;
                }
                else
                    return 60;
            }
            set
            {
                
                if (value < 1)
                    value = -1;
                
                App.GlobalSettings.SetPreset("Rendering.FramerateCap", value);
            }
        }

        public string UITransparency
        {
            get => App.GlobalSettings.GetPreset("UI.Transparency")!;
            set
            {
                App.GlobalSettings.SetPreset("UI.Transparency", value.Length >= 3 ? value[..3] : value); 

                OnPropertyChanged(nameof(UITransparency));
            }
        }

        
        
        
        
        private const int MinQualityLevel = 1;
        private const int MaxQualityLevel = 21;
        private const int MaxSavedQualityLevel = 10;

        private static int ToGraphicsQualityLevel(int savedLevel) =>
            (int)Math.Round((savedLevel - 1) * (double)(MaxQualityLevel - MinQualityLevel) / (MaxSavedQualityLevel - 1)) + MinQualityLevel;

        private static int ToSavedQualityLevel(int graphicsLevel) =>
            (int)Math.Round((graphicsLevel - MinQualityLevel) * (double)(MaxSavedQualityLevel - 1) / (MaxQualityLevel - MinQualityLevel)) + 1;

        public int GraphicsQuality
        {
            get
            {
                if (!int.TryParse(App.GlobalSettings.GetPreset("Rendering.SavedQualityLevel"), out int savedLevel))
                    return 0; 

                if (savedLevel <= 0)
                    return 0;

                
                
                if (savedLevel >= MaxSavedQualityLevel
                    && int.TryParse(App.GlobalSettings.GetPreset("Rendering.GraphicsQualityLevel"), out int graphicsLevel)
                    && graphicsLevel >= MinQualityLevel && graphicsLevel < MaxQualityLevel)
                {
                    return Math.Clamp(ToSavedQualityLevel(graphicsLevel), 1, MaxSavedQualityLevel);
                }

                return Math.Min(savedLevel, MaxSavedQualityLevel);
            }

            set
            {
                int savedLevel = Math.Clamp(value, 0, MaxSavedQualityLevel);

                App.GlobalSettings.SetPreset("Rendering.SavedQualityLevel", savedLevel);

                
                
                App.GlobalSettings.SetPreset("Rendering.MaxQualityEnabled", false);

                if (savedLevel > 0)
                    App.GlobalSettings.SetPreset("Rendering.GraphicsQualityLevel", ToGraphicsQualityLevel(savedLevel));

                
                
                
                App.FastFlags.SetPreset("Rendering.FRMQualityOverride", null);

                OnPropertyChanged(nameof(GraphicsQuality));
                OnPropertyChanged(nameof(QualityOverrideActive));
            }
        }

        /// <summary>
        /// True while the FRM quality override fastflag is set, which pins roblox's render quality
        /// and greys out the in-game graphics slider no matter what's configured here.
        /// </summary>
        public bool QualityOverrideActive =>
            App.FastFlags.GetValue("DFIntDebugFRMQualityLevelOverride") is not null;

        public Visibility QualityOverrideVisibility =>
            QualityOverrideActive ? Visibility.Visible : Visibility.Collapsed;

        public bool ReducedMotion
        {
            get => App.GlobalSettings.GetPreset("UI.ReducedMotion")?.ToLowerInvariant() == "true";
            set => App.GlobalSettings.SetPreset("UI.ReducedMotion", value);
        }

        public IReadOnlyDictionary<FontSize, string?> FontSizes => GlobalSettingsManager.FontSizes;
        public FontSize SelectedFontSize
        {
            get => FontSizes.FirstOrDefault(x => x.Value == App.GlobalSettings.GetPreset("UI.FontSize")).Key;
            set => App.GlobalSettings.SetPreset("UI.FontSize", FontSizes[value]);
        }

        public string MouseSensitivity
        {
            get => App.GlobalSettings.GetPreset("User.MouseSensitivity")!;
            set => App.GlobalSettings.SetPreset("User.MouseSensitivity", value);
        }

        public string VREnabled
        {
            get => App.GlobalSettings.GetPreset("User.VREnabled")!;
            set => App.GlobalSettings.SetPreset("User.VREnabled", value);
        }
    }
}