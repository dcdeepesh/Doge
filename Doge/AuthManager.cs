using IPCHandler;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Doge {
    // TODO handle situation where refresh token gets expired (after an year)
    // TODO whatif access token is invalidated before time?
    internal class AuthManager {
        private static readonly string TOKEN_URL = "https://discord.com/api/oauth2/token";

        public static async Task PrepareAccessTokenAsync() {
            if (Preferences.Current.AuthPending)
                return;

            var currentTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            if (currentTime >= Preferences.Current.AccessTokenValidUntil) {
                await RefreshTokenAsync();
            }
        }

        public static async Task<bool> AuthorizeAsync() {
            string authCode;
            try {
                authCode = await IPCAuthorizationHandler.GetAuthCodeAsync(AuthData.CLIENT_ID);
            } catch (IPCAuthorizationHandler.AuthorizationDeniedException) {
                return false;
            }

            using HttpClient client = new();
            var response = await client.PostAsync(TOKEN_URL, new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new("client_id", AuthData.CLIENT_ID),
                new("client_secret", AuthData.CLIENT_SECRET),
                new("code", authCode),
                new("redirect_uri", AuthData.REDIRECT_URI)
            }));

            // if something unexpected happens
            if (response.StatusCode != HttpStatusCode.OK &&
                response.StatusCode != HttpStatusCode.BadRequest)
                return false;

            // all good, now either the code is valid or it's not
            var responseJson = await response.Content.ReadAsStringAsync();
            var responseDto = JsonSerializer.Deserialize<TokenResponse>(responseJson);

            if (response.IsSuccessStatusCode) {
                // valid refresh token, update credentials
                Preferences.Current.AccessToken = responseDto.access_token;
                Preferences.Current.RefreshToken = responseDto.refresh_token;
                // 999 (instead of 1000) to invalidate token a bit before actual invalidation; margin to stay safe
                Preferences.Current.AccessTokenValidUntil = responseDto.expires_in * 999 +
                    new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
                Preferences.Current.AuthPending = false;
                Preferences.Save();
            } else if (response.StatusCode.Equals(HttpStatusCode.BadRequest)) {
                // invalid auth code, TODO what to do now?
            }

            return true;
        }

        private static async Task RefreshTokenAsync() {
            using HttpClient client = new();
            var response = await client.PostAsync(TOKEN_URL, new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new("client_id", AuthData.CLIENT_ID),
                new("client_secret", AuthData.CLIENT_SECRET),
                new("refresh_token", Preferences.Current.RefreshToken)
            }));

            // if something unexpected happens
            if (response.StatusCode != HttpStatusCode.OK &&
                response.StatusCode != HttpStatusCode.BadRequest)
                return;

            // all good, now either the refresh token is valid or it's not
            var responseJson = await response.Content.ReadAsStringAsync();
            var responseDto = JsonSerializer.Deserialize<TokenResponse>(responseJson);

            if (response.IsSuccessStatusCode) {
                // valid refresh token, update credentials
                Preferences.Current.AccessToken = responseDto.access_token;
                Preferences.Current.RefreshToken = responseDto.refresh_token;
                // 999 (instead of 1000) to invalidate token a bit before actual invalidation; margin to stay safe
                Preferences.Current.AccessTokenValidUntil = responseDto.expires_in * 999 +
                    new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
                Preferences.Save();
            } else if (response.StatusCode.Equals(HttpStatusCode.BadRequest)) {
                // invalid (outdated) refresh token, so re-authorize
                await AuthorizeAsync();
            }
        }

#pragma warning disable IDE1006 // Naming Styles

        private class TokenResponse {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string refresh_token { get; set; }
            public string scope { get; set; }
            public string token_type { get; set; }

            public string error { get; set; }
        }
    }

#pragma warning restore IDE1006 // Naming Styles
}
