using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Bloxstrap.UI.ViewModels;

namespace Bloxstrap.UI.Elements.Controls
{
    public partial class ProfilePopup : UserControl
    {
        public ProfilePopup()
        {
            InitializeComponent();

            PopupRoot.RenderTransform = new TranslateTransform(0, 12);
        }

        public void PlayOpenAnimation()
        {
            var ease = new CubicEase { EasingMode = EasingMode.EaseOut };

            PopupRoot.BeginAnimation(OpacityProperty,
                new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300)) { EasingFunction = ease });

            if (PopupRoot.RenderTransform is TranslateTransform transform)
                transform.BeginAnimation(TranslateTransform.YProperty,
                    new DoubleAnimation(12, 0, TimeSpan.FromMilliseconds(300)) { EasingFunction = ease });
        }

        private void UsernameBox_LostFocus(object sender, RoutedEventArgs e) => CommitUsername();

        private void UsernameBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            CommitUsername();
            e.Handled = true;
        }

        private void CommitUsername()
        {
            if (DataContext is ProfileViewModel viewModel)
                viewModel.CommitUsername();
        }
    }
}
