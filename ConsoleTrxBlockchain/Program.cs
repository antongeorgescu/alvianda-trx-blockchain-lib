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
            Console.WriteLine("+++ Step 1: Read blockchain difficulty of work parameters");
            Console.ReadKey();
            Console.WriteLine($"Average mining time (msec):{ChainHelper.MINING_PERIOD}, Difficulty of work (#leading zeroes):{ChainHelper.NUMBER_ZEROES}");
            Console.ReadKey();

            Console.WriteLine("*********************************************************************************************************************");
            Console.WriteLine("+++ Step 2: Add first set of transactions to blockchain");
            Console.ReadKey();

            var blockchain = new Blockchain();
            BlockMiner miner = new BlockMiner(blockchain);

            string error;

            var trxList1 = new List<Tuple<string,string,double>>();
            trxList1.Add(new Tuple<string, string, double>("Anton-Georgescu","Loan003.Repayment.Pay",200));
            trxList1.Add(new Tuple<string, string, double>("Alex-Georgescu", "Loan002.Repayment.Pay", 214));
            trxList1.Add(new Tuple<string, string, double>("Loan002.Repayment.Credit", "Alex-Georgescu", 200));

            Console.WriteLine($"adding {trxList1.Count} raw transactions...");
            foreach (var trx in trxList1)
                Console.WriteLine($"...[Transaction] from_account:{trx.Item1},to_account:{trx.Item2},amount:{trx.Item3} ");

            Console.WriteLine("*********************************************************************************************************************");
            Console.WriteLine("+++ Step 3: Run blockchain mining process");
            Console.ReadKey();
            miner.TransactionPool = AddTransactionsToPool(trxList1);
            Console.WriteLine($"{DateTime.Now}: start blockchained mining...");
            
            miner.DoGenerateBlockTest(blockchain);
            Console.WriteLine($"{DateTime.Now}: end blockchained mining.");
            Console.WriteLine($"blockchain is valid:{blockchain.IsChainValid(out error)}");

            Console.WriteLine("*********************************************************************************************************************");
            Console.WriteLine("+++ Step 4: Add next set of transactions to blockchain");
            Console.ReadKey();

            var trxList2 = new List<Tuple<string, string, double>>();
            trxList2.Add(new Tuple<string, string, double>("Jake-Trajanovich", "Loan002.Repayment.Pay", 200));
            trxList2.Add(new Tuple<string, string, double>("Cora-Trajanovich", "Loan003.Repayment.Pay", 214));
            trxList2.Add(new Tuple<string, string, double>("Alex-Georgescu", "Loan002.Repayment.Pay", 72));

            Console.WriteLine($"adding {trxList2.Count} raw transactions...");
            foreach (var trx in trxList1)
                Console.WriteLine($"...[Transaction] from_account:{trx.Item1},to_account:{trx.Item2},amount:{trx.Item3} ");

            Console.WriteLine("*********************************************************************************************************************");
            Console.WriteLine("+++ Step 5: Run blockchain mining process");
            Console.ReadKey();
            miner.TransactionPool = AddTransactionsToPool(trxList2);
            Console.WriteLine($"{DateTime.Now}: start blockchained mining...");
            miner.DoGenerateBlockTest(blockchain);
            Console.WriteLine($"{DateTime.Now}: end blockchained mining.");
            Console.WriteLine($"blockchain is valid:{blockchain.IsChainValid(out error)}");

            string testAccount = "Alex-Georgescu";

            Console.WriteLine("*********************************************************************************************************************");
            Console.WriteLine($"+++ Step 6: List existing transactions for {testAccount}");
            Console.ReadKey();

            Console.WriteLine($"list existing transactions for:{testAccount}");

            var trxs = blockchain.GetAllTransactionsForAccount(testAccount);
            foreach (var tx in trxs)
                Console.WriteLine($"timestamp:{tx.timestamp},from:{tx.fromAccount},to:{tx.toAccount},amount:{tx.amount},isvalid:{tx.IsValid()}");

            Console.WriteLine("*********************************************************************************************************************");
            Console.WriteLine($"+++ Step 7: Tamper with an existing transaction");
            Console.ReadKey();

            Console.WriteLine("*********************************************************************************************************************");

            Console.WriteLine($"tampering with blockchain transaction...");
            Console.WriteLine($"[original] timestamp:{blockchain.blocks[2].transactions[0].timestamp},from:{blockchain.blocks[2].transactions[0].fromAccount},to:{blockchain.blocks[2].transactions[0].toAccount},amount:{blockchain.blocks[2].transactions[0].amount},isvalid:{blockchain.blocks[2].transactions[0].IsValid()}");
            blockchain.blocks[2].transactions[0].amount = 50;
            Console.WriteLine($"[tampered] timestamp:{blockchain.blocks[2].transactions[0].timestamp},from:{blockchain.blocks[2].transactions[0].fromAccount},to:{blockchain.blocks[2].transactions[0].toAccount},amount:{blockchain.blocks[2].transactions[0].amount},isvalid:{blockchain.blocks[2].transactions[0].IsValid()}");
            var isChainValid = blockchain.IsChainValid(out error);
            Console.WriteLine($"blockchain is valid:{isChainValid} {error}");

            Console.WriteLine($"trying to revert transaction to initial value...");
            Console.WriteLine($"[original] timestamp:{blockchain.blocks[2].transactions[0].timestamp},from:{blockchain.blocks[2].transactions[0].fromAccount},to:{blockchain.blocks[2].transactions[0].toAccount},amount:{blockchain.blocks[2].transactions[0].amount},isvalid:{blockchain.blocks[2].transactions[0].IsValid()}");
            blockchain.blocks[2].transactions[0].amount = 200;
            Console.WriteLine($"[tampered] timestamp:{blockchain.blocks[2].transactions[0].timestamp},from:{blockchain.blocks[2].transactions[0].fromAccount},to:{blockchain.blocks[2].transactions[0].toAccount},amount:{blockchain.blocks[2].transactions[0].amount},isvalid:{blockchain.blocks[2].transactions[0].IsValid()}");
            isChainValid = blockchain.IsChainValid(out error);
            Console.WriteLine($"blockchain is valid:{isChainValid} {error}");

            Console.WriteLine("*********************************************************************************************************************");
            Console.WriteLine($"+++ Step 8: Run Blockchain Explorer");
            Console.ReadKey();

            var chainExplore = ChainHelper.BlockchainExplorer(blockchain);
            foreach (var line in chainExplore)
                Console.WriteLine(line);

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
