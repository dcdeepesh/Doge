using IPCHandler;

using System;
using System.Linq;
using System.Windows;

namespace Doge {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Speakers.Init(UserPanels);

            IPCHandler.IPCHandler.OnVoiceChannelJoin += OnChannelJoin;
            IPCHandler.IPCHandler.OnVoiceChannelLeave += OnChannelLeave;
            IPCHandler.IPCHandler.OnUserJoinOrUpdate += OnUserJoinOrUpdate;
            IPCHandler.IPCHandler.OnUserLeave += OnUserLeave;
            IPCHandler.IPCHandler.OnSpeakingStart += OnSpeakingStart;
            IPCHandler.IPCHandler.OnSpeakingStop += OnSpeakingStop;

            IPCHandler.IPCHandler.InitAndStartEvents();
        }

        protected override void OnClosed(EventArgs e) {
            UserPanels.Children.Clear();
            IPCHandler.IPCHandler.OnVoiceChannelJoin -= OnChannelJoin;
            IPCHandler.IPCHandler.OnVoiceChannelLeave -= OnChannelLeave;
            IPCHandler.IPCHandler.OnUserJoinOrUpdate -= OnUserJoinOrUpdate;
            IPCHandler.IPCHandler.OnUserLeave -= OnUserLeave;
            base.OnClosed(e);
        }

        void OnChannelJoin(object sender, string channelId) {
            Dispatcher.Invoke(() => Show());
        }

        void OnChannelLeave(object sender, EventArgs args) {
            Dispatcher.Invoke(() => {
                UserPanels.Children.Clear();
                Hide();
            });
        }

        void OnUserJoinOrUpdate(object sender, Speaker speaker) {
            if (speaker is not null)
                Dispatcher.Invoke(() => Speakers.AddOrUpdate(speaker));
        }

        void OnUserLeave(object sender, string userId) {
            Dispatcher.Invoke(() => Speakers.Remove(userId));
        }

        void OnSpeakingStart(object sender, string userId) {
            Dispatcher.Invoke(() => {
                foreach (var panel in UserPanels.Children.OfType<UserPanel>())
                    if (panel.Speaker.Id == userId)
                        panel.Opacity = 1;
            });
        }

        void OnSpeakingStop(object sender, string userId) {
            Dispatcher.Invoke(() => {
                foreach (var panel in UserPanels.Children.OfType<UserPanel>())
                    if (panel.Speaker.Id == userId)
                        panel.Opacity = 0.6;
            });
        }
    }
}
