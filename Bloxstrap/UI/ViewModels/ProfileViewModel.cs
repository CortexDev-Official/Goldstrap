using System.Windows.Input;
using System.Windows.Media;

using Bloxstrap.Models.Entities;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace Bloxstrap.UI.ViewModels
{
    public class ProfileViewModel : NotifyPropertyChangedViewModel
    {
        private string _username;

        public ProfileViewModel()
        {
            _username = App.Profile.GetUsername();
        }

        public ICommand ChooseAvatarCommand => new RelayCommand(ChooseAvatar);

        public string Username
        {
            get => _username;
            set
            {
                string username = value ?? "";

                if (_username == username)
                    return;

                _username = username;
                OnPropertyChanged(nameof(Username));
                OnPropertyChanged(nameof(DisplayName));
                OnPropertyChanged(nameof(WelcomeText));
            }
        }

        public string DisplayName
        {
            get
            {
                string username = _username.Trim();
                return username.Length == 0 ? Strings.Profile_DefaultUsername : username;
            }
        }

        public string WelcomeText => String.Format(Strings.Profile_Welcome, DisplayName);

        public ImageSource? AvatarSource => HasAvatar && !IsAvatarAnimated ? App.Profile.GetAvatarImage() : null;

        public Uri? AvatarUri => IsAvatarAnimated ? App.Profile.AvatarUri : null;

        public bool IsAvatarAnimated => App.Profile.IsAvatarAnimated;

        public bool HasAvatar => App.Profile.HasAvatar;

        public bool IsVerified
        {
            get => App.Profile.Prop.VerifiedBadge;
            set
            {
                App.Profile.SetVerified(value);
                OnPropertyChanged(nameof(IsVerified));
            }
        }

        public IReadOnlyList<ProfileFont> Fonts => ProfileFontRegistry.Fonts;

        public ProfileFont SelectedFont
        {
            get => ProfileFontRegistry.Get(App.Profile.Prop.FontId);
            set
            {
                App.Profile.SetFont(value?.Id);
                OnPropertyChanged(nameof(SelectedFont));
                OnPropertyChanged(nameof(UsernameFont));
            }
        }

        public System.Windows.Media.FontFamily? UsernameFont => ProfileFontRegistry.Resolve(App.Profile.Prop.FontId);

        public string StreakText => String.Format(Strings.Profile_Streak_Format, App.Profile.Prop.Streak);

        public void CommitUsername()
        {
            App.Profile.SetUsername(_username);
            _username = App.Profile.GetUsername();

            OnPropertyChanged(nameof(Username));
            OnPropertyChanged(nameof(DisplayName));
            OnPropertyChanged(nameof(WelcomeText));
        }

        private void ChooseAvatar()
        {
            var dialog = new OpenFileDialog
            {
                Title = Strings.Profile_Avatar_Title,
                Filter = Strings.Profile_Avatar_Filter
            };

            if (dialog.ShowDialog() != true)
                return;

            App.Profile.SetAvatar(dialog.FileName);

            OnPropertyChanged(nameof(AvatarSource));
            OnPropertyChanged(nameof(AvatarUri));
            OnPropertyChanged(nameof(IsAvatarAnimated));
            OnPropertyChanged(nameof(HasAvatar));
        }
    }
}
