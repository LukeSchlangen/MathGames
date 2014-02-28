using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.DataModel;

namespace ProjectDelta
{
    static class GameUtils
    {

        private const string initVector = "AbamathTomLuke90";
        private static User user = null;

        // This constant is used to determine the keysize of the encryption algorithm.
        private const int keysize = 256;

        public static string encrypt(string passwordToEncrypt, string usernameAsKey)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(passwordToEncrypt);
            PasswordDeriveBytes password = new PasswordDeriveBytes(usernameAsKey, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
        }

        public static void databaseEncryptionHelper(string username, DynamoDBContext context)
        {
            user = context.Load<User>(username);
            Debug.WriteLine("here");
            user.password = GameUtils.encrypt(user.password, username);
            context.Save<User>(user);
        }
    }
}
