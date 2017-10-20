// ==========================================================================
//  IAppConsumerRegistryGrain.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using System.Threading.Tasks;
using Orleans;

namespace Squidex.Domain.Apps.Read.Grains
{
    public interface IAppConsumerRegistryGrain : IGrainWithGuidKey
    {
        Task CreateAsync(Guid appId);

        Task RestartAsync();
    }
}
