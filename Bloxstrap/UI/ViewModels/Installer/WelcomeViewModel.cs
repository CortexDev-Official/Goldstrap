namespace Bloxstrap.UI.ViewModels.Installer
{
    public class WelcomeViewModel : NotifyPropertyChangedViewModel
    {
        
        public string MainText => String.Format(
            Strings.Installer_Welcome_MainText,
            "[github.com/CortexDev-Official/Goldstrap](https://github.com/CortexDev-Official/Goldstrap)"
        );

        public bool CanContinue { get; set; } = false;
    }
}
