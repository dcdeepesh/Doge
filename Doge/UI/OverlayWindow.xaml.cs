using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Doge {
    public partial class OverlayWindow : Window {
        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr window, int index);
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr window, int index, int value);

        public OverlayWindow() {
            InitializeComponent();
            DataContext = Preferences.Current;
            Loaded += (_, _) => {
                const int GWL_EXSTYLE = -20;
                const int WS_EX_TOOLWINDOW = 0x00000080;

                var handle = new WindowInteropHelper(this).Handle;
                int currentStyle = GetWindowLong(handle, GWL_EXSTYLE);
                SetWindowLong(handle, GWL_EXSTYLE, currentStyle | WS_EX_TOOLWINDOW);
            };
        }
    }
}
