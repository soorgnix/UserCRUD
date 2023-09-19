using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace UserCRUD.Service
{
    public class TokenBlackListService : ITokenBlackListService
    {
        private readonly IDistributedCache _cache;

        public TokenBlackListService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task BlackListToken(string token)
        {
            string cacheKey = _ConvertToken(token);
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1) // Example: Cache for 5 minutes
            };
            await _cache.SetStringAsync(cacheKey, "revoked", cacheOptions);
        }

        public async Task<bool> IsBlackListed(string token)
        {
            string cacheKey = _ConvertToken(token);
            var cachedToken = await _cache.GetStringAsync(cacheKey);
            return !string.IsNullOrEmpty(cachedToken);
        }

        private string _ConvertToken(string token)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hashBytes);
        }
    }
}

