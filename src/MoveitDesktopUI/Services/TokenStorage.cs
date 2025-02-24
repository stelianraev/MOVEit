namespace MoveitWpf
{
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    namespace MoveitDesktopUI.Services
    {
        public static class TokenStorage
        {
            private static readonly string AccessTokenFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MoveitAccessToken.dat");

            public static void SaveAccessToken(string token, int tokenExpireSeconds, string refreshToken, DateTime expiresDateTime)
            {
                var tokenData = new TokenData(token, tokenExpireSeconds, refreshToken, expiresDateTime);

                string json = JsonConvert.SerializeObject(tokenData);
                byte[] encryptedData = ProtectedData.Protect(Encoding.UTF8.GetBytes(json), null, DataProtectionScope.CurrentUser);
                File.WriteAllBytes(AccessTokenFilePath, encryptedData);
            }

            public static TokenData GetAccessToken()
            {
                if (!File.Exists(AccessTokenFilePath))
                {
                    return null;
                }

                byte[] encryptedData = File.ReadAllBytes(AccessTokenFilePath);
                byte[] decryptedData = ProtectedData.Unprotect(encryptedData, null, DataProtectionScope.CurrentUser);
                string json = Encoding.UTF8.GetString(decryptedData);
                return JsonConvert.DeserializeObject<TokenData>(json);
            }

            public static void RemoveAccessToken()
            {
                if (File.Exists(AccessTokenFilePath))
                    File.Delete(AccessTokenFilePath);
            }

            public record TokenData(string AccessToken, int TokenExpireSeconds, string RefreshToken, DateTime ExpiresDateTime);
        }
    }
}
