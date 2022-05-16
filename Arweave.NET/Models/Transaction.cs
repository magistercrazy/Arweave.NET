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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyFilePath">Path to the wallet private key .json file (exported from Arweave wallet)</param>
        public Transaction(string keyFilePath):this()
        {
            Format = 2;
            Tags = new List<Tag>();
            LoadOwner(keyFilePath);
        }
      
        /// <summary>
        /// Add custom tag with value
        /// </summary>
        /// <param name="name">Tag name</param>
        /// <param name="value">Tag value</param>
        public void AddTag(string name, string value)
        {
            var tag = new Tag
            {
                Name = Utils.Base64Encode(Encoding.UTF8.GetBytes(name)),
                Value = Utils.Base64Encode(Encoding.UTF8.GetBytes(value))
            };
            Tags.Add(tag);
        }

        /// <summary>
        /// Constructs a transaction for simple file upload
        /// </summary>
        /// <param name="dataPath">Path to uploading file</param>
        /// <param name="keyFilePath">Path to the wallet private key .json file (exported from Arweave wallet)</param>
        /// <param name="reward">Custom reward amount</param>
        /// <param name="typeFromPath">Determine content-type by file extension</param>
        /// <param name="contentType">Custom content type value</param>
        /// <returns>Transaction object</returns>
        public static Transaction CreateDataTransaction(string dataPath, string keyFilePath, string reward="", bool typeFromPath = true, string contentType = null)
        {
            var transaction = new Transaction(keyFilePath);
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

            DataTransaction(transaction, dataBuff, reward);
            SignData(transaction);
            return transaction;
        }

        /// <summary>
        /// Constructs a transaction for simple file upload
        /// </summary>
        /// <param name="data">Data to upload</param>
        /// <param name="keyFilePath">Path to the wallet private key .json file (exported from Arweave wallet)</param>
        /// <param name="reward">Custom reward amount</param>
        /// <param name="fileFormat">Data format in mimeType/extension</param>
        /// <returns>Transaction object</returns>
        public static Transaction CreateDataTransaction(Stream data, string fileFormat, string keyFilePath, string reward = "")
        {
            var transaction = new Transaction(keyFilePath);

            MemoryStream ms = new();
            data.CopyTo(ms);
            var dataBuff = ms.GetBuffer();

            transaction.AddTag("Content-Type", fileFormat);
            DataTransaction(transaction, dataBuff, reward);
            SignData(transaction);
            return transaction;
        }


        /// <summary>
        /// Constructs a transaction for simple AR token transfer
        /// </summary>
        /// <param name="keyFilePath">Path to the wallet private key .json file (exported from Arweave wallet)</param>
        /// <param name="quantity">Amount of ARs to send</param>
        /// <param name="target">Target wallet address</param>
        /// <param name="reward">Custom reward amount</param>
        /// <returns>Transaction object</returns>
        public static Transaction CreateW2WTransaction(string keyFilePath, string quantity, string target, string reward = "")
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

        /// <summary>
        /// 
        /// </summary>
        /// /// <param name="dataPath">Path to uploading file</param>
        /// <param name="keyFilePath">Path to the wallet private key .json file (exported from Arweave wallet)</param>
        /// <param name="quantity">Amount of ARs to send</param>
        /// <param name="target">Target wallet address</param>
        /// <param name="reward">Custom reward amount</param>
        /// <param name="typeFromPath">Determine content-type by file extension</param>
        /// <param name="contentType">Custom content type value</param>
        /// <returns>Transaction object</returns>
        public static Transaction CreateW2WTransactionWithData(string dataPath,
                                                          string keyFilePath,
                                                          string quantity,
                                                          string target,
                                                          string reward="",
                                                          bool typeFromPath = true,
                                                          string contentType = null)
        {
            var transaction = new Transaction(keyFilePath);
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

            DataTransaction(transaction, dataBuff, reward, quantity, target);
            SignData(transaction);
            return transaction;
        }


        public static Transaction CreateW2WTransactionWithData(Stream data,
                                                          string fileFormat,
                                                          string keyFilePath,
                                                          string quantity,
                                                          string target,
                                                          string reward = "")
        {
            var transaction = new Transaction(keyFilePath);

            MemoryStream ms = new();
            data.CopyTo(ms);
            var dataBuff = ms.GetBuffer();

            transaction.AddTag("Content-Type", fileFormat);
            DataTransaction(transaction, dataBuff, reward, quantity, target);
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

        private static void DataTransaction(Transaction transaction, byte[] data, string reward = "", string quantity = "0", string target = "")
        {
            var transactionService = new TransactionService();

            transaction.Quantity = quantity;
            transaction.Target = target;
            transaction.Data = Utils.Base64Encode(data);
            transaction.DataSize = data.Length.ToString();
            transaction.DataRoot = Utils.Base64Encode(ChunkOperations.GenerateTransactionChunks(data));
            transaction.Reward = string.IsNullOrEmpty(reward) ? transactionService.GetPriceAsync(null, data.LongLength).Result : reward;
            transaction.LastTx = transactionService.GetAnchorAsync().Result;
        }

    }
    

}
