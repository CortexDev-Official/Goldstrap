namespace Bloxstrap.Extensions
{
    static class RobloxThemeEx
    {
        public static IReadOnlyCollection<RobloxTheme> Selections => new RobloxTheme[]
        {
            RobloxTheme.Default,
            RobloxTheme.Light,
            RobloxTheme.Dark
        };
    }
}
