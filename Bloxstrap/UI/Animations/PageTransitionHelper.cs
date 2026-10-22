using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Bloxstrap.UI.Animations
{
    public static class PageTransitionHelper
    {
        public static void ApplyTransition(FrameworkElement element, double durationMs = 620, bool slideFromRight = true)
        {
            if (element == null) return;

            var duration = TimeSpan.FromMilliseconds(durationMs);

            element.Opacity = 0;
            element.RenderTransformOrigin = new Point(0.5, 0.5);

            double slideOffset = slideFromRight ? 20 : -20;
            var translate = new TranslateTransform { X = slideOffset, Y = 10 };
            var scale = new ScaleTransform(0.99, 0.99);

            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(translate);
            transformGroup.Children.Add(scale);
            element.RenderTransform = transformGroup;

            var ease = new ExponentialEase { EasingMode = EasingMode.EaseOut, Exponent = 4 };

            element.BeginAnimation(UIElement.OpacityProperty,
                new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(durationMs * 0.7)) { EasingFunction = ease });

            translate.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(slideOffset, 0, duration) { EasingFunction = ease });
            translate.BeginAnimation(TranslateTransform.YProperty, new DoubleAnimation(10, 0, duration) { EasingFunction = ease });

            scale.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(0.99, 1, duration) { EasingFunction = ease });
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(0.99, 1, duration) { EasingFunction = ease });
        }
    }
}
