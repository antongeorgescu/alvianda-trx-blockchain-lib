using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Cryptography;

namespace Trx_Blockchain_Lib
{
    public class BlockMiner
    {
        private static int MINING_PERIOD = 5000;
        //private TransactionPool TransactionPool { get => DependencyManager.TransactionPool; }
        public TransactionPool TransactionPool { get; set; }
        private Blockchain blockchain;
        private System.Threading.CancellationTokenSource cancellationToken;

        public BlockMiner(Blockchain blockchain)
        {
            blockchain = blockchain;
        }

        public void Start()
        {
            cancellationToken = new CancellationTokenSource();
            Task.Run(() => DoGenerateBlock(blockchain), cancellationToken.Token);
            Console.WriteLine("Mining has started");
        }
        public void Stop()
        {
            cancellationToken.Cancel();
            Console.WriteLine("Mining has stopped");
        }

        public void DoGenerateBlock(Blockchain blockchain)
        {
            while (true)
            {
                var startTime = DateTime.Now.Millisecond;
                GenerateBlock(blockchain);
                var endTime = DateTime.Now.Millisecond;
                var remainTime = MINING_PERIOD - (endTime - startTime);
                Thread.Sleep(remainTime < 0 ? 0 : remainTime);
            }
        }

        public void DoGenerateBlockTest(Blockchain blockchain)
        {
            var startTime = DateTime.Now.Millisecond;
            GenerateBlock(blockchain);
            var endTime = DateTime.Now.Millisecond;
            var remainTime = MINING_PERIOD - (endTime - startTime);
            Thread.Sleep(remainTime < 0 ? 0 : remainTime);
        }

        public void GenerateBlock(Blockchain blockchain)
        {
            var lastBlock = blockchain.GetLatestBlock();
            var block = new Block()
            {
                timestamp = DateTime.Now,
                nonce = 0,
                transactions = TransactionPool.TakeAllRaw(),
                index = (lastBlock?.index + 1 ?? 0),
                previousHash = lastBlock?.hash ?? string.Empty
            };
            MineBlock(block);
            blockchain.AddBlock(block);
        }

        private void MineBlock(Block block)
        {
            if (block.transactions == null)
                return;

            var merkleRootHash = FindMerkleRootHash(block.transactions);
            long nonce = -1;
            var hash = string.Empty;
            do
            {
                nonce++;
                var rowData = block.index + block.previousHash + block.timestamp.ToString() + nonce + merkleRootHash;
                hash = CalculateHash(CalculateHash(rowData));
            }
            while (!hash.StartsWith("0000"));
            block.hash = hash;
            block.nonce = nonce;
        }

        private string FindMerkleRootHash(List<Transaction> transactionList)
        {
            var transactionStrList = transactionList.Select(trx => CalculateHash(trx.CalculateHash())).ToList();
            return BuildMerkleRootHash(transactionStrList);
        }

        private string BuildMerkleRootHash(IList<string> merkelLeaves)
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
    }
}
