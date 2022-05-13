using System.Net;

namespace Arweave.NET.Models
{
    public class ResponseResult
    {
        public string Id { get; set; }
        public Error Error { get; set; }
    }
    public partial class Error
    {
        public string Message { get; set; }
        public HttpStatusCode Code { get; set; }
    }
}
