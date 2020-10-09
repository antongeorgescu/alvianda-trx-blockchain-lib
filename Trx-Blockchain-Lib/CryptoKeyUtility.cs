using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Trx_Blockchain_Lib
{
    public static class CryptoKeyUtility
    {
        /// <summary>
        /// Generates a random salt value of the specified length.
        /// </summary>
        public static byte[] CreateRandomSalt(int length)
        {
            // Create a buffer
            byte[] randBytes;

            if (length >= 1)
            {
                randBytes = new byte[length];
            }
            else
            {
                randBytes = new byte[1];
            }

            // Create a new RNGCryptoServiceProvider.
            RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();

            // Fill the buffer with random bytes.
            rand.GetBytes(randBytes);

            // return the bytes.
            return randBytes;
        }

        /// <summary>
        /// Clear the bytes in a buffer so they can't later be read from memory.
        /// </summary>
        public static void ClearBytes(byte[] buffer)
        {
            // Check arguments.
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            // Set each byte in the buffer to 0.
            for (int x = 0; x < buffer.Length; x++)
            {
                buffer[x] = 0;
            }
        }

        public static Tuple<string, string> GenerateRsaKeyPairStandalone(out string error)
        {
            Tuple<string, string> keyPair;
            try
            {
                Chilkat.Rsa rsa = new Chilkat.Rsa();

                // Generate a 1024-bit RSA key pair.
                if (!rsa.GenerateKey(1024))
                    throw new Exception(rsa.LastErrorText);

                keyPair = new Tuple<string, string>(item1: rsa.ExportPrivateKey(), item2: rsa.ExportPublicKey());

                error = null;
                return keyPair;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return new Tuple<string, string>(null, null);
            }
        }

        public static byte[] GenerateKeyWithSaltFromString(string strData, out string error)
        {
            byte[] pwd = Encoding.Unicode.GetBytes(strData);
            byte[] salt = CreateRandomSalt(length: 7);

            byte[] result = null;

            // Create a TripleDESCryptoServiceProvider object.
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

            try
            {
                // Create a PasswordDeriveBytes object and then create
                // a TripleDES key from the password and salt.
                PasswordDeriveBytes pdb = new PasswordDeriveBytes(pwd, salt);

                // Create the key and set it to the Key property
                // of the TripleDESCryptoServiceProvider object.
                tdes.Key = pdb.CryptDeriveKey("TripleDES", "SHA1", 192, tdes.IV);
                result = (byte[])tdes.Key.Clone();

            }
            catch (Exception e)
            {
                error = e.Message;
                return result;
            }
            finally
            {
                // Clear the buffers
                ClearBytes(pwd);
                ClearBytes(salt);

                // Clear the key.
                tdes.Clear();
            }
            error = null;
            return result;
        }

        public static bool IsValidSignature(byte[] signature, string data, out string error)
        {
            try
            {
                MemoryStream msDecrypt = new MemoryStream(signature);

                // Create a CryptoStream using the MemoryStream
                // and the passed key and initialization vector (IV).
                CryptoStream csDecrypt = new CryptoStream(msDecrypt,
                    new TripleDESCryptoServiceProvider().CreateDecryptor(signature, SymmetricAlgorithm.Create("TripleDes").IV),
                    CryptoStreamMode.Read);

                // Create buffer to hold the decrypted data.
                byte[] fromEncrypt = new byte[signature.Length];

                // Read the decrypted data out of the crypto stream
                // and place it into the temporary buffer.
                csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

                //Convert the buffer into a string and return it.
                var result = new ASCIIEncoding().GetString(fromEncrypt);
                if (result != data)
                {
                    error = $"Invalid signature for {data}";
                    return false;
                }

                error = null;
                return true;
            }
            catch (CryptographicException e)
            {
                error = $"A Cryptographic error occurred: {e.Message}";
                return false;
            }

        }

        public static string ToString(this byte[] bytes)
        {
            string response = string.Empty;

            foreach (byte b in bytes)
                response += (Char)b;

            return response;
        }
    }
}
