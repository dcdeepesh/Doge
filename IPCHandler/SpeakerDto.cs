using Dec.DiscordIPC.Events;

using System.ComponentModel;

namespace IPCHandler {
    public class SpeakerDto : INotifyPropertyChanged {
        public string Id { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }

        public bool Mute { get; set; }
        public bool Deaf { get; set; }
        public bool SelfMute { get; set; }
        public bool SelfDeaf { get; set; }

        public static SpeakerDto From(VoiceStateCreate.Data data) {
            string avatarUrl;
            if (data?.user?.avatar is not null)
                avatarUrl = $"https://cdn.discordapp.com/avatars/{data.user.id}/{data.user.avatar}.png?size=64";
            else {
                int remainder = int.Parse(data.user.discriminator) % 5;
                avatarUrl = $"https://cdn.discordapp.com/embed/avatars/{remainder}.png?size=64";
            }

            return new SpeakerDto() {
                Id = data.user.id,
                Name = data.nick ?? data.user.username,
                AvatarUrl = avatarUrl,

                Mute = data.voice_state.mute.GetValueOrDefault(),
                Deaf = data.voice_state.deaf.GetValueOrDefault(),
                SelfMute = data.voice_state.self_mute.GetValueOrDefault(),
                SelfDeaf = data.voice_state.self_deaf.GetValueOrDefault()
            };
        }

        #region Property change events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
