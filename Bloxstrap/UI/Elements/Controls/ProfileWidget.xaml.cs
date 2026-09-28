using System.Windows;
using System.Windows.Controls;

using Bloxstrap.UI.ViewModels;

namespace Bloxstrap.UI.Elements.Controls
{
    public partial class ProfileWidget : UserControl
    {
        private static readonly TimeSpan DismissGuard = TimeSpan.FromMilliseconds(250);

        private DateTime _lastClosed = DateTime.MinValue;

        public ProfileWidget()
        {
            InitializeComponent();

            DataContext = new ProfileViewModel();
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (DateTime.UtcNow - _lastClosed < DismissGuard)
                return;

            ProfilePopup.IsOpen = true;
        }

        private void ProfilePopup_Opened(object sender, EventArgs e)
        {
            if (ProfilePopup.Child is ProfilePopup popup)
                popup.PlayOpenAnimation();
        }

        private void ProfilePopup_Closed(object sender, EventArgs e)
        {
            _lastClosed = DateTime.UtcNow;
        }
    }
}
