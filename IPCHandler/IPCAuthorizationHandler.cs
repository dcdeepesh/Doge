using Dec.DiscordIPC;
using Dec.DiscordIPC.Commands;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace IPCHandler {
    public class IPCAuthorizationHandler {
        public static async Task<string> GetAuthCodeAsync(string clientId) {
            DiscordIPC discord = new(clientId);
            var response = await discord.SendCommandAsync(new Authorize.Args {
                client_id = clientId,
                scopes = new List<string> {
                    Authenticate.OAuth2Scopes.RPC,
                    Authenticate.OAuth2Scopes.RPC_VOICE_READ
                }
            });

            discord.Dispose();
            return response.code;
        }
    }
}
