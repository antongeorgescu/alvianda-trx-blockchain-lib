using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.WebPages;

namespace Trx_Blockchain_Lib
{
    public class Blockchain
    {
        public List<Block> chain = new List<Block>();
        List<Transaction> pendingTransactions; 
    
        public Blockchain()
        {
            var block = createGenesisBlock();

            block.index = 0;
            block.previousHash = "";
            block.hash = block.CalculateHash();
            chain.Add(block);
        }

        /**
         * @returns {Block}
         */
        Block createGenesisBlock()
        {
            return new Block(DateTime.Now,new List<Transaction>(), "");
        }

        /**
         * Returns the latest block on our chain. Useful when you want to create a
         * new Block and you need the hash of the previous Block.
         *
         * @returns {Block[]}
         */
        public Block GetLatestBlock()
        {
            return chain.LastOrDefault<Block>();
        }

        public void AddBlock(Block block)
        {
            Block latestBlock = GetLatestBlock();
            block.index = latestBlock.index + 1;
            block.previousHash = latestBlock.hash;
            block.hash = block.CalculateHash();
            chain.Add(block);
        }

        /**
         * Add a new transaction to the list of pending transactions (to be added
         * next time the mining process starts). This verifies that the given
         * transaction is properly signed.
         *
         * @param {Transaction} transaction
         */
        public bool AddTransaction(Transaction transaction, out string error)
        {
            try
            {
                if (transaction.fromAccount.IsEmpty() || transaction.toAccount.IsEmpty())
                {
                    throw new Exception("Transaction must include from and to address");
                }

                // Verify the transactiion
                if (!transaction.IsValid())
                {
                    throw new Exception("Cannot add invalid transaction to chain");
                }

                if (transaction.amount <= 0.0)
                {
                    throw new Exception("Transaction amount should be higher than 0");
                }

                // Making sure that the amount sent is not greater than existing balance
                if (this.GetBalanceOfAccount(transaction.fromAccount) < transaction.amount)
                {
                    throw new Exception("Not enough balance");
                }

                this.pendingTransactions.Append(transaction);
                
                error = null;
                return true;
            }
            catch(Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        /**
         * Returns the balance of a given wallet address.
         *
         * @param {string} address
         * @returns {number} The balance of the wallet
         */
        public double GetBalanceOfAccount(string account)
        {
            double balance = 0;

            foreach (var block in chain) {
                foreach (var trx in block.transactions) {
                    if (trx.fromAccount == account)
                    {
                        balance -= trx.amount;
                    }

                    if (trx.toAccount == account)
                    {
                        balance += trx.amount;
                    }
                }
            }
            return balance;
        }

        /**
         * Returns a list of all transactions that happened
         * to and from the given wallet address.
         *
         * @param  {string} address
         * @return {Transaction[]}
         */
        public List<Transaction> GetAllTransactionsForAccount(string account)
        {
            List<Transaction> txs = new List<Transaction>();

            foreach (var block in chain) {
                foreach (var trx in block.transactions) {
                    if (trx.fromAccount == account || trx.toAccount == account)
                    {
                        txs.Add(trx);
                    }
                }
            }

            return txs;
        }

        public List<Transaction> ReadAllPendingTransactions()
        {
            return pendingTransactions;
        }

        /**
         * Loops over all the blocks in the chain and verify if they are properly
         * linked together and nobody has tampered with the hashes. By checking
         * the blocks it also verifies the (signed) transactions inside of them.
         *
         * @returns {boolean}
         */
        public bool IsChainValid()
        {
            // Check if the Genesis block hasn't been tampered with by comparing
            // the output of createGenesisBlock with the first block on our chain
            //var realGenesis = JsonConvert.SerializeObject(this.createGenesisBlock(), Formatting.Indented);

            //if (realGenesis != JsonConvert.SerializeObject(this.chain[0], Formatting.Indented))
            //{
            //    return false;
            //}

            // Check the remaining blocks on the chain to see if there hashes and
            // signatures are correct
            for (int i=0; i < this.chain.Count; i++)
            {
                var currentBlock = this.chain[i];

                if (!currentBlock.HasValidTransactions())
                {
                    return false;
                }

                if (currentBlock.hash != currentBlock.CalculateHash())
                {
                    return false;
                }
            }

            return true;
        }
    }
}
