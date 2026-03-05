using InteractiveDisplayCapture.Helpers;
using InteractiveDisplayCapture.Services;
using InteractiveDisplayCapture.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace InteractiveDisplayCapture
{
    public class MainWindowViewModel : ViewModelBase
    {
        public EdgeHandleViewModel EdgeHandleVM { get; }

        public MainWindowViewModel(WindowPositionService positionService)
        {
           // EdgeHandleVM = new EdgeHandleViewModel(positionService);
        }
    }
}
