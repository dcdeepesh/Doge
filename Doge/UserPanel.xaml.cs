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
            DependencyProperty.Register("Avatar", typeof(ImageSource), typeof(UserPanel), new PropertyMetadata(default(ImageSource)));

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



        public bool Mute {
            get { return (bool) GetValue(MuteProperty); }
            set {
                SetValue(MuteProperty, value);
                MuteIcon.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public static readonly DependencyProperty MuteProperty =
            DependencyProperty.Register("Mute", typeof(bool), typeof(UserPanel), new PropertyMetadata(false));

        public bool Deaf {
            get { return (bool) GetValue(DeafProperty); }
            set {
                SetValue(DeafProperty, value);
                DeafIcon.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public static readonly DependencyProperty DeafProperty =
            DependencyProperty.Register("Deaf", typeof(bool), typeof(UserPanel), new PropertyMetadata(false));
    }
}
