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
        
        //private TransactionPool TransactionPool { get => DependencyManager.TransactionPool; }
        public TransactionPool TransactionPool { get; set; }
        private Blockchain blockchain;
        private System.Threading.CancellationTokenSource cancellationToken;

        private string hashStartWith;

        public BlockMiner(Blockchain blockChain)
        {
            blockchain = blockChain;
            hashStartWith = string.Empty;
            for (int i = 0; i < ChainHelper.NUMBER_ZEROES; i++)
                hashStartWith += "0";
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
                var remainTime = ChainHelper.MINING_PERIOD - (endTime - startTime);
                Thread.Sleep(remainTime < 0 ? 0 : remainTime);
            }
        }

        public void DoGenerateBlockTest(Blockchain blockchain)
        {
            var startTime = DateTime.Now.Millisecond;
            GenerateBlock(blockchain);
            var endTime = DateTime.Now.Millisecond;
            var remainTime = ChainHelper.MINING_PERIOD - (endTime - startTime);
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

            var merkleRootHash = ChainHelper.FindMerkleRootHash(block.transactions);
            long nonce = -1;
            var hash = string.Empty;
            do
            {
                nonce++;
                var rowData = block.index + block.previousHash + block.timestamp.ToString() + nonce + merkleRootHash;
                hash = ChainHelper.CalculateHash(ChainHelper.CalculateHash(rowData));
            }
            while (!hash.StartsWith(hashStartWith)) ;
            block.hash = hash;
            block.nonce = nonce;
        }
    }
}
