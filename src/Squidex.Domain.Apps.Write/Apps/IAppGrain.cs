// ==========================================================================
//  IAppGrain.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System.Threading.Tasks;
using Orleans;
using Squidex.Domain.Apps.Write.Apps.Commands;

namespace Squidex.Domain.Apps.Write.Apps
{
    public interface IAppGrain : IGrainWithGuidKey
    {
        Task<long> Create(CreateApp command);

        Task<long> AssignContributor(AssignContributor command);

        Task<long> RemoveContributor(RemoveContributor command);
    }
}
