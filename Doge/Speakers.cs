using IPCHandler;

using System.Linq;
using System.Windows.Controls;

namespace Doge {
    internal class Speakers {
        static StackPanel panels;

        public static void Init(StackPanel userPanels) {
            panels = userPanels;
        }

        public static void AddOrUpdate(Speaker speaker) {
            bool found = false;
            foreach (var panel in panels.Children.OfType<UserPanel>()) {
                if (panel.Speaker.Id == speaker.Id) {
                    found = true;
                    panel.Speaker = speaker;
                }
            }

            if (!found) {
                panels.Children.Add(new UserPanel() {
                    Speaker = speaker,
                    Opacity = 0.6
                });
            }
        }

        public static void Remove(string userId) {
            UserPanel panelToRemove = null;
            foreach (var panel in panels.Children.OfType<UserPanel>())
                if (panel.Speaker.Id == userId)
                    panelToRemove = panel;
            if (panelToRemove != null)
                panels.Children.Remove(panelToRemove);
        }
    }
}
