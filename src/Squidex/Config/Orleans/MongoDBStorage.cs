// ==========================================================================
//  MongoDBStorage.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using Orleans.Providers;
using Squidex.Infrastructure.MongoDb;

namespace Squidex.Config.Orleans
{
    public sealed class MongoDBStorage : BaseJSONStorageProvider, IJSONStateDataManager
    {
        private IMongoDatabase database;

        public override async Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {
            await base.Init(name, providerRuntime, config);

            var mongoConnectionString = config.Properties["ConnectionString"];
            var mongoDatabase = config.Properties["Database"];

            if (string.IsNullOrWhiteSpace(mongoConnectionString))
            {
                throw new ArgumentException("ConnectionString property not set", nameof(config));
            }

            if (string.IsNullOrWhiteSpace(mongoDatabase))
            {
                throw new ArgumentException("Database property not set", nameof(config));
            }

            var client = new MongoClient(mongoConnectionString);

            database = client.GetDatabase(mongoDatabase);

            DataManager = this;
        }

        void IDisposable.Dispose()
        {
        }

        Task IJSONStateDataManager.DeleteAsync(string collectionName, string key)
        {
            var collection = GetCollection(collectionName);

            var builder = Builders<BsonDocument>.Filter.Eq("key", key);

            return collection.DeleteManyAsync(builder);
        }

        async Task<JToken> IJSONStateDataManager.ReadAsync(string collectionName, string key)
        {
            var collection = GetCollection(collectionName);

            var documentQuery = Builders<BsonDocument>.Filter.Eq("_id", key);
            var document = await collection.Find(documentQuery).FirstOrDefaultAsync();

            if (document == null)
            {
                return null;
            }

            document.Remove("_id");

            return document.ToJson();
        }

        async Task IJSONStateDataManager.WriteAsync(string collectionName, string key, JToken entityData)
        {
            var collection = GetCollection(collectionName);

            var documentQuery = Builders<BsonDocument>.Filter.Eq("_id", key);
            var document = (BsonDocument)entityData.ToBson();

            document["_id"] = key;

            await collection.ReplaceOneAsync(documentQuery, document, new UpdateOptions { IsUpsert = true });
        }

        private IMongoCollection<BsonDocument> GetCollection(string name)
        {
            return database.GetCollection<BsonDocument>(name);
        }
    }
}
