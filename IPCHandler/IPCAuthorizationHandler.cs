using Dec.DiscordIPC;
using Dec.DiscordIPC.Commands;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace IPCHandler {
    public class IPCAuthorizationHandler {
        public static async Task<string> GetAuthCodeAsync(string clientId) {
            DiscordIPC discordIpc = new(clientId);
            await discordIpc.InitAsync();
            var response = await discordIpc.SendCommandAsync(new Authorize.Args {
                client_id = clientId,
                scopes = new List<string> {
                    Authenticate.OAuth2Scopes.RPC,
                    Authenticate.OAuth2Scopes.RPC_VOICE_READ
                }
            });

            discordIpc.Dispose();
            return response.code;
        }
    }
}
