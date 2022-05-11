using Arweave.NET.Models;
using Arweave.NET.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var transactionService = new TransactionService();

            var j = Encoding.UTF8.GetBytes("name");
            var d = Convert.ToBase64String(j);
            var chunk = new ChunkService();
            //var text = File.ReadAllText("C:/Users/semen/Desktop/data.txt");
            //var ms = new MemoryStream();
            //var buff = Array.Empty<byte>();
            //using (var sr = new StreamReader("C:/Users/semen/Desktop/5006649194446848.png"))
            //{               
            //    sr.BaseStream.CopyTo(ms);
            //    buff = ms.GetBuffer();
            //}
            //var bytes = Convert.FromBase64String("hwRZ9E0M7nePWV3UE5RPtTHsyo4aPgw1sxj32J3cQ5ptR7BPXq7sP085mxL5PRpa/xj2MMQTXtZb0FmpOt6sxVCky/qtlUoNzLix3dRWI8tsUxaZ1xHdTP9co72THRMo6++wnOZM8k17itlFSyEXLfotAVBD7r7YJiJU92jtq/Yr9a2WHzopfgrBrpglYQFDDquDOaHAJHvxG9yZaobRnU6jnLBwZRTg2bCzMoDU877nq5gfQVItWga1SeHkUlwUPHpW2OiOc0TBUm0ea5sskRXVjhvc0N5D34gzx02hesiY7+ltcMjL5vyl0YyIv2h1eR3Jg/ob+gaiG9nDFwpm1FocTjuOV/905hgAh5wbAcW1slQ0IJFioWyiLkwYw8+pOrVYGY4DBkBbRXTt+krWaL0ZBZISC7oZ6oVhDtRifCsQDPPx/B0xzg8HTSrtrDNrWBhwaOIB/Zg2TYZgJrJAtp1lB3ktxoJmxPB6RaYzSEVf9yPevZgHwsUq3c1U0UQz/TKpzuJRJJ3I9Ks2Rn37kCf9tN4xdq8di3Zqk2ky3J8oJPv/AVNIUIaEvIBsimVpT2kf30ILlAMYIRGjkP4wDNK9EnYjpOJVvBKYDb+DTTdmO/lV6u4npbdzn0H6DkJt0NlxynJMdhp9YRU+gpR7IG36hY2NZxhCrqtqw0jp/aU=");
            //var f = Convert.ToBase64String(bytes);
            //chunk.GenerateTransactionChunks(bytes);
          
            //var res = transactionService.GetAsync("ReUohI9tEmXQ6EN9H9IkRjY9bSdgql_OdLUCOeMEte0").Result;
            //var res = transactionService.GetAsync("BNttzDav3jHVnNiV7nYbQv-GY0HQ-4XXsdkE5K9ylHQ").Result;
           // var json = JsonSerializer.Serialize(res);
           // File.WriteAllText("C:/Users/semen/Desktop/Data.json", json);
            //var res = transactionService.GetStatusAsync("BNttzDav3jHVnNiV7nYbQv-GY0HQ-4XXsdkE5K9ylHQ").Result;
            //var res = transactionService.GetFieldAsync("BNttzDav3jHVnNiV7nYbQv-GY0HQ-4XXsdkE5K9ylHQ", "signature").Result;
            //var res = transactionService.GetAnchorAsync().Result;
            //var res = transactionService.GetStringDataAsync("BNttzDav3jHVnNiV7nYbQv-GY0HQ-4XXsdkE5K9ylHQ").Result;
            //var res = transactionService.GetStringDataByExtensionAsync("BNttzDav3jHVnNiV7nYbQv-GY0HQ-4XXsdkE5K9ylHQ","html").Result;
            //var res = transactionService.GetPriceAsync("JLZKhihKvyCR7If7kUhLSAN_DmRsZYD_cH3MYXHn9gM", 0).Result;
            //if (res != null)
            //{
            //    Console.Write(res.ToExampleString());
            //}
            //var res = transactionService.GetByteDataAsync("BNttzDav3jHVnNiV7nYbQv-GY0HQ-4XXsdkE5K9ylHQ").Result;
            //if(res!=null)
            //{
            //    Console.WriteLine(res.Length);
            //}

            //var transaction = new Transaction("c:\\Users\\semen\\Downloads\\HP32h0fNTv6VXLIM2fRIGF0h1VESfV4Tc0GVUXMxiNQ.json");
            var f = transactionService.SubmitTransaction("C:/Users/semen/Desktop/5006649194446848.png", new Tag[0]).Result;
            Console.ReadKey();
        }
    }
}
