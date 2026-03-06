using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
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
    }
}
