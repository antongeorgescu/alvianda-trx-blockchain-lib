using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Trx_Blockchain_Lib
{
    public static class EncryptionHelper
    {
        public static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public static string CalculateHash(string inputStr)
        {
            SHA256 sha256 = SHA256.Create();

            byte[] inputBytes = Encoding.ASCII.GetBytes(inputStr);
            byte[] outputBytes = sha256.ComputeHash(inputBytes);

            return Convert.ToBase64String(outputBytes);
        }

        public static string CalculateHash2(string data)
        {
            SHA256 sha256Hash = SHA256.Create();
            string hash = EncryptionHelper.GetHash(sha256Hash, data);
            return hash;
            //return crypto.createHash('sha256').update(this.fromAddress + this.toAddress + this.amount + this.timestamp).digest('hex');
        }

        public static string SignDataWithPrivateKey(string strData,
                                    string keyPrivate,
                                    string hashAlgorithm,
                                    out string error)
        {
            try
            {
                Chilkat.Rsa rsa = new Chilkat.Rsa();

                rsa.ImportPrivateKey(keyPrivate);

                // sign a string, and receive the signature in a hex-encoded string.  
                // Therefore, set the encoding mode to "hex":
                rsa.EncodingMode = "hex";

                // It is important to match the byte-ordering.
                // The LittleEndian property may be set to true for little-endian byte ordering, 
                // or false  for big-endian byte ordering.
                // Microsoft apps typically use little-endian, while
                // OpenSSL and other services (such as Amazon CloudFront) use big-endian.
                rsa.LittleEndian = false;

                // Sign the string using the  md5 hash algorithm.
                // Other valid choices are "md2", "sha256", "sha384",
                // "sha512", and "sha-1".
                string hexSignature = rsa.SignStringENC(strData, hashAlgorithm);
                error = string.Empty;
                return hexSignature;
            }
            catch(Exception ex)
            {
                error = ex.Message;
                return null;
            }
        }

        public static bool VerifyData(string data,
                                        string hashAlgorithm,
                                        string hexSignature, 
                                        out string error)
        {
            try
            {
                Chilkat.Rsa rsa = new Chilkat.Rsa();

                var success = rsa.VerifyStringENC(data, hashAlgorithm, hexSignature);
                if (success)
                {
                    error = string.Empty;
                    return true;
                }
                else
                {
                    error = rsa.LastErrorText;
                    return false;
                }
            }
            catch(Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        
    }
}
