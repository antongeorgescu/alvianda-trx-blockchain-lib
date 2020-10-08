using System;
using System.Collections.Generic;
using Trx_Blockchain_Lib;

namespace ConsoleTrxBlockchain
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Loan Star Blockchain!");

            Console.WriteLine("*********************************************************************************************************************");

            var blockchain = new Blockchain();
            BlockMiner miner = new BlockMiner(blockchain);

            string error;

            var trxList1 = new List<Tuple<string,string,double>>();
            trxList1.Add(new Tuple<string, string, double>("Anton-Georgescu","Loan003.Repayment.Pay",200));
            trxList1.Add(new Tuple<string, string, double>("Alex-Georgescu", "Loan002.Repayment.Pay", 214));
            trxList1.Add(new Tuple<string, string, double>("Loan002.Repayment.Credit", "Alex-Georgescu", 200));

            Console.WriteLine($"adding {trxList1.Count} raw transactions...");

            miner.TransactionPool = AddTransactionsToPool(trxList1);
            Console.WriteLine($"{DateTime.Now}: start blockchained mining...");
            miner.DoGenerateBlockTest(blockchain);
            Console.WriteLine($"{DateTime.Now}: end blockchained mining.");
            Console.WriteLine($"blockchain is valid:{blockchain.IsChainValid(out error)}");

            Console.WriteLine("*********************************************************************************************************************");

            var trxList2 = new List<Tuple<string, string, double>>();
            trxList2.Add(new Tuple<string, string, double>("Jake-Trajanovich", "Loan002.Repayment.Pay", 200));
            trxList2.Add(new Tuple<string, string, double>("Cora-Trajanovich", "Loan003.Repayment.Pay", 214));
            trxList2.Add(new Tuple<string, string, double>("Alex-Georgescu", "Loan002.Repayment.Pay", 72));

            Console.WriteLine($"adding {trxList2.Count} raw transactions...");

            miner.TransactionPool = AddTransactionsToPool(trxList2);
            Console.WriteLine($"{DateTime.Now}: start blockchained mining...");
            miner.DoGenerateBlockTest(blockchain);
            Console.WriteLine($"{DateTime.Now}: end blockchained mining.");
            Console.WriteLine($"blockchain is valid:{blockchain.IsChainValid(out error)}");

            Console.WriteLine("*********************************************************************************************************************");

            string testAccount = "Alex-Georgescu";
            Console.WriteLine($"list existing transactions for:{testAccount}");

            var trxs = blockchain.GetAllTransactionsForAccount(testAccount);
            foreach (var tx in trxs)
                Console.WriteLine($"timestamp:{tx.timestamp},from:{tx.fromAccount},to:{tx.toAccount},amount:{tx.amount},isvalid:{tx.IsValid()}");

            Console.WriteLine("*********************************************************************************************************************");

            Console.WriteLine($"tampering with blockchain transaction...");
            Console.WriteLine($"[original] timestamp:{blockchain.chain[2].transactions[0].timestamp},from:{blockchain.chain[2].transactions[0].fromAccount},to:{blockchain.chain[2].transactions[0].toAccount},amount:{blockchain.chain[2].transactions[0].amount},isvalid:{blockchain.chain[2].transactions[0].IsValid()}");
            blockchain.chain[2].transactions[0].amount = 50;
            Console.WriteLine($"[tampered] timestamp:{blockchain.chain[2].transactions[0].timestamp},from:{blockchain.chain[2].transactions[0].fromAccount},to:{blockchain.chain[2].transactions[0].toAccount},amount:{blockchain.chain[2].transactions[0].amount},isvalid:{blockchain.chain[2].transactions[0].IsValid()}");
            var isChainValid = blockchain.IsChainValid(out error);
            Console.WriteLine($"blockchain is valid:{isChainValid} {error}");

            Console.WriteLine("*********************************************************************************************************************");

            Console.WriteLine($"trying to revert transaction to initial value...");
            Console.WriteLine($"[original] timestamp:{blockchain.chain[2].transactions[0].timestamp},from:{blockchain.chain[2].transactions[0].fromAccount},to:{blockchain.chain[2].transactions[0].toAccount},amount:{blockchain.chain[2].transactions[0].amount},isvalid:{blockchain.chain[2].transactions[0].IsValid()}");
            blockchain.chain[2].transactions[0].amount = 200;
            Console.WriteLine($"[tampered] timestamp:{blockchain.chain[2].transactions[0].timestamp},from:{blockchain.chain[2].transactions[0].fromAccount},to:{blockchain.chain[2].transactions[0].toAccount},amount:{blockchain.chain[2].transactions[0].amount},isvalid:{blockchain.chain[2].transactions[0].IsValid()}");
            isChainValid = blockchain.IsChainValid(out error);
            Console.WriteLine($"blockchain is valid:{isChainValid} {error}");

            Console.WriteLine("*********************************************************************************************************************");

            Console.ReadKey();
        }

        static TransactionPool AddTransactionsToPool(List<Tuple<string,string,double>> listTrx)
        {
            string error;

            var trxPool = new TransactionPool();

            foreach (var entry in listTrx)
            {
                var trans = new Transaction(entry.Item1, entry.Item2, entry.Item3);
                if (!trans.SignTransaction(entry.Item1, out error))
                    Console.WriteLine($"invalid transaction - from:{entry.Item1},to:{entry.Item2},amount{entry.Item3}");
                trxPool.AddRaw(trans);
            }

            return trxPool;
        }
    }
}
