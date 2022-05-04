using IPCHandler;

using System.Linq;
using System.Windows.Controls;
using System.Windows;

namespace Doge {
    internal static class OverlayManager {
        private static OverlayWindow overlayWindow;
        private static UIElementCollection SpeakerPanels => overlayWindow?.SpeakerPanels?.Children;

        public static void ActivateOverlay() {
            if (overlayWindow is null)
                Application.Current.Dispatcher.Invoke(() => overlayWindow = new());

            overlayWindow.Dispatcher.Invoke(() => overlayWindow.Show());
        }

        public static void DeactivateOverlay() {
            overlayWindow.Dispatcher.Invoke(() => {
                overlayWindow.Hide();
                SpeakerPanels.Clear();
            });
        }

        public static void AddOrUpdateSpeaker(SpeakerDto speaker) {
            overlayWindow?.Dispatcher?.Invoke(() => {
                var matchedSpeakers = SpeakerPanels.OfType<SpeakerPanel>().Where(panel => panel.Speaker.Id == speaker.Id);
                if (matchedSpeakers.Any()) {
                    matchedSpeakers.Single().Speaker = speaker;
                } else {
                    SpeakerPanels.Add(new SpeakerPanel(speaker) {
                        Opacity = 0.6
                    });
                }
            });
        }

        public static void RemoveSpeaker(string userId) {
            overlayWindow?.Dispatcher?.Invoke(() => {
                /*
                SpeakerPanel panelToRemove = null;
                foreach (var panel in SpeakerPanels.OfType<SpeakerPanel>())
                    if (panel.Speaker.Id == userId)
                        panelToRemove = panel;
                if (panelToRemove != null)
                    panels.Children.Remove(panelToRemove);
                */
                SpeakerPanels.Remove(SpeakerPanels
                    .OfType<SpeakerPanel>()
                    .Where(panel => panel.Speaker.Id == userId)
                    .Single());
            });
        }

        public static void StartSpeaking(string userId) {
            //Dispatcher.Invoke(() => {
            //    foreach (var panel in UserPanels.Children.OfType<SpeakerPanel>())
            //        if (panel.Speaker.Id == userId)
            //            panel.Opacity = 1;
            //});
        }

        public static void StopSpeaking(string userId) {
            //Dispatcher.Invoke(() => {
            //    foreach (var panel in UserPanels.Children.OfType<SpeakerPanel>())
            //        if (panel.Speaker.Id == userId)
            //            panel.Opacity = 0.6;
            //});
        }
    }
}
