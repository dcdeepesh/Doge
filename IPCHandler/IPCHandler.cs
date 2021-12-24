using Dec.DiscordIPC;
using Dec.DiscordIPC.Commands;
using Dec.DiscordIPC.Entities;
using Dec.DiscordIPC.Events;

using System;
using System.Threading.Tasks;

namespace IPCHandler {
    public class IPCHandler {
        static readonly string CLIENT_ID = "872000127513010206";
        static readonly string ACCESS_TOKEN = "vav4Ae1dJeWozZZlgomSaFZObx9pSW";
        static readonly DiscordIPC client = new DiscordIPC(CLIENT_ID);
        static readonly object dummy = null;
        static string currentChannelId = "";

        public static event EventHandler<string> OnVoiceChannelJoin;
        public static event EventHandler OnVoiceChannelLeave;
        public static event EventHandler<User> OnUserJoin;
        public static event EventHandler<User> OnUserLeave;
        public static event EventHandler<UserState> OnUserUpdate;
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

        static void OnVoiceChannelSelectHandler(object _, VoiceChannelSelect.Data data) {
            if (data.channel_id is null) {
                OnVoiceChannelLeave?.Invoke(null, null);
                currentChannelId = "";
            } else {
                if (currentChannelId.Length != 0)
                    OnVoiceChannelLeave?.Invoke(null, null);
                currentChannelId = data.channel_id;
                OnVoiceChannelJoin?.Invoke(null, data.channel_id);
            }
        }

        #region Other event dispatchers

        static readonly EventHandler<SpeakingStart.Data> hSpeakingStart =
            (sender, data) => OnSpeakingStart?.Invoke(dummy, data.user_id);
        static readonly EventHandler<SpeakingStop.Data> hSpeakingStop =
            (sender, data) => OnSpeakingStop?.Invoke(dummy, data.user_id);
        static readonly EventHandler<VoiceStateCreate.Data> hVSCreate =
            (sender, data) => OnUserJoin?.Invoke(dummy, data.user);
        static readonly EventHandler<VoiceStateDelete.Data> hVSDelete =
            (sender, data) => OnUserLeave?.Invoke(dummy, data.user);
        static readonly EventHandler<VoiceStateUpdate.Data> hVSUpdate =
            (sender, data) => OnUserUpdate?.Invoke(dummy, new UserState {
                Deaf = data.voice_state.deaf.GetValueOrDefault(),
                SelfDeaf = data.voice_state.self_deaf.GetValueOrDefault(),
                Mute = data.voice_state.mute.GetValueOrDefault(),
                SelfMute = data.voice_state.self_mute.GetValueOrDefault()
            });

        static SpeakingStart.Args aSpeakingStart;
        static SpeakingStop.Args aSpeakingStop;
        static VoiceStateCreate.Args aVSCreate;
        static VoiceStateDelete.Args aVSDelete;
        static VoiceStateUpdate.Args aVSUpdate;

        static async void OnVoiceChannelJoinHandler(object _, string channelId) {
            aSpeakingStart = new SpeakingStart.Args() { channel_id = channelId };
            aSpeakingStop = new SpeakingStop.Args() { channel_id = channelId };
            aVSCreate = new VoiceStateCreate.Args() { channel_id = channelId };
            aVSDelete = new VoiceStateDelete.Args() { channel_id = channelId };
            aVSUpdate = new VoiceStateUpdate.Args() { channel_id = channelId };

            client.OnSpeakingStart += hSpeakingStart;
            client.OnSpeakingStop += hSpeakingStop;
            client.OnVoiceStateCreate += hVSCreate;
            client.OnVoiceStateDelete += hVSDelete;
            client.OnVoiceStateUpdate += hVSUpdate;

            await client.SubscribeAsync(aSpeakingStart);
            await client.SubscribeAsync(aSpeakingStop);
            await client.SubscribeAsync(aVSCreate);
            await client.SubscribeAsync(aVSDelete);
            await client.SubscribeAsync(aVSUpdate);
        }

        static async void OnVoiceChannelLeaveHandler(object sender, EventArgs args) {
            await client.UnsubscribeAsync(aSpeakingStart);
            await client.UnsubscribeAsync(aSpeakingStop);
            await client.UnsubscribeAsync(aVSCreate);
            await client.UnsubscribeAsync(aVSDelete);
            await client.UnsubscribeAsync(aVSUpdate);

            client.OnSpeakingStart -= hSpeakingStart;
            client.OnSpeakingStop -= hSpeakingStop;
            client.OnVoiceStateCreate -= hVSCreate;
            client.OnVoiceStateDelete -= hVSDelete;
            client.OnVoiceStateUpdate -= hVSUpdate;
        }

        #endregion
    }
}
