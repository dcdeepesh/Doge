using IPCHandler;

using System.Configuration;

namespace Doge {
    internal static class IPCEventHandler {
        private static readonly string ACCESS_TOKEN = ConfigurationManager.AppSettings["accessToken"];
        private static readonly string CLIENT_ID = ConfigurationManager.AppSettings["clientId"];

        public static void Init() {
            IPCEventTranslator.OnVoiceChannelJoin += OnVoiceChannelJoin;
            IPCEventTranslator.OnVoiceChannelLeave += OnVoiceChannelLeave;
            IPCEventTranslator.OnUserJoinOrUpdate += OnUserJoinOrUpdate;
            IPCEventTranslator.OnUserLeave += OnUserLeave;
            IPCEventTranslator.OnSpeakingStart += OnSpeakingStart;
            IPCEventTranslator.OnSpeakingStop += OnSpeakingStop;

            IPCEventTranslator.InitAndStartEventsAsync(CLIENT_ID, ACCESS_TOKEN).Wait();
        }

        public static void Dispose() {
            IPCEventTranslator.OnVoiceChannelJoin -= OnVoiceChannelJoin;
            IPCEventTranslator.OnVoiceChannelLeave -= OnVoiceChannelLeave;
            IPCEventTranslator.OnUserJoinOrUpdate -= OnUserJoinOrUpdate;
            IPCEventTranslator.OnUserLeave -= OnUserLeave;
            IPCEventTranslator.OnSpeakingStart -= OnSpeakingStart;
            IPCEventTranslator.OnSpeakingStop -= OnSpeakingStop;
            
            IPCEventTranslator.ShutdownAsync().Wait();
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
