using Microsoft.Extensions.Caching.Distributed;

namespace UserCRUD.Service
{
    public interface ITokenBlackListService
    {
        public Task BlackListToken(string token);
        public Task<bool> IsBlackListed(string token);
    }
}
