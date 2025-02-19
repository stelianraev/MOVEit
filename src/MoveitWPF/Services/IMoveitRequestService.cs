using System.Net.Http;

namespace MoveitWpf.Services
{
    public interface IMoveitRequestService
    {
        Task<HttpResponseMessage> GetToken(string username, string password);
    }
}
