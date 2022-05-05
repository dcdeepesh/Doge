using IPCHandler;

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
                SpeakerPanels.Remove(SpeakerPanels
                    .OfType<SpeakerPanel>()
                    .Single(panel => panel.Speaker.Id == userId));
            });
        }

        public static void StartSpeaking(string userId) {
            if (overlayWindow is null)
                return;

            DispatchAsync(() => {
                var panel = SpeakerPanels
                    .OfType<SpeakerPanel>()
                    .Where(panel => panel.Speaker.Id == userId)
                    .Single();
                panel.BindSpeakingOpacity();
                panel.UnbindAlwaysVisibility();
                panel.Visibility = Visibility.Visible;
            });
        }

        public static void StopSpeaking(string userId) {
            if (overlayWindow is null)
                return;

            DispatchAsync(() => {
                var panel = SpeakerPanels
                    .OfType<SpeakerPanel>()
                    .Where(panel => panel.Speaker.Id == userId)
                    .Single();
                panel.BindIdleOpacity();
                panel.BindAlwaysVisibility();
            });
        }

        private static void DispatchAsync(Action action) =>
            (overlayWindow?.Dispatcher ?? Application.Current.Dispatcher).BeginInvoke(action);
        private static void Dispatch(Action action) =>
            (overlayWindow?.Dispatcher ?? Application.Current.Dispatcher).Invoke(action);
    }
}
