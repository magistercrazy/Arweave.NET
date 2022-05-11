using Arweave.NET.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Arweave.NET.Services
{
    public class TransactionService:BaseService
    {
        public async Task<Transaction> GetAsync(string id)
        {
            Transaction transaction = null;
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"tx/{id}");
            var response = await Client.Request(requestMessage);

            if(response.StatusCode==HttpStatusCode.OK)
            {
                return JsonSerializer.Deserialize<Transaction>(await response.Content.ReadAsStringAsync());
            }

            return transaction;
        }

        public async Task<TransactionStatus> GetStatusAsync(string id)
        {
            TransactionStatus status = null;
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"tx/{id}/status");
            var response = await Client.Request(requestMessage);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return JsonSerializer.Deserialize<TransactionStatus>(await response.Content.ReadAsStringAsync());
            }

            return status;
        }

        public async Task<string> GetFieldAsync(string id,string fieldName)
        {
            if (!IsValidFieldName(fieldName))
                return null;
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"tx/{id}/{fieldName}");
            var response = await Client.Request(requestMessage);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return null;
        }

        public async Task<string> GetAnchorAsync()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "tx_anchor");
            var response = await Client.Request(requestMessage);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
                return null;
        }

        public async Task<string> GetStringDataAsync(string id)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{id}");
            var response = await Client.Request(requestMessage);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
                return null;
        }

        public async Task<string> GetStringDataByExtensionAsync(string id,string extension)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"tx/{id}/data.{extension}");
            var response = await Client.Request(requestMessage);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
                return null;
        }

        public async Task<byte[]> GetByteDataAsync(string id)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"tx/{id}");
            var response = await Client.Request(requestMessage);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            else
                return null;
        }

        public async Task<string> GetPriceAsync(string target,long byteLength = 0)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, string.IsNullOrEmpty(target)?$"price/{byteLength}": $"price/{byteLength}/{target} ");
            var response = await Client.Request(requestMessage);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
                return null;
        }

        public async Task<string> SubmitTransaction(string dataPath, Tag[] tags)
        {
            var chunk = new ChunkService();
            var encrypt = new EncryptionService();
            var utils = new Utils();
            var signature = new Signature();
            var transaction = new Transaction("c:\\Users\\semen\\Downloads\\HP32h0fNTv6VXLIM2fRIGF0h1VESfV4Tc0GVUXMxiNQ.json");
            var archor = await GetAnchorAsync();
            var reward = string.Empty;
            var buff = Array.Empty<byte>();
            using (FileStream fstream = File.OpenRead(dataPath))
            {
                buff = new byte[fstream.Length];
                reward = await GetPriceAsync(null, buff.LongLength);
                await fstream.ReadAsync(buff.AsMemory(0, buff.Length));

            }
            var dataRoot = chunk.GenerateTransactionChunks(buff);
            
            transaction.Quantity = "0";
            transaction.Target = "";
            transaction.Tags = ConvertTags(tags, utils);
            transaction.Data = utils.Base64Encode(buff);
            transaction.DataSize =buff.Length.ToString();
            transaction.Reward = reward;
            transaction.LastTx = archor;
            transaction.DataRoot = utils.Base64Encode(dataRoot);

            var dataToSign = signature.GetSignature(transaction);
            var calcSign = signature.Sign(dataToSign, transaction.GetJWK("c:\\Users\\semen\\Downloads\\HP32h0fNTv6VXLIM2fRIGF0h1VESfV4Tc0GVUXMxiNQ.json"));
            transaction.Signature = utils.Base64Encode(calcSign);
            transaction.Id = utils.Base64Encode(encrypt.Hash(calcSign, "SHA-256"));

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "tx");
            var json = JsonSerializer.Serialize(transaction);
            requestMessage.Content = new StringContent(json, Encoding.UTF8,"application/json");
            var response = await Client.Request(requestMessage);
            var res = await response.Content.ReadAsStringAsync();
            return res;
        }

        #region Helper Methods
        private bool IsValidFieldName(string fieldName)
        {
            var transactionTypeFields = typeof(Transaction).GetProperties();
            var fieldNames = new HashSet<string>();
            foreach (var pi in transactionTypeFields)
            {
                var attributes = pi.GetCustomAttributes();
                foreach (var attr in attributes)
                {
                    if (attr is JsonPropertyNameAttribute)
                    {
                        var val = (attr as JsonPropertyNameAttribute).Name;
                        fieldNames.Add(val);
                    }
                }

            }
            return fieldNames.Contains(fieldName);
        }

        private Tag[] ConvertTags(Tag[] tags, Utils utils)
        {
            var tagArr = new Tag[tags.Length];
            for (int i = 0; i < tags.Length; i++)
            {
                tagArr[i] = new Tag
                {
                    Name = utils.Base64Encode(Encoding.UTF8.GetBytes(tags[i].Name)),
                    Value = utils.Base64Encode(Encoding.UTF8.GetBytes(tags[i].Value))
                };
            }
            return tagArr;
        }

       
        #endregion
    }
}
