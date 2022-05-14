# Arweave.NET

A simple implementation or Arweave protocol for the Microsoft.NET platform. Currently supported .NET 5, .NET 6 in the upcoming plans. 

Implemented most of methods described in official documentation (https://docs.arweave.org/developers/server/http-api), supporting only v2 format to upload files. 

Current version doesn't allow delayed uploads but can be used for the most of data types for simple transactions. 

#### Example usage to submit a transaction of a different type: 

```c#

// keyfilePath - path to your keyfile, filePath - path to uploading file, toAddress - receipient wallet address

// Simple file upload
var dataTransaction = Transaction.CreateDataTransaction(filePath, keyfilePath);

//Wallet-to-wallet transfer without data
var W2WTransaction = Transaction.CreateW2WTransaction(keyFilePath,"10000000",toAddress);

// Wallet-to-wallet tranfer with data
var W2WDataTransaction = Transaction.CreateW2WTransactionWithData(filePath,keyFilePath,"10000000",toAddress);

var result = transactionService.SubmitTransaction(dataTransaction).Result;
if(result.Error == null)
{
    Console.WriteLine(result.Id);
}
else
{
    Console.WriteLine(result.Error.Message);
}
```

There are also a bunch of methods to get necessary API-related data, examples can be found inside the TestConsole application. 

_All the transactions by default using the recommended AR price inside the Reward field, meanwhile you can set up your own cost by passing the parameter value manually_
