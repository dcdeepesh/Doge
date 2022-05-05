using IPCHandler;

using System.Windows.Controls;
using System.Windows.Data;

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
            BindIdleOpacity();
            BindAlwaysVisibility();
        }

        public void BindIdleOpacity() {
            SetBinding(OpacityProperty,
                new Binding(nameof(Preferences.Current.IdleOpacity)) {
                    Source = Preferences.Current,
                    Mode = BindingMode.TwoWay
                });
        }

        public void BindSpeakingOpacity() {
            SetBinding(OpacityProperty,
                new Binding(nameof(Preferences.Current.SpeakingOpacity)) {
                    Source = Preferences.Current,
                    Mode = BindingMode.TwoWay
                });
        }

        public void BindAlwaysVisibility() {
            SetBinding(VisibilityProperty,
                new Binding(nameof(Preferences.Current.DisplayUsersAlways)) {
                    Source = Preferences.Current,
                    Mode = BindingMode.TwoWay,
                    Converter = new BooleanToVisibilityConverter()
                });
        }

        public void UnbindAlwaysVisibility() => BindingOperations.ClearBinding(this, VisibilityProperty);
    }
}
