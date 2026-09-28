namespace Bloxstrap.Models.Persistable
{
    public class ProfileSettings
    {
        public string Username { get; set; } = "User";

        public string AvatarFileName { get; set; } = "";

        public bool VerifiedBadge { get; set; } = false;

        public string FontId { get; set; } = "default";

        public int Streak { get; set; } = 0;

        public string LastOpenDate { get; set; } = "";
    }
}
