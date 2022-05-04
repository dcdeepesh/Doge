using System.Windows;

namespace Doge {
    public partial class App : Application {
        public App() {
            Preferences.Load();
            IPCEventHandler.Init();
        }
    }
}
