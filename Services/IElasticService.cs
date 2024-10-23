using DotnetElastic.Models;

namespace DotnetElastic.Services;

public interface IElasticService{
    Task CreateIndexIfNotExistsAsync(string indexName);
    Task<bool> AddOrUpdate(User User);    
    Task<bool> AddOrUpdateBulk(IEnumerable<User> users, string indexName);
    Task<User> Get(string key);
    Task<List<User>?> GetAll();
    Task<User> Delete(string key);
    Task<long?> DeleteAll();
}