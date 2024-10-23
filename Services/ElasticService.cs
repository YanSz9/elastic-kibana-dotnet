using DotnetElastic.Configuration;
using DotnetElastic.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Security;
using Microsoft.Extensions.Options;
using User = Elastic.Clients.Elasticsearch.Security.User;

namespace DotnetElastic.Services;

public class ElasticService : IElasticService
{
    private readonly ElasticsearchClient _client;
    private readonly ElasticSettings _elasticSettings;
    public ElasticService(IOptions<ElasticSettings> optionsMonitor)
    {
        _elasticSettings = optionsMonitor.Value;

        var settings = new ElasticsearchClientSettings(new Uri(_elasticSettings.Url))
            .DefaultIndex(_elasticSettings.DefaultIndex);
        
        _client = new ElasticsearchClient(settings);
    }
    public async Task<bool> AddOrUpdate(User User)
    {
        var response = await _client.IndexAsync(User, idx => 
            idx.Index(_elasticSettings.DefaultIndex)
                .OpType(OpType.Index));

        return response.IsValidResponse;
    }

    public async Task<bool> AddOrUpdateBulk(IEnumerable<User> users, string indexName)
    {
        var response = await _client.BulkAsync(b => b.Index(_elasticSettings.DefaultIndex)
            .UpdateMany(users, (ud, u) => ud.Doc(u).DocAsUpsert(true)));

        return response.IsValidResponse;
    }

    public async Task CreateIndexIfNotExistsAsync(string indexName)
    {
        if(!_client.Indices.Exists(indexName).Exists)
          await _client.Indices.CreateAsync(indexName);  
    }

    public async Task<bool> Delete(string key)
    {
        var response = await _client.DeleteAsync<User>(key, d=> d.Index(_elasticSettings.DefaultIndex));

        return response.IsValidResponse;
    }

    public async Task<long?> DeleteAll()
    {
        var response = await _client.DeleteByQueryAsync<User>(d=> d.Indices(_elasticSettings.DefaultIndex));

        return response.IsValidResponse ? response.Deleted : default; 
    }

    public async Task<User> Get(string key)
    {
        var response = await _client.GetAsync<User>(key, g=> g.Index(_elasticSettings.DefaultIndex));

        return response.Source;
    }

    public async Task<List<User>?> GetAll()
    {
        var response = await _client.SearchAsync<User>(s=> s.Index(_elasticSettings.DefaultIndex));

        return response.Documents.ToList();
    }
}