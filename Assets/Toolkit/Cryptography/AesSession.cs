using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace Toolkit.Cryptography
{
    public class AesUtility
    {
        public enum KeyLength
        {
            _128 = 16,
            _192 = 24,
            _256 = 32,
        }

        #region Variables

        public const KeyLength DEFAULT_KEY_LENGTH = KeyLength._256;

        #endregion
    }

    public class AesSession
    {
        private Aes aes;
        private byte[] buffer = new byte[2048];

        public byte[] Key => aes.Key;
        public byte[] IV => aes.IV;

        public AesSession() {
            aes = Aes.Create();
            aes.GenerateKey();
            aes.GenerateIV();
        }

        public AesSession(string password) {
            aes = Aes.Create();
            aes.KeySize = 256;
            aes.Key = SHA256.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            aes.IV = System.Text.Encoding.UTF8.GetBytes("ABCD-EFG-FHI-JLM");
        }

        public AesSession(byte[] key, byte[] iv) {
            aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
        }

        public byte[] Encrypt(byte[] data) {
            var encryptor = aes.CreateEncryptor();
            MemoryStream ms = new MemoryStream();
            using(var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)) {
                cs.Write(data, 0, data.Length);
            }

            return ms.ToArray();
        }

        public byte[] Decrypt(byte[] cypher) {
            var decryptor = aes.CreateDecryptor();
            MemoryStream ms = new MemoryStream();
            using(var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read)) {
                if(cs.Length > buffer.Length && cs.Length < short.MaxValue) {
                    buffer = new byte[UnityEngine.Mathf.NextPowerOfTwo((int)cs.Length)];
                }
                int amount = cs.Read(buffer, 0, buffer.Length);
                byte[] arr = new byte[amount];
                Array.Copy(buffer, 0, arr, 0, amount);
                return arr;
            }
        }
    }

    public static class CryptographyUtility
    {
        public static SymmetricAlgorithm GetSymmetric(CryptoAlgorithm algorithm) {
            switch(algorithm) {
                case CryptoAlgorithm.Aes: return Aes.Create();
                case CryptoAlgorithm.AesCryptoServiceProvider: return new AesCryptoServiceProvider();
                case CryptoAlgorithm.AesManaged: return new AesManaged();
            }
            throw new System.Exception("Unsupported algorithm!");
        }
    }

    public enum AesAlgorithm
    {
        None = 0,
        Aes = 10,
        AesManaged = 11,
        AesCryptoServiceProvider = 12,

        AesCng = 13,
        AesCcm = 14,
    }

    public enum CryptoAlgorithm
    {
        None,

        Aes = 10,
        AesManaged = 11,
        AesCryptoServiceProvider = 12,

        AesCng = 13,
        AesCcm = 14,

    }
}
