using System.Windows;

namespace Doge {
    public partial class OverlayWindow : Window {
        public OverlayWindow() {
            InitializeComponent();
            DataContext = Preferences.Current;
        }
    }
}
