using Dec.DiscordIPC.Events;

namespace IPCHandler {
    public class Speaker {
        public static Speaker Convert(VoiceStateCreate.Data data) {
            string avatarUrl;
            if (data?.user?.avatar is null is false)
                avatarUrl = $"https://cdn.discordapp.com/avatars/{data.user.id}/{data.user.avatar}.png?size=64";
            else {
                int remainder = int.Parse(data.user.discriminator) % 5;
                avatarUrl = $"https://cdn.discordapp.com/embed/avatars/{remainder}.png?size=64";
            }

            return new Speaker() {
                Id = data.user.id,
                Name = data.nick ?? data.user.username,
                AvatarUrl = avatarUrl,

                Mute = data.voice_state.mute.GetValueOrDefault(),
                Deaf = data.voice_state.deaf.GetValueOrDefault(),
                SelfMute = data.voice_state.self_mute.GetValueOrDefault(),
                SelfDeaf = data.voice_state.self_deaf.GetValueOrDefault()
            };
        }

        public string Id;
        public string Name;
        public string AvatarUrl;

        public bool Mute;
        public bool Deaf;
        public bool SelfMute;
        public bool SelfDeaf;
    }
}
