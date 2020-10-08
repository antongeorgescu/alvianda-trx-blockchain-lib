using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trx_Blockchain_Lib
{
    public class TransactionPool
    {
        private List<Transaction> rawTransactionList;

        private object lockObj;

        public TransactionPool()
        {
            lockObj = new object();
            rawTransactionList = new List<Transaction>();
        }

        public void AddRaw(Transaction transaction)
        {
            lock (lockObj)
            {
                rawTransactionList.Add(transaction);
            }
        }

        public List<Transaction> TakeAllRaw()
        {
            lock (lockObj)
            {
                Transaction[] all = new Transaction[rawTransactionList.Count];
                rawTransactionList.CopyTo(all);
                rawTransactionList.Clear();
                return all.ToList<Transaction>();
            }
        }
    }
}
