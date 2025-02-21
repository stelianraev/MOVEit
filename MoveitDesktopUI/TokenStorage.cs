namespace MoveitWpf
{
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    namespace MoveitDesktopUI
    {
        public static class TokenStorage
        {
            private static readonly string AccessTokenFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyAppAccessToken.dat");

            public static void SaveAccessToken(string token, DateTime expiresDateTime)
            {
                var tokenData = new TokenData(token, expiresDateTime);

                string json = JsonConvert.SerializeObject(tokenData);
                byte[] encryptedData = ProtectedData.Protect(Encoding.UTF8.GetBytes(json), null, DataProtectionScope.CurrentUser);
                File.WriteAllBytes(AccessTokenFilePath, encryptedData);
            }

            public static TokenData GetAccessToken()
            {
                if (!File.Exists(AccessTokenFilePath))
                    return null;

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

            public record TokenData(string AccessToken, DateTime ExpiresDateTime);
        }
    }
}
