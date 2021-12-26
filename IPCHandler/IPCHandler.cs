using Dec.DiscordIPC;
using Dec.DiscordIPC.Commands;
using Dec.DiscordIPC.Events;

using System;
using System.Threading.Tasks;

namespace IPCHandler {
    public class IPCHandler {
        static readonly string CLIENT_ID = "872000127513010206";
        static readonly string ACCESS_TOKEN = "vav4Ae1dJeWozZZlgomSaFZObx9pSW";
        static readonly DiscordIPC client = new DiscordIPC(CLIENT_ID);
        static string currentChannelId;

        public static event EventHandler<string> OnVoiceChannelJoin;
        public static event EventHandler OnVoiceChannelLeave;
        public static event EventHandler<Speaker> OnUserJoinOrUpdate;
        public static event EventHandler<string> OnUserLeave;
        public static event EventHandler<string> OnSpeakingStart;
        public static event EventHandler<string> OnSpeakingStop;

        static readonly VoiceChannelSelect.Args aVoiceChannelSelect = new VoiceChannelSelect.Args();

        public static async Task InitAsync() {
            await client.InitAsync();
            await client.SendCommandAsync(new Authenticate.Args() {
                access_token = ACCESS_TOKEN
            });

            OnVoiceChannelJoin += OnVoiceChannelJoinHandler;
            OnVoiceChannelLeave += OnVoiceChannelLeaveHandler;
        }

        public static async Task StartEventsAsync() {
            var response = await client.SendCommandAsync(new GetSelectedVoiceChannel.Args() { });
            if (response is null is false) {
                currentChannelId = response.id;
                OnVoiceChannelJoin?.Invoke(null, response.id);
            }

            client.OnVoiceChannelSelect += OnVoiceChannelSelectHandler;
            await client.SubscribeAsync(aVoiceChannelSelect);
        }

        public static async Task ShutdownAsync() {
            OnVoiceChannelJoin -= OnVoiceChannelJoinHandler;
            OnVoiceChannelLeave -= OnVoiceChannelLeaveHandler;

            await client.UnsubscribeAsync(aVoiceChannelSelect);
            client.OnVoiceChannelSelect -= OnVoiceChannelSelectHandler;
            client.Dispose();
        }

        static void OnVoiceChannelSelectHandler(object sender, VoiceChannelSelect.Data data) {
            if (data.channel_id is null) {
                OnVoiceChannelLeave?.Invoke(sender, null);
            } else {
                if (currentChannelId != null)
                    OnVoiceChannelLeave?.Invoke(sender, null);
                OnVoiceChannelJoin?.Invoke(sender, data.channel_id);
            }

            currentChannelId = data.channel_id;
        }

        #region Other event dispatchers

        static readonly EventHandler<SpeakingStart.Data> hSpeakingStart =
            (sender, data) => OnSpeakingStart?.Invoke(sender, data.user_id);
        static readonly EventHandler<SpeakingStop.Data> hSpeakingStop =
            (sender, data) => OnSpeakingStop?.Invoke(sender, data.user_id);
        static readonly EventHandler<VoiceStateCreate.Data> hVSCreate =
            (sender, data) => OnUserJoinOrUpdate?.Invoke(sender, Speaker.Convert(data));
        static readonly EventHandler<VoiceStateUpdate.Data> hVSUpdate =
            (sender, data) => OnUserJoinOrUpdate?.Invoke(sender, Speaker.Convert(data));
        static readonly EventHandler<VoiceStateDelete.Data> hVSDelete =
            (sender, data) => OnUserLeave?.Invoke(sender, data.user.id);

        static SpeakingStart.Args aSpeakingStart;
        static SpeakingStop.Args aSpeakingStop;
        static VoiceStateCreate.Args aVSCreate;
        static VoiceStateUpdate.Args aVSUpdate;
        static VoiceStateDelete.Args aVSDelete;

        static async void OnVoiceChannelJoinHandler(object _, string channelId) {
            aSpeakingStart = new SpeakingStart.Args() { channel_id = channelId };
            aSpeakingStop = new SpeakingStop.Args() { channel_id = channelId };
            aVSCreate = new VoiceStateCreate.Args() { channel_id = channelId };
            aVSUpdate = new VoiceStateUpdate.Args() { channel_id = channelId };
            aVSDelete = new VoiceStateDelete.Args() { channel_id = channelId };

            client.OnSpeakingStart += hSpeakingStart;
            client.OnSpeakingStop += hSpeakingStop;
            client.OnVoiceStateCreate += hVSCreate;
            client.OnVoiceStateUpdate += hVSUpdate;
            client.OnVoiceStateDelete += hVSDelete;

            await client.SubscribeAsync(aSpeakingStart);
            await client.SubscribeAsync(aSpeakingStop);
            await client.SubscribeAsync(aVSCreate);
            await client.SubscribeAsync(aVSUpdate);
            await client.SubscribeAsync(aVSDelete);

            var selectedVC = await client.SendCommandAsync(new GetSelectedVoiceChannel.Args() { });
            if (selectedVC.voice_states is null is false && selectedVC.voice_states.Count > 0)
                foreach (var voiceState in selectedVC.voice_states)
                    OnUserJoinOrUpdate?.Invoke(null, Speaker.Convert(voiceState));
        }

        static async void OnVoiceChannelLeaveHandler(object sender, EventArgs args) {
            await client.UnsubscribeAsync(aSpeakingStart);
            await client.UnsubscribeAsync(aSpeakingStop);
            await client.UnsubscribeAsync(aVSCreate);
            await client.UnsubscribeAsync(aVSUpdate);
            await client.UnsubscribeAsync(aVSDelete);

            client.OnSpeakingStart -= hSpeakingStart;
            client.OnSpeakingStop -= hSpeakingStop;
            client.OnVoiceStateCreate -= hVSCreate;
            client.OnVoiceStateUpdate -= hVSUpdate;
            client.OnVoiceStateDelete -= hVSDelete;
        }

        #endregion
    }
}
