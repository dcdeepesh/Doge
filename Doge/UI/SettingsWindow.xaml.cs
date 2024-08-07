﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace Doge {
    public partial class SettingsWindow : Window {
        public SettingsWindow() {
            Preferences.Current.WindowLeftMax = (int) SystemParameters.WorkArea.Right;
            Preferences.Current.WindowTopMax = (int) SystemParameters.WorkArea.Bottom;

            InitializeComponent();
            DataContext = Preferences.Current;
            SizeChanged += (sender, args) => PositionWindow();
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

        private void OnWindowClosed(object sender, EventArgs e) {
            Preferences.Save();
        }

        private async void OnClickConnect(object sender, RoutedEventArgs e) {
            ConnectButton.IsEnabled = false;
            var authSuccessful = await AuthManager.AuthorizeAsync();
            if (authSuccessful)
                IPCEventHandler.Init();
            ConnectButton.IsEnabled = true;
        }

        private void OnClickHyperlink(object sender, RequestNavigateEventArgs e) {
            Process.Start(e.Uri.ToString());
        }
    }
}
