using DotnetElastic.Models;

namespace DotnetElastic.Services
{
    public interface IElasticService
    {
        Task CreateIndexIfNotExistsAsync(string indexName);
        Task<bool> AddOrUpdate(AppUser user);    
        Task<bool> AddOrUpdateBulk(IEnumerable<AppUser> users, string indexName);
        Task<AppUser> Get(string key);
        Task<List<AppUser>?> GetAll();
        Task<bool> Delete(string key);
        Task<long?> DeleteAll();
    }
}