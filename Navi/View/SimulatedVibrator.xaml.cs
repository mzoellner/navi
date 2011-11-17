using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Navi.View
{
    /// <summary>
    /// Interaction logic for SimulatedVibrator.xaml
    /// </summary>
    public partial class SimulatedVibrator : UserControl
    {
        private static DispatcherTimer _synchronizedTimer = new DispatcherTimer();

        private static List<SimulatedVibrator> _registeredVibrators = new List<SimulatedVibrator>();

        private static object _listLocker = new object();

        private static bool _toggle = false;

        static SimulatedVibrator()
        {
            _synchronizedTimer.Interval = TimeSpan.FromMilliseconds(500);
            _synchronizedTimer.Tick += new EventHandler(OnTimerTick);
            _synchronizedTimer.Start();
        }

        static void OnTimerTick(object sender, EventArgs e)
        {
            _toggle = !_toggle;
            lock (_listLocker)
            {
                foreach (SimulatedVibrator vib in _registeredVibrators)
                {
                    vib.UpdateImage(_toggle);
                }
            }
        }

        private static void RegisterVibratorWithTimer(SimulatedVibrator vib)
        {
            lock (_listLocker)
            {
                if (!_registeredVibrators.Contains(vib))
                    _registeredVibrators.Add(vib);
            }
        }

        private static void UnregisterVibratorWithTimer(SimulatedVibrator vib)
        {
            lock (_listLocker)
            {
                if (_registeredVibrators.Contains(vib))
                    _registeredVibrators.Remove(vib);
            }
        }

        internal void UpdateImage(bool show)
        {
            if (show)
            {
                img2.Visibility = System.Windows.Visibility.Visible;
                //img3.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                img2.Visibility = System.Windows.Visibility.Collapsed;
                //img3.Visibility = System.Windows.Visibility.Visible;
            }
        }

        public SimulatedVibrator()
        {
            InitializeComponent();
        }

        #region Mode

        /// <summary>
        /// Mode Dependency Property
        /// </summary>
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(int), typeof(SimulatedVibrator),
                new FrameworkPropertyMetadata((int)0,
                    new PropertyChangedCallback(OnModeChanged)));

        /// <summary>
        /// Gets or sets the Mode property. This dependency property 
        /// indicates ....
        /// </summary>
        public int Mode
        {
            get { return (int)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Mode property.
        /// </summary>
        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SimulatedVibrator target = (SimulatedVibrator)d;
            int oldMode = (int)e.OldValue;
            int newMode = target.Mode;
            target.OnModeChanged(oldMode, newMode);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Mode property.
        /// </summary>
        protected virtual void OnModeChanged(int oldMode, int newMode)
        {
            switch(oldMode)
            {
                case 0:
                    img1.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case 1:
                    UnregisterVibratorWithTimer(this);
                    img2.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case 2:
                    break;
                case 3:
                    img1.Visibility = System.Windows.Visibility.Collapsed;
                    break;
            }

            switch (newMode)
            {
                case 0:
                    img1.Visibility = System.Windows.Visibility.Visible;
                    break;
                case 1:
                    RegisterVibratorWithTimer(this);
                    break;
                case 2:
                    break;
                case 3:
                    img1.Visibility = System.Windows.Visibility.Visible;
                    break;
            }


        }

        #endregion

    }
}
