using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Bloxstrap.UI.Animations
{
    public static class PageTransitionHelper
    {
        public static void ApplyTransition(FrameworkElement element, double durationMs = 450, bool slideFromRight = true)
        {
            if (element == null) return;

            var duration = TimeSpan.FromMilliseconds(durationMs);

            element.Opacity = 0;
            element.RenderTransformOrigin = new Point(0.5, 0.5);

            // gentle upward drift + slight horizontal slide reads smoother than a big lateral jump
            double slideOffset = slideFromRight ? 24 : -24;
            var translate = new TranslateTransform { X = slideOffset, Y = 12 };
            var scale = new ScaleTransform(0.985, 0.985);

            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(translate);
            transformGroup.Children.Add(scale);
            element.RenderTransform = transformGroup;

            // exponential ease-out: fast start, silky landing
            var ease = new ExponentialEase { EasingMode = EasingMode.EaseOut, Exponent = 6 };

            // fade completes faster than movement so content is readable almost immediately
            element.BeginAnimation(UIElement.OpacityProperty,
                new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(durationMs * 0.6)) { EasingFunction = ease });

            translate.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(slideOffset, 0, duration) { EasingFunction = ease });
            translate.BeginAnimation(TranslateTransform.YProperty, new DoubleAnimation(12, 0, duration) { EasingFunction = ease });

            scale.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(0.985, 1, duration) { EasingFunction = ease });
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(0.985, 1, duration) { EasingFunction = ease });
        }
    }
}
