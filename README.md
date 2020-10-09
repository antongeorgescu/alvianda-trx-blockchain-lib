# Blockchain Transaction Ledger with .NET/C# 

## Introduction
 
**Blockchain** is a chain of blocks which could be simply assumed as an immutable data structure.
Immutability leads us to build trust in completely unreliable environments and is achieved by using Cryptographic hashing algorithms 
which prevent data from being changed or deleted after data is added to the Blockchain.
Two different hashes are stored in every block, one hash is calculated for the current block and the other is for the previous one.
Any change in a block invalidates every block after it, and that creates a powerful mechanism against tampering with the records.

## TrxBlockchainLib Library
TrxBlockchainLib is a .NET Core library that implements a Financial Transaction Ledger with Blockchain pattern. 
This library implements a Transaction Blockchain, with payer/receiver accounts
The code map (see diagram below), shows the classes identity and association, in order to provide full Blockchain-compliant functionality

There are a few main actors (classes):
**BlockMiner** class instance stays into an infinite loop:
* reads Transactions pool (where raw transactions are stored)
* creates blocks with the raw transactions, after which empties the pools

**Transaction** class contains:
* account properties
* encrypted signature properties

**TransactionPool** class contains: 
* list of raw (unprocessed) transactions
* these transactions are attached to a block during the mining phase
* once they are attached, the pool of transactions is cleared

**Block** class contains:
* list of processed, signed transactions
* these transactions are immutable

**CryptoKeyUtility** and **EncryptionHelper** classes provide: 
* methods for hashing and encryption/decryption strings

The blockchain can be persisted into a data store (database) once an ORM/db pair of components will be added to the design


## Component Diagram (Code Map)

![TrxBlockchainLib-CodeMap](https://user-images.githubusercontent.com/6631390/95639120-4d501280-0a65-11eb-8c5e-77a2fdacecbc.png)


