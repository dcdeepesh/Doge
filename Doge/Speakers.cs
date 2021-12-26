using IPCHandler;

using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Doge {
    internal class Speakers {
        static StackPanel panels;

        public static void Init(StackPanel userPanels) {
            panels = userPanels;
        }

        public static void AddOrUpdate(Speaker speaker) {
            string uri = $"https://cdn.discordapp.com/avatars/{speaker.Id}/{speaker.AvatarHash}.png?size=64";
            bool found = false;
            foreach (var panel in panels.Children.OfType<UserPanel>()) {
                if (panel.Speaker.Id == speaker.Id) {
                    found = true;
                    panel.Avatar = new BitmapImage(new Uri(uri));
                    panel.Speaker = speaker;
                    panel.SpeakerName = speaker.Name;
                }
            }

            if (!found) {
                panels.Children.Add(new UserPanel() {
                    Speaker = speaker,
                    Avatar = new BitmapImage(new Uri(uri)),
                    SpeakerName = speaker.Name
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
