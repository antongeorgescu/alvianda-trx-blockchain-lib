using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Web.Helpers;
using Newtonsoft;
using Newtonsoft.Json;
using System.Linq;

namespace Trx_Blockchain_Lib
{
    public class Block
    {
        /**
         * @param {number} timestamp
         * @param {Transaction[]} transactions
         * @param {string} previousHash
         */

        public long index { get; set; }
        public DateTime timestamp { get; set; }
        public List<Transaction> transactions { get; set; }
        public string previousHash { get; set; }
        public string hash { get; set; }
        public long nonce {get;set;}

        public Block()
        {
            this.previousHash = string.Empty;
            this.timestamp = DateTime.Now;
            this.transactions = null;
            this.hash = string.Empty;
        }

        public Block(DateTime timestamp, List<Transaction> transactions, string previousHash = "")
        {
            this.previousHash = previousHash;
            this.timestamp = timestamp;
            this.transactions = transactions;
            this.hash = "";
        }

        /**
         * Returns the SHA256 of this block (by processing all the data stored
         * inside this block)
         *
         * @returns {string}
         */
        public string CalculateHash()
        {
            string inputStr = this.previousHash + this.timestamp.ToString() + JsonConvert.SerializeObject(this.transactions, Formatting.Indented) + this.nonce;
            return ChainHelper.CalculateHash(inputStr);
        }

        /**
         * Validates all the transactions inside this block (signature + hash) and
         * returns true if everything checks out. False if the block is invalid.
         *
         * @returns {boolean}
         */
        public bool HasValidTransactions()
        {
            foreach (var trx in transactions) {
                if (!trx.IsValid())
                {
                    return false;
                }
            }

            return true;
        }

        
    }
}
