// ==========================================================================
//  BaseJsonStorageProvider.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Serialization;
using Orleans.Storage;
using Squidex.Infrastructure.Tasks;

namespace Squidex.Config.Orleans
{
    public abstract class BaseJSONStorageProvider : IStorageProvider
    {
        private JsonSerializerSettings serializerSettings;

        public Logger Log { get; protected set; }

        public string Name { get; private set; }

        protected IJSONStateDataManager DataManager { get; set; }

        protected BaseJSONStorageProvider()
        {
        }

        public virtual Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {
            Log = providerRuntime.GetLogger(this.GetType().FullName);

            this.serializerSettings =
                OrleansJsonSerializer.GetDefaultSerializerSettings(
                    providerRuntime.ServiceProvider.GetRequiredService<SerializationManager>(),
                    providerRuntime.ServiceProvider.GetRequiredService<IGrainFactory>());

            return TaskHelper.Done;
        }

        public Task Close()
        {
            DataManager?.Dispose();

            return TaskHelper.Done;
        }

        public async Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var grainTypeName = GetGrainTypeName(grainType);

            var entityData = await DataManager.ReadAsync(grainTypeName, grainReference.ToKeyString());

            if (entityData != null)
            {
                ConvertFromStorageFormat(grainState, entityData);
            }
        }

        public Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            CheckDataManager();

            var grainTypeName = GetGrainTypeName(grainType);
            var grainDataJson = ConvertToStorageFormat(grainState);

            return DataManager.WriteAsync(grainTypeName, grainReference.ToKeyString(), grainDataJson);
        }

        public Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            CheckDataManager();

            var grainTypeName = GetGrainTypeName(grainType);

            return DataManager.DeleteAsync(grainTypeName, grainReference.ToKeyString());
        }

        private void CheckDataManager()
        {
            if (DataManager == null)
            {
                throw new InvalidOperationException("DataManager property not initialized");
            }
        }

        private static string GetGrainTypeName(string grainType)
        {
            return grainType.Split('.').Last();
        }

        protected JToken ConvertToStorageFormat(IGrainState grainState)
        {
            return JToken.FromObject(grainState, JsonSerializer.Create(this.serializerSettings));
        }

        protected void ConvertFromStorageFormat(IGrainState grainState, JToken entityData)
        {
            var serializer = JsonSerializer.Create(this.serializerSettings);

            serializer.Populate(new JTokenReader(entityData), grainState);
        }
    }
}
