# Arweave.NET

A simple implementation or Arweave protocol for Microsoft.NET platform. Currently supported .NET 5, .NET 6 in the upcoming plans. 

Implemented most of methods described in official documentation (https://docs.arweave.org/developers/server/http-api), supporting only v2 format to upload files. 

Current version doesn't allow delayed uploads but can be used for most of data types for simple transactions. 

#### Example usage to upload file: 

```c#
// keyfile.json - is your private wallet key
var transaction = new Transaction("keyfile.json");

//file.png - file path to upload file
var result = transactionService.SubmitTransaction(transaction, "file.png").Result;
if(result.Error == null)
{
    Console.WriteLine(result.Id);
}
```

There are also a bunch of methods to get necessary API-related data, examples can be found inside the TestConsole application. 
