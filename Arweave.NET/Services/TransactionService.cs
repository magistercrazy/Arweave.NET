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

        public async Task<ResponseResult> SubmitTransaction(Transaction transaction, string dataPath, bool typeFromPath = true)
        {
            try
            {
                var dataBuff = await Utils.ReadDataAsync(dataPath);

                transaction.CreateDataTransaction(dataBuff);
                if (typeFromPath)
                {
                    var format = Utils.GetFileFormat(dataPath);
                    if (string.IsNullOrEmpty(format))
                        return new ResponseResult { Error = new Error { Message = "Couldn't resolve format" } };
                    transaction.AddTag("Content-Type", format);
                }

                transaction.Reward = await GetPriceAsync(null, dataBuff.LongLength);
                transaction.LastTx = await GetAnchorAsync();


                var dataToSign = Signature.GetSignature(transaction);
                var calcSign = Signature.Sign(dataToSign, transaction.JWK);
                transaction.Signature = Utils.Base64Encode(calcSign);
                transaction.Id = Utils.Base64Encode(Encryption.Hash(calcSign, "SHA-256"));

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, "tx");
                var json = JsonSerializer.Serialize(transaction);
                requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await Client.Request(requestMessage);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return new ResponseResult
                    {
                        Id = transaction.Id,
                        Error = null
                    };
                }
                var res = await response.Content.ReadAsStringAsync();
                return new ResponseResult
                {
                    Error = new Error
                    {
                        Message = res,
                        Code = response.StatusCode
                    }
                };
            }
            catch(Exception exp)
            {
                return new ResponseResult
                {
                    Error = new Error { Message = exp.Message }
                };
            }
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

        

       
        #endregion
    }
}
