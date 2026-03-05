using InteractiveDisplayCapture.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Windows.Point;
using UserControl = System.Windows.Controls.UserControl;

namespace InteractiveDisplayCapture.Controls
{
    /// <summary>
    /// Interaction logic for EdgeHandleControl.xaml
    /// </summary>
    public partial class EdgeHandleControl : UserControl
    {
        private bool _isDragging;
        private Point _startPoint;


        private Point _startMousePosition;
        private double _startTop;
        // Declare event
        public event Action EdgeButtonClicked;

        public EdgeHandleControl()
        {
            InitializeComponent();
         
        }

        private void EdgeButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null)
            {
                window.DragMove();
            }

            if (!_isDragging)
            {
                // THIS replaces Click event
                EdgeButtonClicked?.Invoke();
            }
        }

        private void EdgeButton_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging) return;

            var parentWindow = Window.GetWindow(this);
            if (parentWindow == null) return;

            Point currentPosition = e.GetPosition(null);
            double deltaY = currentPosition.Y - _startMousePosition.Y;

            double newTop = _startTop + deltaY;

            double screenHeight = SystemParameters.PrimaryScreenHeight;
            newTop = Math.Max(0, Math.Min(screenHeight - parentWindow.Height, newTop));

            parentWindow.Left = 0;      // ?? Always stay at left edge
            parentWindow.Top = newTop;  // ? Move only vertically
        }

        private void EdgeButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            Mouse.Capture(null);

         
        }

    
        //private void OnMouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    var window = Window.GetWindow(this);
        //    if (window == null) return;

        //    _isDragging = true;
        //    _startPoint = e.GetPosition(window);
        //    Mouse.Capture((UIElement)sender);
        //}

        //private void OnMouseMove(object sender, MouseEventArgs e)
        //{
        //    if (!_isDragging) return;

        //    var window = Window.GetWindow(this);
        //    if (window == null) return;

        //    Point currentPoint = e.GetPosition(window);
        //    double deltaY = currentPoint.Y - _startPoint.Y;

        //    Rect workArea = SystemParameters.WorkArea;

        //    double newTop = window.Top + deltaY;
        //    newTop = Math.Max(workArea.Top,
        //             Math.Min(newTop, workArea.Bottom - window.Height));

        //    window.Top = newTop;
        //    window.Left = workArea.Left; // lock to left edge

        //    _startPoint = currentPoint; // 🔑 critical
        //}

        //private void OnMouseUp(object sender, MouseButtonEventArgs e)
        //{
        //    _isDragging = false;
        //    Mouse.Capture(null);
        //}


        //private void EdgeButton_Click(object sender, RoutedEventArgs e)
        //{
        //    EdgeButtonClicked?.Invoke();
        //}



        //private void EdgeButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (e.ButtonState == MouseButtonState.Pressed)
        //    {
        //        this.DragMove();
        //    }
        //}
    }
}
