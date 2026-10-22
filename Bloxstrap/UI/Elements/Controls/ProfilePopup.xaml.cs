using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

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

        public void CaptureBackdrop()
        {
            const double renderScale = 0.125;

            try
            {
                Window? host = Window.GetWindow(this);

                if (host is null || host.ActualWidth <= 0 || host.ActualHeight <= 0 || PopupRoot.ActualWidth <= 0)
                    return;

                var dpi = VisualTreeHelper.GetDpi(this);

                var bitmap = new RenderTargetBitmap(
                    (int)Math.Ceiling(host.ActualWidth * renderScale),
                    (int)Math.Ceiling(host.ActualHeight * renderScale),
                    96 * renderScale,
                    96 * renderScale,
                    PixelFormats.Pbgra32);

                bitmap.Render(host);
                bitmap.Freeze();

                Point popupOrigin = PopupRoot.PointToScreen(new Point(0, 0));
                Point hostOrigin = host.PointToScreen(new Point(0, 0));

                double offsetX = (popupOrigin.X - hostOrigin.X) / dpi.DpiScaleX;
                double offsetY = (popupOrigin.Y - hostOrigin.Y) / dpi.DpiScaleY;

                BackdropBrush.ViewportUnits = BrushMappingMode.Absolute;
                BackdropBrush.ViewboxUnits = BrushMappingMode.Absolute;
                BackdropBrush.Viewport = new Rect(0, 0, PopupRoot.ActualWidth, PopupRoot.ActualHeight);
                BackdropBrush.Viewbox = new Rect(offsetX, offsetY, PopupRoot.ActualWidth, PopupRoot.ActualHeight);
                BackdropBrush.ImageSource = bitmap;
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("ProfilePopup::CaptureBackdrop", "Failed to build the popup backdrop");
                App.Logger.WriteException("ProfilePopup::CaptureBackdrop", ex);
            }
        }

        public void PlayOpenAnimation()
        {
            var ease = new QuarticEase { EasingMode = EasingMode.EaseOut };

            PopupRoot.BeginAnimation(OpacityProperty,
                new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(420)) { EasingFunction = ease });

            if (PopupRoot.RenderTransform is TranslateTransform transform)
                transform.BeginAnimation(TranslateTransform.YProperty,
                    new DoubleAnimation(16, 0, TimeSpan.FromMilliseconds(420)) { EasingFunction = ease });
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
