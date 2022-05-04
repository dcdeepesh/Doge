using IPCHandler;

using System.Windows.Controls;

namespace Doge {
    public partial class SpeakerPanel : UserControl {
        private SpeakerDto _speaker;
        internal SpeakerDto Speaker {
            get => _speaker;
            set {
                if (_speaker != value) {
                    _speaker = value;
                    DataContext = _speaker;
                }
            }
        }

        public SpeakerPanel(SpeakerDto speaker) {
            Speaker = speaker;
            DataContext = Speaker;
            InitializeComponent();
            Opacity = Preferences.Current.IdleOpacity / 100;
        }
    }
}
