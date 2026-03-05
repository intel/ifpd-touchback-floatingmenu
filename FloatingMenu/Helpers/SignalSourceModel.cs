using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace InteractiveDisplayCapture.Helpers
{
    public class SignalSourceModel : INotifyPropertyChanged
    {
        private DeviceStatusEnum _status;
        public string Name { get; set; }
        public DeviceStatusEnum Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
