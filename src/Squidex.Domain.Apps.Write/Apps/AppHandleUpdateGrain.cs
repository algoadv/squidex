// ==========================================================================
//  AppHandleUpdateGrain.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;
using Squidex.Domain.Apps.Write.Apps.Commands;
using Squidex.Infrastructure.CQRS.Commands;

namespace Squidex.Domain.Apps.Write.Apps
{
    [StatelessWorker]
    public sealed class AppHandleUpdateGrain : Grain,
        IHandler<AssignContributor>,
        IHandler<RemoveContributor>
    {
        public async Task<object> HandleAsync(AssignContributor command)
        {
            var appGrain = GrainFactory.GetGrain<IAppGrain>(command.AppId.Id);
            var appVersion = await appGrain.AssignContributor(command);

            return new EntitySavedResult(appVersion);
        }

        public async Task<object> HandleAsync(RemoveContributor command)
        {
            var appGrain = GrainFactory.GetGrain<IAppGrain>(command.AppId.Id);
            var appVersion = await appGrain.RemoveContributor(command);

            return new EntitySavedResult(appVersion);
        }
    }
}
