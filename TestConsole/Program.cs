using Arweave.NET.Services;
using System;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var transactionService = new TransactionService();

            //var res = transactionService.GetAsync("G8kyqNYgnqDoh0vuNjdmK-ova3OXdUr50iRkkhXFoS4").Result;
            //var res = transactionService.GetStatusAsync("BNttzDav3jHVnNiV7nYbQv-GY0HQ-4XXsdkE5K9ylHQ").Result;
            //var res = transactionService.GetFieldAsync("BNttzDav3jHVnNiV7nYbQv-GY0HQ-4XXsdkE5K9ylHQ", "signature").Result;
            //var res = transactionService.GetAnchorAsync().Result;
            //var res = transactionService.GetStringDataAsync("BNttzDav3jHVnNiV7nYbQv-GY0HQ-4XXsdkE5K9ylHQ").Result;
            //var res = transactionService.GetStringDataByExtensionAsync("BNttzDav3jHVnNiV7nYbQv-GY0HQ-4XXsdkE5K9ylHQ","html").Result;
            var res = transactionService.GetPriceAsync("JLZKhihKvyCR7If7kUhLSAN_DmRsZYD_cH3MYXHn9gM", 0).Result;
            if (res != null)
            {
                Console.Write(res.ToString());
            }
            //var res = transactionService.GetByteDataAsync("BNttzDav3jHVnNiV7nYbQv-GY0HQ-4XXsdkE5K9ylHQ").Result;
            //if(res!=null)
            //{
            //    Console.WriteLine(res.Length);
            //}
            Console.ReadKey();
        }
    }
}
