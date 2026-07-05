using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Bloxstrap.UI.Animations
{
    public static class ButtonPressBehavior
    {
        public static readonly DependencyProperty PressEnabledProperty =
            DependencyProperty.RegisterAttached("PressEnabled", typeof(bool), typeof(ButtonPressBehavior),
                new PropertyMetadata(false, OnPressEnabledChanged));

        public static bool GetPressEnabled(DependencyObject obj) => (bool)obj.GetValue(PressEnabledProperty);
        public static void SetPressEnabled(DependencyObject obj, bool value) => obj.SetValue(PressEnabledProperty, value);

        private static void OnPressEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not System.Windows.Controls.Primitives.ButtonBase element) return;

            if ((bool)e.NewValue)
            {
                element.PreviewMouseLeftButtonDown += OnPress;
                element.PreviewMouseLeftButtonUp += OnRelease;
                element.MouseLeave += OnRelease;
            }
            else
            {
                element.PreviewMouseLeftButtonDown -= OnPress;
                element.PreviewMouseLeftButtonUp -= OnRelease;
                element.MouseLeave -= OnRelease;
            }
        }

        private static void OnPress(object sender, MouseButtonEventArgs e)
        {
            if (sender is not FrameworkElement element) return;

            element.RenderTransformOrigin = new Point(0.5, 0.5);
            var transform = element.RenderTransform as ScaleTransform ?? new ScaleTransform(1, 1);
            element.RenderTransform = transform;

            var anim = new DoubleAnimation(0.96, TimeSpan.FromMilliseconds(110))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            transform.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
            transform.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
        }

        private static void OnRelease(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement element) return;
            if (element.RenderTransform is not ScaleTransform transform) return;

            // springy release - slight overshoot makes the button feel alive
            var anim = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(330))
            {
                EasingFunction = new ElasticEase { EasingMode = EasingMode.EaseOut, Oscillations = 1, Springiness = 6 }
            };
            transform.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
            transform.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
        }
    }
}
