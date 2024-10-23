using DotnetElastic.Configuration;
using DotnetElastic.Models;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Options;
using UserElastic = Elastic.Clients.Elasticsearch.Security.User;

namespace DotnetElastic.Services
{
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

        public async Task<bool> AddOrUpdate(AppUser user)
        {
            var response = await _client.IndexAsync(user, idx => 
                idx.Index(_elasticSettings.DefaultIndex)
                    .OpType(OpType.Index));

            return response.IsValidResponse;
        }

        public async Task<bool> AddOrUpdateBulk(IEnumerable<AppUser> users, string indexName)
        {
            var response = await _client.BulkAsync(b => b.Index(_elasticSettings.DefaultIndex)
                .UpdateMany(users, (ud, u) => ud.Doc(u).DocAsUpsert(true)));

            return response.IsValidResponse;
        }

        public async Task CreateIndexIfNotExistsAsync(string indexName)
        {
            if (!_client.Indices.Exists(indexName).Exists)
                await _client.Indices.CreateAsync(indexName);  
        }

        public async Task<bool> Delete(string key)
        {
            var response = await _client.DeleteAsync<AppUser>(key, d => d.Index(_elasticSettings.DefaultIndex));
            return response.IsValidResponse;
        }

        public async Task<long?> DeleteAll()
        {
            var response = await _client.DeleteByQueryAsync<AppUser>(d => d.Indices(_elasticSettings.DefaultIndex));
            return response.IsValidResponse ? response.Deleted : default; 
        }

        public async Task<AppUser> Get(string key)
        {
            var response = await _client.GetAsync<AppUser>(key, g => g.Index(_elasticSettings.DefaultIndex));
            return response.Source;
        }

        public async Task<List<AppUser>?> GetAll()
        {
            var response = await _client.SearchAsync<AppUser>(s => s.Index(_elasticSettings.DefaultIndex));
            return response.Documents.ToList();
        }
    }
}
