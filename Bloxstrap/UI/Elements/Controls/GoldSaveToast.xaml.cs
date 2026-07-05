using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Bloxstrap.UI.Elements.Controls
{
    /// <summary>
    /// Compact gold-accented toast shown when settings are saved.
    /// Quiet rise-and-fade entrance with a checkmark that draws in.
    /// </summary>
    public partial class GoldSaveToast : UserControl
    {
        private readonly DispatcherTimer _hideTimer = new();
        private bool _isShowing;

        public GoldSaveToast()
        {
            InitializeComponent();

            _hideTimer.Interval = TimeSpan.FromMilliseconds(2400);
            _hideTimer.Tick += (_, _) =>
            {
                _hideTimer.Stop();
                PlayExit();
            };
        }

        public void Show(string title, string message)
        {
            TitleText.Text = title;
            MessageText.Text = message;

            _hideTimer.Stop();
            _isShowing = true;
            Visibility = Visibility.Visible;

            PlayEntrance();
            _hideTimer.Start();
        }

        private void PlayEntrance()
        {
            var ease = new CubicEase { EasingMode = EasingMode.EaseOut };

            ToastRoot.BeginAnimation(OpacityProperty,
                new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(220)) { EasingFunction = ease });

            ToastTranslate.BeginAnimation(TranslateTransform.YProperty,
                new DoubleAnimation(16, 0, TimeSpan.FromMilliseconds(300)) { EasingFunction = ease });

            // checkmark draws in just after the card settles
            CheckMark.BeginAnimation(Shape.StrokeDashOffsetProperty,
                new DoubleAnimation(18, 0, TimeSpan.FromMilliseconds(320))
                {
                    BeginTime = TimeSpan.FromMilliseconds(150),
                    EasingFunction = ease
                });
        }

        private void PlayExit()
        {
            if (!_isShowing)
                return;

            _isShowing = false;

            var ease = new CubicEase { EasingMode = EasingMode.EaseIn };

            var fade = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(270)) { EasingFunction = ease };
            fade.Completed += (_, _) =>
            {
                if (!_isShowing)
                    Visibility = Visibility.Collapsed;
            };

            ToastRoot.BeginAnimation(OpacityProperty, fade);

            ToastTranslate.BeginAnimation(TranslateTransform.YProperty,
                new DoubleAnimation(0, 10, TimeSpan.FromMilliseconds(270)) { EasingFunction = ease });
        }
    }
}
