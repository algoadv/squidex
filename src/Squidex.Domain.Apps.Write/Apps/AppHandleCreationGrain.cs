// ==========================================================================
//  AppCreationGrain.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using Squidex.Domain.Apps.Read.Apps.Repositories;
using Squidex.Domain.Apps.Write.Apps.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.CQRS.Commands;

namespace Squidex.Domain.Apps.Write.Apps
{
    public sealed class AppHandleCreationGrain : Grain, IHandler<CreateApp>
    {
        private readonly HashSet<string> createdApps = new HashSet<string>();
        private readonly IAppRepository appRepository;

        public async Task<object> HandleAsync(CreateApp command)
        {
            if (createdApps.Contains(command.Name) || (appRepository != null && await appRepository.FindAppAsync(command.Name) != null))
            {
                createdApps.Add(command.Name);

                var error =
                    new ValidationError($"An app with name '{command.Name}' already exists",
                        nameof(CreateApp.Name));

                throw new ValidationException("Cannot create a new app.", error);
            }

            var appGrain = GrainFactory.GetGrain<IAppGrain>(command.AppId);
            var appVersion = await appGrain.Create(command);

            createdApps.Add(command.Name);

            return EntityCreatedResult.Create(command.AppId, appVersion);
        }
    }
}
