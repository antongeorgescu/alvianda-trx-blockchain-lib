using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.CompilerServices;

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

        public static Tuple<byte[],byte[],byte[]> GenerateKeyFromString(string strData, out string error)
        {
            try
            {
                // Create a TripleDESCryptoServiceProvider object.
                TripleDESCryptoServiceProvider tDESalg = new TripleDESCryptoServiceProvider();

                // Encrypt the string to an in-memory buffer.
                byte[] encryptData = EncryptTextToMemory(strData, tDESalg.Key, tDESalg.IV);

                error = null;
                return new Tuple<byte[], byte[], byte[]>(encryptData, tDESalg.Key, tDESalg.IV);

            }
            catch (Exception e)
            {
                error = e.Message;
                return null;
            }
        }

        public static bool IsValidSignature(byte[] signature, byte[] key, byte[] initVector, string origData,out string error)
        {
            // References:
            // https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.tripledescryptoserviceprovider?view=netcore-3.1
            
            try
            {
                // Decrypt the buffer back to a string.
                string decryptData = DecryptTextFromMemory(signature, key, initVector).Replace("\0","");

                if (decryptData != origData)
                {
                    error = "Invalid signature";
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

        public static byte[] EncryptTextToMemory(string Data, byte[] Key, byte[] IV)
        {
            try
            {
                // Create a MemoryStream.
                MemoryStream mStream = new MemoryStream();

                // Create a CryptoStream using the MemoryStream
                // and the passed key and initialization vector (IV).
                CryptoStream cStream = new CryptoStream(mStream,
                    new TripleDESCryptoServiceProvider().CreateEncryptor(Key, IV),
                    CryptoStreamMode.Write);

                // Convert the passed string to a byte array.
                byte[] toEncrypt = new ASCIIEncoding().GetBytes(Data);

                // Write the byte array to the crypto stream and flush it.
                cStream.Write(toEncrypt, 0, toEncrypt.Length);
                cStream.FlushFinalBlock();

                // Get an array of bytes from the
                // MemoryStream that holds the
                // encrypted data.
                byte[] ret = mStream.ToArray();

                // Close the streams.
                cStream.Close();
                mStream.Close();

                // Return the encrypted buffer.
                return ret;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }
        }

        public static string DecryptTextFromMemory(byte[] Data, byte[] Key, byte[] IV)
        {
            try
            {
                // Create a new MemoryStream using the passed
                // array of encrypted data.
                MemoryStream msDecrypt = new MemoryStream(Data);

                // Create a CryptoStream using the MemoryStream
                // and the passed key and initialization vector (IV).
                CryptoStream csDecrypt = new CryptoStream(msDecrypt,
                    new TripleDESCryptoServiceProvider().CreateDecryptor(Key, IV),
                    CryptoStreamMode.Read);

                // Create buffer to hold the decrypted data.
                byte[] fromEncrypt = new byte[Data.Length];

                // Read the decrypted data out of the crypto stream
                // and place it into the temporary buffer.
                csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

                //Convert the buffer into a string and return it.
                return new ASCIIEncoding().GetString(fromEncrypt);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }
        }
    }
}
