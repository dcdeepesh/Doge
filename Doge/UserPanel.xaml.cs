using IPCHandler;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Doge {
    public partial class UserPanel : UserControl {
        public UserPanel() {
            InitializeComponent();
        }

        public ImageSource Avatar {
            get { return (ImageSource) GetValue(AvatarProperty); }
            set { SetValue(AvatarProperty, value); }
        }
        public static readonly DependencyProperty AvatarProperty =
            DependencyProperty.Register("Avatar", typeof(ImageSource), typeof(UserPanel), new PropertyMetadata(null));

        public Speaker Speaker {
            get { return (Speaker) GetValue(SpeakerProperty); }
            set { SetValue(SpeakerProperty, value); }
        }
        public static readonly DependencyProperty SpeakerProperty =
            DependencyProperty.Register("Speaker", typeof(Speaker), typeof(UserPanel), new PropertyMetadata(null));

        public string SpeakerName {
            get { return (string) GetValue(SpeakerNameProperty); }
            set { SetValue(SpeakerNameProperty, value); }
        }
        public static readonly DependencyProperty SpeakerNameProperty =
            DependencyProperty.Register("SpeakerName", typeof(string), typeof(UserPanel), new PropertyMetadata(null));
    }
}
