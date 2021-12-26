using Dec.DiscordIPC.Entities;
using Dec.DiscordIPC.Events;

namespace IPCHandler {
    public class Speaker {
        public static Speaker Convert(VoiceState vs) {
            if ((vs is null) || (vs.user_id is null))
                return null;
            else return new Speaker() {
                Id = vs.user_id,
                Name = vs.member?.nick ?? vs.member?.user?.username,
                AvatarHash = vs.member?.user?.avatar,
                Mute = vs.mute.GetValueOrDefault(),
                Deaf = vs.deaf.GetValueOrDefault(),
                SelfMute = vs.self_mute.GetValueOrDefault(),
                SelfDeaf = vs.self_deaf.GetValueOrDefault()
            };
        }

        public static Speaker Convert(VoiceStateCreate.Data data) {
            return Convert(data.voice_state) ?? new Speaker() {
                Id = data.user.id,
                Name = data.nick ?? data.user.username,
                AvatarHash = data.user.avatar,
            };
        }

        public string Id;
        public string Name;
        public string AvatarHash;

        public bool Mute;
        public bool Deaf;
        public bool SelfMute;
        public bool SelfDeaf;
    }
}
