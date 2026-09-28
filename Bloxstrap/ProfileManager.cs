using System.Windows.Media.Imaging;

using Bloxstrap.Models.Persistable;

namespace Bloxstrap
{
    public class ProfileManager : JsonManager<ProfileSettings>
    {
        private const string DateFormat = "yyyy-MM-dd";
        private const string DefaultUsername = "User";
        private const int MaxUsernameLength = 20;

        private static readonly string[] AllowedAvatarExtensions = { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".webp" };

        public override string ClassName => "Profile";

        public string ProfileDirectory => Path.Combine(Paths.Base, "Profile");

        public void RegisterDailyOpen()
        {
            const string LOG_IDENT = "ProfileManager::RegisterDailyOpen";

            DateTime today = DateTime.Today;

            if (!DateTime.TryParseExact(Prop.LastOpenDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime lastOpen))
            {
                Prop.Streak = 1;
                Prop.LastOpenDate = today.ToString(DateFormat, CultureInfo.InvariantCulture);
                Save();

                App.Logger.WriteLine(LOG_IDENT, "First recorded launch, streak set to 1");
                return;
            }

            lastOpen = lastOpen.Date;

            if (lastOpen > today)
            {
                App.Logger.WriteLine(LOG_IDENT, "Stored date is in the future, leaving the streak untouched");
                return;
            }

            if (lastOpen == today)
            {
                if (Prop.Streak < 1)
                {
                    Prop.Streak = 1;
                    Save();
                }

                App.Logger.WriteLine(LOG_IDENT, "Already opened today, streak unchanged");
                return;
            }

            Prop.Streak = (lastOpen == today.AddDays(-1) && Prop.Streak > 0) ? Prop.Streak + 1 : 1;
            Prop.LastOpenDate = today.ToString(DateFormat, CultureInfo.InvariantCulture);
            Save();

            App.Logger.WriteLine(LOG_IDENT, $"Streak updated to {Prop.Streak}");
        }

        public string GetUsername()
        {
            string username = (Prop.Username ?? "").Trim();
            return username.Length == 0 ? DefaultUsername : username;
        }

        public void SetUsername(string? value)
        {
            string username = (value ?? "").Trim();

            if (username.Length > MaxUsernameLength)
                username = username[..MaxUsernameLength];

            if (username.Length == 0)
                username = DefaultUsername;

            if (Prop.Username == username)
                return;

            Prop.Username = username;
            Save();
        }

        public void SetVerified(bool value)
        {
            if (Prop.VerifiedBadge == value)
                return;

            Prop.VerifiedBadge = value;
            Save();
        }

        public void SetFont(string? id)
        {
            string fontId = ProfileFontRegistry.Get(id).Id;

            if (Prop.FontId == fontId)
                return;

            Prop.FontId = fontId;
            Save();
        }

        public string? AvatarPath
        {
            get
            {
                if (String.IsNullOrWhiteSpace(Prop.AvatarFileName))
                    return null;

                return Path.Combine(ProfileDirectory, Prop.AvatarFileName);
            }
        }

        public bool HasAvatar => AvatarPath is string path && File.Exists(path);

        public bool IsAvatarAnimated
        {
            get
            {
                string? path = AvatarPath;
                return path is not null && Path.GetExtension(path).Equals(".gif", StringComparison.OrdinalIgnoreCase);
            }
        }

        public Uri? AvatarUri => HasAvatar ? new Uri(AvatarPath!) : null;

        public bool SetAvatar(string sourcePath)
        {
            const string LOG_IDENT = "ProfileManager::SetAvatar";

            try
            {
                if (String.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
                {
                    App.Logger.WriteLine(LOG_IDENT, "Avatar source does not exist");
                    return false;
                }

                string extension = Path.GetExtension(sourcePath).ToLowerInvariant();

                if (!AllowedAvatarExtensions.Contains(extension))
                {
                    App.Logger.WriteLine(LOG_IDENT, $"Unsupported avatar extension '{extension}'");
                    return false;
                }

                if (!CanDecodeImage(sourcePath))
                {
                    App.Logger.WriteLine(LOG_IDENT, "Avatar could not be decoded");
                    return false;
                }

                Directory.CreateDirectory(ProfileDirectory);

                foreach (string existing in Directory.GetFiles(ProfileDirectory, "avatar*.*"))
                {
                    try { File.Delete(existing); }
                    catch (Exception ex) { App.Logger.WriteException(LOG_IDENT, ex); }
                }

                string fileName = $"avatar_{DateTime.UtcNow.Ticks}{extension}";
                File.Copy(sourcePath, Path.Combine(ProfileDirectory, fileName), true);

                Prop.AvatarFileName = fileName;
                Save();

                return true;
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, "Failed to set avatar");
                App.Logger.WriteException(LOG_IDENT, ex);
                return false;
            }
        }

        public BitmapImage? GetAvatarImage()
        {
            string? path = AvatarPath;

            if (path is null || !File.Exists(path))
                return null;

            try
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(path);
                image.EndInit();
                image.Freeze();
                return image;
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("ProfileManager::GetAvatarImage", "Failed to load the avatar image");
                App.Logger.WriteException("ProfileManager::GetAvatarImage", ex);
                return null;
            }
        }

        private static bool CanDecodeImage(string path)
        {
            try
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(path);
                image.EndInit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
