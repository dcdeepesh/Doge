using System;
using System.Windows;

namespace Doge {
    public partial class SettingsWindow : Window {
        public SettingsWindow() {
            InitializeComponent();
            DataContext = Preferences.Current;
            Loaded += (sender, args) => PositionWindow();
        }

        private void OnWindowClosed(object sender, EventArgs e) {
            Preferences.Save();
        }

        private void PositionWindow() {
            int margin = 0;
            var workArea = SystemParameters.WorkArea;
            if (workArea.Top != 0) {
                // taskbar is on the top
                Top = workArea.Top + margin;
                Left = workArea.Right - ActualWidth - margin;
            } else if (workArea.Left != 0) {
                // taskbar is on the left
                Top = workArea.Bottom - ActualHeight - margin;
                Left = workArea.Left + margin;
            } else {
                // tqaskbar is on the right or bottom
                Top = workArea.Bottom - ActualHeight - margin;
                Left = workArea.Right - ActualWidth - margin;
            }
        }

        private void OnClickConnect(object sender, RoutedEventArgs e) {
            AuthManager.AuthorizeAsync();
        }
    }
}
