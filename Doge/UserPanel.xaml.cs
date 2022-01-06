using IPCHandler;

using System.Windows;
using System.Windows.Controls;

namespace Doge {
    public partial class UserPanel : UserControl {
        public UserPanel() {
            InitializeComponent();
        }

        public Speaker Speaker {
            get { return (Speaker) GetValue(SpeakerProperty); }
            set { SetValue(SpeakerProperty, value); }
        }
        public static readonly DependencyProperty SpeakerProperty =
            DependencyProperty.Register("Speaker", typeof(Speaker), typeof(UserPanel), new PropertyMetadata(null));
    }
}
