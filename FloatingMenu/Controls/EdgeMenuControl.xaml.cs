using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UserControl = System.Windows.Controls.UserControl;

namespace InteractiveDisplayCapture.Controls
{
    /// <summary>
    /// Interaction logic for EdgeMenuControl.xaml
    /// </summary>
    public partial class EdgeMenuControl : UserControl
    {
        private bool _suppressSelectionEvent;

        public event Action<int> MenuItemSelected;
        public EdgeMenuControl()
        {
            InitializeComponent();
        }

        private void NavList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppressSelectionEvent)
                return;

            if (NavList.SelectedIndex < 0)
                return;

            // Fire event to MainWindow
            MenuItemSelected?.Invoke(NavList.SelectedIndex);
        }

        public void ClearSelection()
        {
            NavList.SelectedItem = null;
        }

        public void SelectMenuItem(int index)
        {
            _suppressSelectionEvent = true;

            NavList.SelectedIndex = index;

            _suppressSelectionEvent = false;
        }

        public void OpenMenu()
        {
            this.Visibility = Visibility.Visible;

            DoubleAnimation slide = new DoubleAnimation
            {
                From = 260,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new CubicEase
                {
                    EasingMode = EasingMode.EaseOut
                }
            };

           
        }

        public void CloseMenu()
        {
            DoubleAnimation slide = new DoubleAnimation
            {
                From = 0,
                To = 260,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new CubicEase
                {
                    EasingMode = EasingMode.EaseIn
                }
            };

            slide.Completed += (s, e) =>
            {
                this.Visibility = Visibility.Collapsed;
            };

           
        }
    }
}
