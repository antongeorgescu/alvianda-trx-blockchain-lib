using System;
using Trx_Blockchain_Lib;

namespace ConsoleTrxBlockchain
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Loan Star Blockchain!");

            var trxPool = new TransactionPool();
            var trx = new Transaction("Anton-Georgescu", "Loan003.Repayment.Pay", 200);
            trxPool.AddRaw(trx);
            trx = new Transaction("Alex-Georgescu", "Loan002.Repayment.Pay", 214);
            trxPool.AddRaw(trx);
            trx = new Transaction("Loan002.Repayment.Credit", "Alex-Georgescu",14);
            trxPool.AddRaw(trx);

            var blockchain = new Blockchain();
            var block = new Block(DateTime.Now, trxPool.TakeAll(), blockchain.GetLatestBlock().hash);
            blockchain.AddBlock(block);

            var trxPool2 = new TransactionPool();
            trx = new Transaction("Jake-Trajanovich", "Loan002.Repayment.Pay", 80);
            trxPool.AddRaw(trx);
            trx = new Transaction("Cora-Trajanovich", "Loan002.Repayment.Pay", 73);
            trxPool2.AddRaw(trx);

            block = new Block(DateTime.Now, trxPool2.TakeAll(), blockchain.GetLatestBlock().hash);
            blockchain.AddBlock(block);

            //BlockMiner miner = new BlockMiner(blockchain);
            //miner.DoGenerateBlock(blockchain);

            var trxs = blockchain.GetAllTransactionsForAccount("Jake-Trajanovich");
            Console.WriteLine($"from:{trxs[0].fromAccount},to:{trxs[0].toAccount},amount:{trxs[0].amount}");
            Console.ReadKey();
        }
    }
}
