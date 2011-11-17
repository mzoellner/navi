using System;
using System.Windows.Interactivity;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Windows.Media;

namespace Navi.View
{
    public class ShakeBehavior : Behavior<FrameworkElement>
    {
        #region private fields

        private DoubleAnimationUsingKeyFrames _widthAnimation;
        private DoubleAnimationUsingKeyFrames _heightAnimation;
        private DispatcherTimer _pulseTimer;
        private Storyboard _shakeStoryboard;

        #endregion

        #region ctor

        public ShakeBehavior()
        {
            _pulseTimer = new DispatcherTimer();
            _widthAnimation = new DoubleAnimationUsingKeyFrames();
            _heightAnimation = new DoubleAnimationUsingKeyFrames();
            _shakeStoryboard = new Storyboard();
        }

        #endregion

        #region Pulse

        /// <summary>
        /// Pulse Dependency Property
        /// </summary>
        public static readonly DependencyProperty PulseProperty =
            DependencyProperty.Register("Pulse", typeof(double), typeof(ShakeBehavior),
                new FrameworkPropertyMetadata((double)0.0,
                    new PropertyChangedCallback(OnPulseChanged)));

        /// <summary>
        /// Gets or sets the Pulse property. This dependency property 
        /// indicates ....
        /// </summary>
        public double Pulse
        {
            get { return (double)GetValue(PulseProperty); }
            set { SetValue(PulseProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Pulse property.
        /// </summary>
        private static void OnPulseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShakeBehavior target = (ShakeBehavior)d;
            double oldPulse = (double)e.OldValue;
            double newPulse = target.Pulse;
            target.OnPulseChanged(oldPulse, newPulse);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Pulse property.
        /// </summary>
        protected virtual void OnPulseChanged(double oldPulse, double newPulse)
        {
            if (_pulseTimer.IsEnabled)
            {
                _pulseTimer.Stop();
                _pulseTimer.Interval = TimeSpan.FromMilliseconds(newPulse);
                _pulseTimer.Start();
            }
            else
            {
                _pulseTimer.Interval = TimeSpan.FromMilliseconds(newPulse);
            }
            
        }

        #endregion

        #region IsEnabled

        /// <summary>
        /// IsEnabled Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(ShakeBehavior),
                new FrameworkPropertyMetadata((bool)false,
                    new PropertyChangedCallback(OnIsEnabledChanged)));

        /// <summary>
        /// Gets or sets the IsEnabled property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        /// <summary>
        /// Handles changes to the IsEnabled property.
        /// </summary>
        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShakeBehavior target = (ShakeBehavior)d;
            bool oldIsEnabled = (bool)e.OldValue;
            bool newIsEnabled = target.IsEnabled;
            target.OnIsEnabledChanged(oldIsEnabled, newIsEnabled);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the IsEnabled property.
        /// </summary>
        protected virtual void OnIsEnabledChanged(bool oldIsEnabled, bool newIsEnabled)
        {
            if (newIsEnabled)
            {
                StartShaking();
                StartTimer();
            }
            else
            {
                StopTimer();
                StopShaking();
            }
        }

        #endregion

        protected override void OnAttached()
        {
            _pulseTimer.Tick += new EventHandler(OnPulseTimerTick);

            CheckScaleStransform();

            _widthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1.1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(100))));
            _widthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(200))));

            _heightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1.1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(100))));
            _heightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(200))));
            
            Storyboard.SetTarget(_widthAnimation, AssociatedObject);
            Storyboard.SetTarget(_heightAnimation, AssociatedObject);
            
            Storyboard.SetTargetProperty(_widthAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));
            Storyboard.SetTargetProperty(_heightAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));
            
            _shakeStoryboard.Children.Add(_widthAnimation);
            _shakeStoryboard.Children.Add(_heightAnimation);
            _shakeStoryboard.RepeatBehavior = RepeatBehavior.Forever;
            _shakeStoryboard.SpeedRatio = 5.0;
            _shakeStoryboard.AutoReverse = true;
            _shakeStoryboard.FillBehavior = FillBehavior.Stop;


            if (IsEnabled)
            {
                StartShaking();
                StartTimer();
            }
                
        }

        private void CheckScaleStransform()
        {
            if (AssociatedObject.RenderTransform == null || AssociatedObject.RenderTransform == MatrixTransform.Identity)
            {
                AssociatedObject.RenderTransform = new ScaleTransform(1.0,1.0);
            }

            AssociatedObject.RenderTransformOrigin = new Point(0.5, 0.5);
            
        }

        void OnPulseTimerTick(object sender, EventArgs e)
        {
            if ((sender as DispatcherTimer).Interval.Milliseconds > 0.0)
            {
                if (_shakeStoryboard.GetIsPaused())
                    _shakeStoryboard.Resume();
                else
                    _shakeStoryboard.Pause();
            }
        }

        private void StartTimer()
        {
            if (_pulseTimer.Interval.Milliseconds > 0.0)
                _pulseTimer.Start();
        }

        private void StopTimer()
        {
            _pulseTimer.Stop();
        }

        private void StartShaking()
        {
            if(AssociatedObject != null)
                _shakeStoryboard.Begin();
        }

        private void StopShaking()
        {
            if (AssociatedObject != null)
                _shakeStoryboard.Stop();
        }

    }
}
