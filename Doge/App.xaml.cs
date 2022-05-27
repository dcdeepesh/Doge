using System;
using System.Windows;

using MenuItem = System.Windows.Forms.MenuItem;
using MouseButtons = System.Windows.Forms.MouseButtons;
using NotifyIcon = System.Windows.Forms.NotifyIcon;

namespace Doge {
    public partial class App : Application {
        private NotifyIcon trayIcon;
        private SettingsWindow activeSettingsWindow;

        private void OnStart(object sender, StartupEventArgs e) {
            Preferences.Load();
            AuthManager.PrepareAccessTokenAsync()
                .ContinueWith(delegate {
                    if (!Preferences.Current.AuthPending)
                        IPCEventHandler.Init();
                });
            InitTrayIcon();

            // Open the settings window on startup
            Dispatcher.BeginInvoke((Action) (() => {
                activeSettingsWindow = new SettingsWindow();
                activeSettingsWindow.Closed += (sender, args) => activeSettingsWindow = null;
                activeSettingsWindow.Show();
            }));
        }

        private void OnExit(object sender, ExitEventArgs e) {
            trayIcon.Dispose();
        }

        private void InitTrayIcon() {
            trayIcon = new NotifyIcon {
                Text = "Doge",
                Icon = new(GetResourceStream(new Uri("Resources\\doge-icon.ico", UriKind.Relative)).Stream),
                Visible = true,
                ContextMenu = new(new MenuItem[] {
                    new("Exit", (sender, args) => Current.Shutdown())
                })
            };

            trayIcon.MouseClick += (sender, args) => {
                if (args.Button != MouseButtons.Left)
                    return;

                Dispatcher.BeginInvoke(new Action(() => {
                    if (activeSettingsWindow is null) {
                        activeSettingsWindow = new SettingsWindow();
                        activeSettingsWindow.Closed += (sender, args) => activeSettingsWindow = null;
                    }
                    activeSettingsWindow.Show();
                    activeSettingsWindow.Activate();
                }));
            };
        }
    }
}
