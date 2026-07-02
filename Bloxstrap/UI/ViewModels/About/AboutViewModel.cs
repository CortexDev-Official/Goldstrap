using System.Windows;

namespace Bloxstrap.UI.ViewModels.About
{
    public class AboutViewModel : NotifyPropertyChangedViewModel
    {
        private const string PrereleaseLabel = "alpha";
        public string VersionPrefix => string.Format(Strings.Menu_About_Version, App.Version.Replace("-" + PrereleaseLabel, ""));
        public string VersionSuffix => App.Version.EndsWith("-" + PrereleaseLabel) ? PrereleaseLabel : "";

        public BuildMetadataAttribute BuildMetadata => App.BuildMetadata;
            
        public string BuildTimestamp => BuildMetadata.Timestamp.ToFriendlyString();
        public string BuildCommitHashUrl => $"https://github.com/{App.ProjectRepository}/commit/{BuildMetadata.CommitHash}";

        public Visibility BuildInformationVisibility => App.IsProductionBuild ? Visibility.Collapsed : Visibility.Visible;
        public Visibility BuildCommitVisibility => App.IsActionBuild ? Visibility.Visible : Visibility.Collapsed;
    }
}
