using IPCHandler;

using System.Configuration;

namespace Doge {
    internal static class IPCEventHandler {
        public static void Init() {
            IPCEventTranslator.OnVoiceChannelJoin += OnVoiceChannelJoin;
            IPCEventTranslator.OnVoiceChannelLeave += OnVoiceChannelLeave;
            IPCEventTranslator.OnUserJoinOrUpdate += OnUserJoinOrUpdate;
            IPCEventTranslator.OnUserLeave += OnUserLeave;
            IPCEventTranslator.OnSpeakingStart += OnSpeakingStart;
            IPCEventTranslator.OnSpeakingStop += OnSpeakingStop;

            _ = IPCEventTranslator.InitAndStartEventsAsync(AuthData.CLIENT_ID, Preferences.Current.AccessToken);
        }

        public static void Dispose() {
            IPCEventTranslator.OnVoiceChannelJoin -= OnVoiceChannelJoin;
            IPCEventTranslator.OnVoiceChannelLeave -= OnVoiceChannelLeave;
            IPCEventTranslator.OnUserJoinOrUpdate -= OnUserJoinOrUpdate;
            IPCEventTranslator.OnUserLeave -= OnUserLeave;
            IPCEventTranslator.OnSpeakingStart -= OnSpeakingStart;
            IPCEventTranslator.OnSpeakingStop -= OnSpeakingStop;

            IPCEventTranslator.ShutdownAsync()
                .ConfigureAwait(false).GetAwaiter().GetResult();
        }

        #region Translated event handlers

        public static void OnVoiceChannelJoin(object sender, string channelId) {
            OverlayManager.ActivateOverlay();
        }

        public static void OnVoiceChannelLeave(object sender, object args) {
            OverlayManager.DeactivateOverlay();
        }

        public static void OnUserJoinOrUpdate(object sender, SpeakerDto speaker) {
            if (speaker is not null)
                OverlayManager.AddOrUpdateSpeaker(speaker);
        }

        public static void OnUserLeave(object sender, string userId) {
            OverlayManager.RemoveSpeaker(userId);
        }

        public static void OnSpeakingStart(object sender, string userId) {
            OverlayManager.StartSpeaking(userId);
        }

        public static void OnSpeakingStop(object sender, string userId) {
            OverlayManager.StopSpeaking(userId);
        }

        #endregion
    }
}
