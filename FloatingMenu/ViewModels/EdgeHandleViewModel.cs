using InteractiveDisplayCapture.Commands;
using InteractiveDisplayCapture.Helpers;
using InteractiveDisplayCapture.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace InteractiveDisplayCapture.ViewModels
{
    public class EdgeHandleViewModel : ViewModelBase
    {
        private WindowPositionService _positionService;

        // private readonly WindowPositionService _positionService;
        // private readonly FloatingToolbarViewModel _toolbarVm;

        public ICommand StartDragCommand { get; }
        public ICommand DragCommand { get; }
        public ICommand EndDragCommand { get; }

        public EdgeHandleViewModel(
            WindowPositionService positionService
            )
        {
            _positionService = positionService;
           // _toolbarVm = toolbarVm;

            StartDragCommand = new RelayCommand(_ => _positionService.StartDrag());
            DragCommand = new RelayCommand(_ => _positionService.Drag());
            EndDragCommand = new RelayCommand(_ => ToggleToolbar());
        }

        private void ToggleToolbar()
        {
            //_toolbarVm.Toggle();
        }
    }
}
