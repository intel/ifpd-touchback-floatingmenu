using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Point = System.Windows.Point;

namespace InteractiveDisplayCapture.Services
{
    public class WindowPositionService
    {
        private Window _window;
        private Point _startPoint;

        public WindowPositionService(Window window)
        {
            _window = window;
        }

        public void DockToRightEdge()
        {
            var area = SystemParameters.WorkArea;
            _window.Left = area.Right - _window.Width;
            _window.Top = area.Height / 2 - _window.Height / 2;
        }

        public void StartDrag()
        {
            _startPoint = Mouse.GetPosition(_window);
        }

        public void Drag()
        {
            var p = Mouse.GetPosition(null);
            _window.Top = p.Y - _startPoint.Y;
        }
    }
}
