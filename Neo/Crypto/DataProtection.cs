using System;
using System.Text;
using System.Security.Cryptography;

namespace Neo.Crypto
{
    internal class DataProtection
    {
        private readonly byte[] mEntropy = { 2, 1, 4, 2, 9 };

        public string Encrpyt(string unprotectedData)
        {
            try
            {
                var rawByte = Encoding.Default.GetBytes(unprotectedData);
                var protectedByte = ProtectedData.Protect(rawByte, this.mEntropy, DataProtectionScope.CurrentUser);
                return Convert.ToBase64String(protectedByte);
            }
            catch (CryptographicException e)
            {
                Log.Error("Unable to encrypt data: " + e.Message);
                return null;
            }
        }

        public string Decrpyt(string data)
        {
            try
            {
                var encryptedByte = Convert.FromBase64String(data);

                var rawByte = ProtectedData.Unprotect(encryptedByte, this.mEntropy, DataProtectionScope.CurrentUser);
                return Encoding.Default.GetString(rawByte);
            }
            catch (CryptographicException e)
            {
                Log.Error("Unable to decrypt data: " + e.Message);
                return null;
            }
        }
    }
}

