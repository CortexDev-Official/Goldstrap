using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Bloxstrap.UI.Animations
{
    public static class SmoothScrollBehavior
    {
        private static bool _initialized;

        public static void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            EventManager.RegisterClassHandler(
                typeof(ScrollViewer),
                ScrollViewer.PreviewMouseWheelEvent,
                new MouseWheelEventHandler(OnPreviewMouseWheel));
        }

        private static void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is not ScrollViewer sv || sv.ScrollableHeight <= 0)
                return;

            double delta = -e.Delta * 1.1;
            double target = Math.Max(0, Math.Min(sv.ScrollableHeight, sv.VerticalOffset + delta));

            if (!_states.TryGetValue(sv, out var state))
            {
                state = new ScrollState(sv);
                _states[sv] = state;
            }

            state.ScrollTo(target);
            e.Handled = true;
        }

        private static readonly Dictionary<ScrollViewer, ScrollState> _states = new();

        private class ScrollState
        {
            private readonly ScrollViewer _sv;
            private readonly IEasingFunction _ease;
            private bool _running;
            private DateTime _startTime;
            private double _from;
            private double _to;
            private const double DURATION_MS = 120;

            public ScrollState(ScrollViewer sv)
            {
                _sv = sv;
                _ease = new PowerEase { EasingMode = EasingMode.EaseOut, Power = 1.1 };
            }

            public void ScrollTo(double target)
            {
                if (!_running)
                {
                    _from = _sv.VerticalOffset;
                    _to = target;
                    double dist = Math.Abs(_to - _from);
                    if (dist < 1)
                    {
                        _sv.ScrollToVerticalOffset(_to);
                        return;
                    }
                    _startTime = DateTime.UtcNow;
                    _running = true;
                    CompositionTarget.Rendering += OnFrame;
                }
                else
                {
                    double elapsed = (DateTime.UtcNow - _startTime).TotalMilliseconds;
                    double progress = Math.Min(1.0, elapsed / DURATION_MS);
                    _from = _from + (_to - _from) * _ease.Ease(progress);
                    _to = target;
                    if (Math.Abs(_to - _from) < 1)
                    {
                        _sv.ScrollToVerticalOffset(_to);
                        Stop();
                        return;
                    }
                    _startTime = DateTime.UtcNow;
                }
            }

            private void OnFrame(object? sender, EventArgs e)
            {
                double elapsed = (DateTime.UtcNow - _startTime).TotalMilliseconds;
                if (elapsed >= DURATION_MS)
                {
                    _sv.ScrollToVerticalOffset(_to);
                    Stop();
                    return;
                }
                double progress = elapsed / DURATION_MS;
                double eased = _ease.Ease(progress);
                double current = _from + (_to - _from) * eased;
                _sv.ScrollToVerticalOffset(current);
            }

            private void Stop()
            {
                _running = false;
                CompositionTarget.Rendering -= OnFrame;
            }
        }
    }
}
