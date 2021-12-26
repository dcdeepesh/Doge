using IPCHandler;

using System;
using System.Threading.Tasks;
using System.Windows;

namespace Doge {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Speakers.Init(UserPanels);
            Task.Run(async () => {
                await IPCHandler.IPCHandler.InitAsync();
                IPCHandler.IPCHandler.OnVoiceChannelJoin += OnChannelJoin;
                IPCHandler.IPCHandler.OnVoiceChannelLeave += OnChannelLeave;
                IPCHandler.IPCHandler.OnUserJoinOrUpdate += OnUserJoinOrUpdate;
                IPCHandler.IPCHandler.OnUserLeave += OnUserLeave;
                await IPCHandler.IPCHandler.StartEventsAsync();
            });
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
            if (speaker is null is false)
                Dispatcher.Invoke(() => Speakers.AddOrUpdate(speaker));
        }

        void OnUserLeave(object sender, string userId) {
            Dispatcher.Invoke(() => Speakers.Remove(userId));
        }
    }
}
