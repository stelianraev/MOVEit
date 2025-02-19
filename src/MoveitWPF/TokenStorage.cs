using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MoveitWpf
{
    public static class TokenStorage
    {
        private static readonly string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyAppToken.dat");

        public static void SaveToken(string token)
        {
            byte[] encryptedData = ProtectedData.Protect(Encoding.UTF8.GetBytes(token), null, DataProtectionScope.CurrentUser);
            File.WriteAllBytes(FilePath, encryptedData);
        }
        public static string GetToken()
        {
            if (!File.Exists(FilePath))
                return null;

            byte[] encryptedData = File.ReadAllBytes(FilePath);
            byte[] decryptedData = ProtectedData.Unprotect(encryptedData, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(decryptedData);
        }
        public static void RemoveToken()
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }
    }
}
