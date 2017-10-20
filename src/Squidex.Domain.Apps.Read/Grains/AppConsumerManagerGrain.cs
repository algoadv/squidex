// ==========================================================================
//  AppConsumerManagerGrain.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Orleans;

namespace Squidex.Domain.Apps.Read.Grains
{
    public sealed class AppConsumerManagerGrain : Grain<AppConsumerManagerState>, IAppConsumerRegistryGrain
    {
        public Task RestartAsync()
        {
            return DoAsync(c => c.RestartAsync());
        }

        public async Task CreateAsync(Guid appId)
        {
            if (!State.Consumers.Contains(appId))
            {
                var grain = GrainFactory.GetGrain<IAppConsumerGrain>(appId);

                await grain.StartAsync();

                State.Consumers.Add(appId);
            }
        }

        private Task DoAsync(Func<IAppConsumerGrain, Task> action)
        {
            var actions =
                State.Consumers
                    .Select(x => GrainFactory.GetGrain<IAppConsumerGrain>(x))
                    .Select(action);

            return Task.WhenAll(actions);
        }
    }
}
