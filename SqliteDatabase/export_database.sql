--
-- File generated with SQLiteStudio v3.2.1 on Mon Oct 19 12:28:11 2020
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: Account
CREATE TABLE Account (
    account_id      INTEGER       PRIMARY KEY,
    account_type_id SMALLINT,
    name            VARCHAR (50),
    description     VARCHAR (150),
    balance         FLOAT (53) 
);


-- Table: AccountType
CREATE TABLE AccountType (
    account_type_id INTEGER       PRIMARY KEY,
    name            VARCHAR (50),
    description     VARCHAR (150) 
);


-- Table: Block
CREATE TABLE Block (
    block_id        INTEGER      PRIMARY KEY,
    timestamp       DATETIME,
    curr_block_hash VARCHAR (50),
    prev_block_hash VARCHAR (50),
    nonce           INTEGER
);


-- Table: Transaction
CREATE TABLE [Transaction] (
    transaction_id  INT         PRIMARY KEY,
    timestamp       DATETIME,
    from_account_id INT         CONSTRAINT fk_trx_from_account_account REFERENCES Account (account_id),
    to_account_id   INT         CONSTRAINT fk_trx_to_account_account REFERENCES Account (account_id),
    amount          FLOAT (53),
    block_id        INT         CONSTRAINT fk_trx_block_block REFERENCES Block (block_id),
    signature       BINARY (50),
    encrypt_key     BINARY (50),
    init_vector     BINARY (50) 
);


COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
