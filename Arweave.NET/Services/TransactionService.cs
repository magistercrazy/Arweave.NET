using Arweave.NET.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<>

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
