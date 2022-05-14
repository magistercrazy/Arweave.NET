using Arweave.NET.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Arweave.NET.Services
{
    public class TransactionService:BaseService
    {
        /// <summary>
        /// Get transaction by Id
        /// </summary>
        /// <param name="id">Transaction Id</param>
        /// <returns>Transaction object</returns>
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

        /// <summary>
        /// Get transaction status by Id
        /// </summary>
        /// <param name="id">Transaction Id</param>
        /// <returns>Transaction status</returns>
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

        /// <summary>
        /// Returns a specific transaction field value
        /// </summary>
        /// <param name="id">Transaction Id</param>
        /// <param name="fieldName">Field name to get</param>
        /// <returns></returns>
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

        /// <summary>
        /// Get last transaction anchor
        /// </summary>
        /// <returns>Anchor address</returns>
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

        /// <summary>
        /// Get transaction data field in string format
        /// </summary>
        /// <param name="id">Transaction Id</param>
        /// <returns>Data field as a string</returns>
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

        /// <summary>
        /// Get transaction data in string format with specifc extension type (like html/css/json/etc)
        /// </summary>
        /// <param name="id">Transaction Id</param>
        /// <param name="extension">Desired extension</param>
        /// <returns>Data field as a string</returns>
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

        /// <summary>
        /// Get data field as a plain byte array
        /// </summary>
        /// <param name="id">Transaction Id</param>
        /// <returns>Data buffer</returns>
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

        /// <summary>
        /// Get actual average reward needed for transaction approval
        /// </summary>
        /// <param name="target">Target wallet address</param>
        /// <param name="byteLength">Size of data in bytes</param>
        /// <returns>Price value in AR</returns>
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

        /// <summary>
        /// Submits the constructed transaction to the network
        /// </summary>
        /// <param name="transaction">Transaction object</param>
        /// <returns>Transaction result. Note: even if the submit response is 200, the transaction can be in pending state until network approval</returns>
        public async Task<ResponseResult> SubmitTransaction(Transaction transaction)
        {
            try
            {              
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
