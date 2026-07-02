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

            double slideOffset = slideFromRight ? 50 : -50;
            var translate = new TranslateTransform { X = slideOffset };
            var scale = new ScaleTransform(0.92, 0.92);

            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(translate);
            transformGroup.Children.Add(scale);
            element.RenderTransform = transformGroup;

            var ease = new CircleEase { EasingMode = EasingMode.EaseOut };

            element.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(0, 1, duration) { EasingFunction = ease });

            translate.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(slideOffset, 0, duration) { EasingFunction = ease });

            scale.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(0.92, 1, duration) { EasingFunction = ease });

            scale.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(0.92, 1, duration) { EasingFunction = ease });
        }
    }
}
