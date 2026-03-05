using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace InteractiveDisplayCapture.Helpers
{
    public class LockButtonEdges
    {
        public void LockToRightEdge(Window window)
        {
            var screen = System.Windows.Forms.Screen.FromHandle(
                new System.Windows.Interop.WindowInteropHelper(window).Handle);

            window.Left = screen.WorkingArea.Right - window.Width;
        }
    }
}
