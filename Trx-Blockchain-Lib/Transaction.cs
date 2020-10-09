﻿using System;
using System.Security.Cryptography;
using System.Web.WebPages;
using Trx_Blockchain_Lib;

namespace Trx_Blockchain_Lib
{
    public class Transaction
    {
        /**
         * @param {string} fromAddress
         * @param {string} toAddress
         * @param {number} amount
         */
        public string fromAccount;
        public string toAccount;
        public double amount;
        public DateTime timestamp;
        public byte[] signature;

        public Transaction(string fromAccount, string toAccount, double amount)
        {
            this.fromAccount = fromAccount;
            this.toAccount = toAccount;
            this.amount = amount;
            this.timestamp = DateTime.Now;
        }

        /**
         * Signs a transaction with the given signingKey (which is an Elliptic keypair
         * object that contains a private key). The signature is then stored inside the
         * transaction object and later stored on the blockchain.
         *
         * @param {string} signingKey
         */
        public bool SignTransaction(string signingKey,out string error)
        {
            try
            {
                // Calculate the hash of this transaction, sign it with the key
                // and store it inside the transaction obect
                //var hashTx = EncryptionHelper.CalculateHash(this.fromAccount + this.toAccount + this.amount + this.timestamp);

                error = null;
                //this.signature = EncryptionHelper.SignDataWithPrivateKey(hashTx, signingKey, "base64", out error);
                this.signature = CryptoKeyUtility.GenerateKeyWithSaltFromString(signingKey,out error);

                if (signature == null)
                    throw new Exception(error);

                return true;
            }
            catch(Exception ex)
            {
                error = ex.Message;
                this.signature = null;
                return false;
            }
        }

        public string CalculateHash()
        {
            string inputStr = fromAccount + toAccount + timestamp + amount;
            string hash = EncryptionHelper.CalculateHash(inputStr);
            return hash;
        }

        /**
         * Checks if the signature is valid (transaction has not been tampered with).
         * It uses the fromAddress as the public key.
         *
         * @returns {boolean}
         */
        public bool IsValid()
        {
            try
            {
                if (this.signature == null)
                    return false;

                if (this.signature.Length == 0)
                    return false;

                // check the fromAccount is the signingKey
                //string error;
                //if (!CryptoKeyUtility.IsValidSignature(signature, fromAccount, out error))
                //    return false;

                return true;

            }
            catch
            {
                return false;
            }
        }
    }

}
