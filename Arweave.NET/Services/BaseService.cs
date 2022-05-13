
namespace Arweave.NET.Services
{
    public abstract class BaseService
    {
        protected readonly Client Client; 

        public BaseService()
        {
            Client = new Client();
        }

        public BaseService(string baseUrl)
        {
            Client = new Client(baseUrl);
        }
    }
}
