using IPCHandler;

using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows;

namespace Doge {
    internal static class OverlayManager {
        private static OverlayWindow overlayWindow;
        private static UIElementCollection SpeakerPanels => overlayWindow?.SpeakerPanels?.Children;

        public static void ActivateOverlay() {
            if (overlayWindow is null)
                Dispatch(() => overlayWindow = new());

            DispatchAsync(() => overlayWindow.Show());
        }

        public static void DeactivateOverlay() {
            if (overlayWindow is null)
                return;

            DispatchAsync(() => {
                overlayWindow.Hide();
                SpeakerPanels.Clear();
            });
        }

        public static void AddOrUpdateSpeaker(SpeakerDto speaker) {
            if (overlayWindow is null)
                return;

            DispatchAsync(() => {
                var matchedSpeakers = SpeakerPanels
                    .OfType<SpeakerPanel>()
                    .Where(panel => panel.Speaker.Id == speaker.Id);

                if (matchedSpeakers.Any()) {
                    matchedSpeakers.Single().Speaker = speaker;
                } else {
                    SpeakerPanels.Add(new SpeakerPanel(speaker));
                }
            });
        }

        public static void RemoveSpeaker(string userId) {
            if (overlayWindow is null)
                return;

            DispatchAsync(() => {
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
            if (overlayWindow is null)
                return;

            DispatchAsync(() => {
                SpeakerPanels
                    .OfType<SpeakerPanel>()
                    .Where(panel => panel.Speaker.Id == userId)
                    .Single()
                    .Opacity = Preferences.Current.SpeakingOpacity / 100;
            });
        }

        public static void StopSpeaking(string userId) {
            if (overlayWindow is null)
                return;

            DispatchAsync(() => {
                SpeakerPanels
                    .OfType<SpeakerPanel>()
                    .Where(panel => panel.Speaker.Id == userId)
                    .Single()
                    .Opacity = Preferences.Current.IdleOpacity / 100;
            });
        }

        private static void DispatchAsync(Action action) =>
            (overlayWindow?.Dispatcher ?? Application.Current.Dispatcher).BeginInvoke(action);
        private static void Dispatch(Action action) =>
            (overlayWindow?.Dispatcher ?? Application.Current.Dispatcher).Invoke(action);
    }
}
