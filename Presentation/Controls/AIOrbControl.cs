using System.Windows;
using System.Windows.Media.Animation;

namespace NexusAI.Presentation.Controls;

/// <summary>
/// AI Orb - Premium thinking indicator with pulsing animation
/// </summary>
public class AIOrbControl : FrameworkElement
{
    private readonly Storyboard _pulseAnimation;

    public AIOrbControl()
    {
        // Create pulsing animation
        _pulseAnimation = new Storyboard { RepeatBehavior = RepeatBehavior.Forever };

        var scaleX = new DoubleAnimation
        {
            From = 1.0,
            To = 1.2,
            Duration = TimeSpan.FromSeconds(1.5),
            AutoReverse = true,
            EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
        };

        var scaleY = new DoubleAnimation
        {
            From = 1.0,
            To = 1.2,
            Duration = TimeSpan.FromSeconds(1.5),
            AutoReverse = true,
            EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
        };

        var opacity = new DoubleAnimation
        {
            From = 0.6,
            To = 1.0,
            Duration = TimeSpan.FromSeconds(1.5),
            AutoReverse = true,
            EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
        };

        Storyboard.SetTargetProperty(scaleX, new PropertyPath("RenderTransform.ScaleX"));
        Storyboard.SetTargetProperty(scaleY, new PropertyPath("RenderTransform.ScaleY"));
        Storyboard.SetTargetProperty(opacity, new PropertyPath("Opacity"));

        _pulseAnimation.Children.Add(scaleX);
        _pulseAnimation.Children.Add(scaleY);
        _pulseAnimation.Children.Add(opacity);

        Loaded += (s, e) => _pulseAnimation.Begin(this);
        Unloaded += (s, e) => _pulseAnimation.Stop(this);
    }
}
