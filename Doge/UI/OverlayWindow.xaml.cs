using IPCHandler;
using System.Windows;

namespace Doge {
    public partial class OverlayWindow : Window {
        public OverlayWindow() {
            DataContext = Preferences.Current;
            InitializeComponent();
        }
    }
}
