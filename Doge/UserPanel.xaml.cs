using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Doge {
    /// <summary>
    /// Interaction logic for UserPanel.xaml
    /// </summary>
    public partial class UserPanel : UserControl {
        public UserPanel() {
            InitializeComponent();
        }


        public ImageSource Avatar {
            get { return (ImageSource) GetValue(AvatarProperty); }
            set { SetValue(AvatarProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Avatar. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AvatarProperty =
            DependencyProperty.Register("Avatar", typeof(ImageSource), typeof(UserPanel), new PropertyMetadata(null));



        public string Username {
            get { return (string) GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Username. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UsernameProperty =
            DependencyProperty.Register("Username", typeof(string), typeof(UserPanel), new PropertyMetadata(null));
    }
}
