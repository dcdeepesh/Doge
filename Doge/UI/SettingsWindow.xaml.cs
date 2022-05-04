using System;
using System.Windows;

namespace Doge {
    public partial class SettingsWindow : Window {
        public SettingsWindow() {
            DataContext = Preferences.Current;
            InitializeComponent();
        }

        private void OnWindowClosed(object sender, EventArgs e) {
            Preferences.Save();
        }
    }
}
