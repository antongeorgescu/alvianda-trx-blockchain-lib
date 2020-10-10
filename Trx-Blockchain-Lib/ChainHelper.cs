using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace Trx_Blockchain_Lib
{
    public static class ChainHelper
    {
        public static int MINING_PERIOD = 5000;
        public static int NUMBER_ZEROES = 4;

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

        public static string CalculateHash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string CalculateHash2(string inputStr)
        {
            SHA256 sha256 = SHA256.Create();

            byte[] inputBytes = Encoding.ASCII.GetBytes(inputStr);
            byte[] outputBytes = sha256.ComputeHash(inputBytes);

            return Convert.ToBase64String(outputBytes);
        }

        public static string CalculateHash3(string data)
        {
            SHA256 sha256Hash = SHA256.Create();
            string hash = ChainHelper.GetHash(sha256Hash, data);
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

        public static List<string> BlockchainExplorer(Blockchain blockChain)
        {
            var result = new List<string>();

            foreach (var block in blockChain.blocks)
            {
                result.Add($"Block index:{block.index},nonce:{block.nonce},hash:{block.hash},create_on:{block.timestamp}");
                foreach (var trx in block.transactions)
                    result.Add($"...Transaction create_on:{trx.timestamp},from_account:{trx.fromAccount},to_account:{trx.toAccount},amount:{trx.amount},signature:{BitConverter.ToString(trx.signature)}");
            }

            return result;
        }

        public static string FindMerkleRootHash(List<Transaction> transactionList)
        {
            var transactionStrList = transactionList.Select(trx => CalculateHash(trx.CalculateHash())).ToList();
            return BuildMerkleRootHash(transactionStrList);
        }

        private static string BuildMerkleRootHash(IList<string> merkelLeaves)
        {
            if (merkelLeaves == null || !merkelLeaves.Any())
                return string.Empty;

            if (merkelLeaves.Count() == 1)
                return merkelLeaves.First();

            if (merkelLeaves.Count() % 2 > 0)
                merkelLeaves.Add(merkelLeaves.Last());

            var merkleBranches = new List<string>();

            for (int i = 0; i < merkelLeaves.Count(); i += 2)
            {
                var leafPair = string.Concat(merkelLeaves[i], merkelLeaves[i + 1]);
                merkleBranches.Add(CalculateHash(CalculateHash(leafPair)));
            }
            return BuildMerkleRootHash(merkleBranches);
        }
    }
}
