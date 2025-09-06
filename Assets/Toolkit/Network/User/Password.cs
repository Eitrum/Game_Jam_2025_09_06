using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Toolkit.Network
{
    public static class Password
    {
        #region Classes

        public enum SecurityType
        {
            None = 0,
            Custom = 1,
            [InspectorName("")] _ = 2,

            [InspectorName("MD5 (128 bits")] MD5 = 6,
            [InspectorName("SHA/SHA-1 (160 bits)")] SHA1 = 8,
            [InspectorName("SHA/SHA-256 (256 bits)")] SHA256 = 9,
            [InspectorName("SHA/SHA-384 (384 bits)")] SHA384 = 10,
            [InspectorName("SHA/SHA-512 (512 bits)")] SHA512 = 11,

            [InspectorName("HMAC/HMAC (160 bits)")] HMAC = 16,
            [InspectorName("HMAC/HMAC SHA-1 (160 bits)")] HMAC_SHA1 = 17,
            [InspectorName("HMAC/HMAC SHA-256 (256 bits)")] HMAC_SHA256 = 18,
            [InspectorName("HMAC/HMAC SHA-384 (384 bits)")] HMAC_SHA384 = 19,
            [InspectorName("HMAC/HMAC SHA-512 (512 bits)")] HMAC_SHA512 = 20,
            [InspectorName("HMAC/HMAC MD5 (128 bits)")] HMAC_MD5 = 21,
        }

        [System.Flags]
        public enum SecurityMode
        {
            None = 0,
            Salt = 1,
            Pepper = 2,
            Salt_Pepper = Salt | Pepper,
        }

        public interface IPassword
        {
            string Identifier { get; }
            SecurityType Type { get; }
            SecurityMode Mode { get; }
            byte[] Create(string password, string salt);
        }

        #endregion

        #region Variables

        private const string TAG = "[Password] - ";
        // private const string DEFAULT_FORMAT = "{PASSWORD}{SALT}{PEPPER}";
#if SERVER
        private const string PEPPER = "seebWasHere";
#else
        private const string PEPPER = "seebWasNotHere"; // Used to throw off hackers on non-server versions.
#endif
        private static bool log = false;
        private static IPassword overridePasswordSystem;

        private static SecurityType type = SecurityType.SHA256;
        private static SecurityMode mode = SecurityMode.Salt_Pepper;

        #endregion

        #region Properties

        public static bool EnableLogging {
            get => log;
            set => log = value;
        }

        public static IPassword Manager {
            get => overridePasswordSystem;
            set {
                overridePasswordSystem = value;
                if(log)
                    Debug.LogWarning(TAG + $"Updated password manager: {overridePasswordSystem.Identifier}");
            }
        }

        public static string Identifier => overridePasswordSystem?.Identifier ?? "Default";

        public static SecurityType Type {
            get => overridePasswordSystem?.Type ?? type;
            set => type = value;
        }

        public static SecurityMode Mode {
            get => overridePasswordSystem?.Mode ?? mode;
            set => mode = value;
        }

        #endregion

        #region Create

        public static byte[] Create(string password, User u)
            => Create(password, u.PasswordSalt);

        public static byte[] Create(string password, string salt) {
            if(overridePasswordSystem != null) {
                if(log)
                    Debug.Log(TAG + $"Creating password with: {password}+{salt}");
                return overridePasswordSystem.Create(password, salt);
            }
            return Create(password, salt, Type, Mode);
        }

        public static byte[] Create(string password, string salt, SecurityType type, SecurityMode mode) {
            if(log)
                Debug.Log(TAG + $"Creating password with: {password}+{salt}");

            if(type == SecurityType.Custom && overridePasswordSystem != null) {
                return overridePasswordSystem.Create(password, salt);
            }

            string completeString = FormatString(password, salt, mode);
            var bytes = Encoding.UTF32.GetBytes(completeString);

            var hash = GetAlgorithm(type);
            if(hash == null) {
                throw new Exception(TAG + $"No provided security type provided '{type}' '{mode}'");
            }
            return hash.ComputeHash(bytes);
        }

        #endregion

        #region Compare

        public static bool Compare(User u, string password) {
            var temporary = Create(password, u.PasswordSalt, u.PasswordType, u.PasswordMode);
            var uData = u.PasswordData;
            if(temporary.Length != uData.Length)
                return false;

            for(int i = 0, length = uData.Length; i < length; i++) {
                if(temporary[i] != uData[i])
                    return false;
            }

            return true;
        }

        public static bool Compare(byte[] hashed, string password, string salt, SecurityType type, SecurityMode mode) {
            var temporary = Create(password, salt, type, mode);
            if(temporary.Length != hashed.Length)
                return false;

            for(int i = 0, length = hashed.Length; i < length; i++) {
                if(temporary[i] != hashed[i])
                    return false;
            }

            return true;
        }

        #endregion

        #region Utility

        public static string FormatString(string password, string salt, SecurityMode mode) {
            switch(mode) {
                case SecurityMode.Salt: return $"{password}{salt}";
                case SecurityMode.Salt_Pepper: return $"{password}{salt}" + PEPPER;
                case SecurityMode.Pepper: return password + PEPPER;
            }
            return password;
        }

        public static HashAlgorithm GetAlgorithm(SecurityType type) {
            switch(type) {
                case SecurityType.SHA256: return SHA256.Create();
                case SecurityType.SHA512: return SHA512.Create();
                case SecurityType.SHA1: return SHA1.Create();
                case SecurityType.SHA384: return SHA384.Create();
                case SecurityType.HMAC: return HMAC.Create();
                case SecurityType.HMAC_SHA1: return new HMACSHA1();
                case SecurityType.HMAC_SHA256: return new HMACSHA256();
                case SecurityType.HMAC_SHA384: return new HMACSHA384();
                case SecurityType.HMAC_SHA512: return new HMACSHA512();
                case SecurityType.HMAC_MD5: return new HMACMD5();
            }
            return null;
        }

        #endregion

        public static class Salt
        {
            #region Variables

            public const int DEFAULT_AMOUNT = 8;
            public const string URL_SAFE_TOKENS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
            public const int URL_SAFE_TOKENS_LENGTH = 64;
            private static System.Random rand = new System.Random();

            #endregion

            #region URL_Safe

            public static string URL_Safe() => URL_Safe(DEFAULT_AMOUNT, rand);
            public static string URL_Safe(int amount) => URL_Safe(amount, rand);
            public static string URL_Safe(System.Random random) => URL_Safe(DEFAULT_AMOUNT, random);
            public static string URL_Safe(int amount, System.Random random) {
                StringBuilder sb = new StringBuilder(amount, amount);
                for(int i = 0; i < amount; i++) {
                    sb.Append(URL_SAFE_TOKENS[random.Next() % URL_SAFE_TOKENS_LENGTH]);
                }
                return sb.ToString();
            }

            #endregion

            #region Full Random

            public static string Generate_Unsafe() => Generate_Unsafe(DEFAULT_AMOUNT, rand);
            public static string Generate_Unsafe(int amount) => Generate_Unsafe(amount, rand);
            public static string Generate_Unsafe(System.Random random) => Generate_Unsafe(DEFAULT_AMOUNT, random);
            public static string Generate_Unsafe(int amount, System.Random random) {
                StringBuilder sb = new StringBuilder(amount, amount);
                for(int i = 0; i < amount; i++) {
                    sb.Append(random.Next(short.MaxValue));
                }
                return sb.ToString();
            }

            #endregion
        }
    }
}
