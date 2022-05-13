using Arweave.NET.Services;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;

namespace Arweave.NET.Models
{
    public class Transaction : BaseModel
    {
        [JsonPropertyName("format")]
        public int Format { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("last_tx")]
        public string LastTx { get; set; }
        [JsonPropertyName("owner")]
        public string Owner { get; set; }
        [JsonPropertyName("tags")]
        public List<Tag> Tags { get; set; }
        [JsonPropertyName("target")]
        public string Target { get; set; }
        [JsonPropertyName("quantity")]
        public string Quantity { get; set; }
        [JsonPropertyName("data_root")]
        public string DataRoot { get; set; }
        [JsonPropertyName("data")]
        public string Data { get; set; }
        [JsonPropertyName("data_size")]
        public string DataSize { get; set; }
        [JsonPropertyName("reward")]
        public string Reward { get; set; }
        [JsonPropertyName("signature")]
        public string Signature { get; set; }
        [JsonIgnore]
        public JsonWebKey JWK {get;set;}
        public Transaction()
        {
            Format = 2;
            Tags = new List<Tag>();
        }

        public void LoadOwner(string keyFilePath)
        {
            string jwksString = string.Empty;
            using (StreamReader sr = File.OpenText(keyFilePath))
            {
                jwksString = sr.ReadToEnd();
                var formattedString = "{ \"keys\":[" + jwksString + "]}";
                var jwks = new JsonWebKeySet(formattedString);
                if (jwks.Keys.Count > 1)
                    throw new NotImplementedException("Key file has more then 1 key, please check");
                Owner = jwks.Keys[0].N;
                JWK = jwks.Keys[0];
            }
        }

        public Transaction(string keyFilePath):this()
        {
            Format = 2;
            Tags = new List<Tag>();
            LoadOwner(keyFilePath);
        }
      

        public void AddTag(string name, string value)
        {
            var tag = new Tag
            {
                Name = Utils.Base64Encode(Encoding.UTF8.GetBytes(name)),
                Value = Utils.Base64Encode(Encoding.UTF8.GetBytes(value))
            };
            Tags.Add(tag);
        }

        public static Transaction CreateDataTransaction(string dataPath, string keyFilePath, string reward="", bool typeFromPath = true, string contentType = null)
        {
            var transaction = new Transaction(keyFilePath);
            var transactionService = new TransactionService();
            var dataBuff = Utils.ReadDataAsync(dataPath).Result;

            if (typeFromPath)
            {
                var format = Utils.GetFileFormat(dataPath);
                if (string.IsNullOrEmpty(format))
                    throw new FormatException("Couldn't resolve format from pre-defined set. Please set up your own content type or leave it blank");
                transaction.AddTag("Content-Type", format);
            }
            else if (!string.IsNullOrEmpty(contentType))
            {
                transaction.AddTag("Content-Type", contentType);
            }

            transaction.Quantity = "0";
            transaction.Target = "";
            transaction.Data = Utils.Base64Encode(dataBuff);
            transaction.DataSize = dataBuff.Length.ToString();
            transaction.DataRoot = Utils.Base64Encode(ChunkOperations.GenerateTransactionChunks(dataBuff));
            transaction.Reward = string.IsNullOrEmpty(reward) ? transactionService.GetPriceAsync(null, dataBuff.LongLength).Result : reward;
            transaction.LastTx = transactionService.GetAnchorAsync().Result;

            SignData(transaction);
            return transaction;
        }


        public static Transaction W2WTransaction(string keyFilePath, string quantity, string target, string reward = "")
        {
            var transaction = new Transaction(keyFilePath);
            var transactionService = new TransactionService();
            transaction.Quantity = quantity;
            transaction.Target = target;
            transaction.Data = "";
            transaction.DataSize = "0";
            transaction.DataRoot = "";
            transaction.Reward = string.IsNullOrEmpty(reward) ? transactionService.GetPriceAsync(target).Result : reward;
            transaction.LastTx = transactionService.GetAnchorAsync().Result;

            SignData(transaction);
            return transaction;
        }

        public static Transaction W2WTransactionWithData(string keyFilePath,
                                                          string dataPath,
                                                          string quantity,
                                                          string target,
                                                          string reward="",
                                                          bool typeFromPath = true,
                                                          string contentType = null)
        {
            var transaction = new Transaction(keyFilePath);
            var transactionService = new TransactionService();
            var dataBuff = Utils.ReadDataAsync(dataPath).Result;

            if (typeFromPath)
            {
                var format = Utils.GetFileFormat(dataPath);
                if (string.IsNullOrEmpty(format))
                    throw new FormatException("Couldn't resolve format from pre-defined set. Please set up your own content type or leave it blank");
                transaction.AddTag("Content-Type", format);
            }
            else if (!string.IsNullOrEmpty(contentType))
            {
                transaction.AddTag("Content-Type", contentType);
            }

            transaction.Quantity = quantity;
            transaction.Target = target;
            transaction.Data = Utils.Base64Encode(dataBuff);
            transaction.DataSize = dataBuff.Length.ToString();
            transaction.DataRoot = Utils.Base64Encode(ChunkOperations.GenerateTransactionChunks(dataBuff));
            transaction.Reward = string.IsNullOrEmpty(reward) ? transactionService.GetPriceAsync(target, dataBuff.LongLength).Result : reward;
            transaction.LastTx = transactionService.GetAnchorAsync().Result;

            SignData(transaction);
            return transaction;
        }

        private static void SignData(Transaction transaction)
        {
            var dataToSign =  SignatureOperations.GetSignature(transaction);
            var calcSign = SignatureOperations.Sign(dataToSign, transaction.JWK);
            transaction.Signature = Utils.Base64Encode(calcSign);
            transaction.Id = Utils.Base64Encode(Encryption.Hash(calcSign, "SHA-256"));
        }

    }
    

}
