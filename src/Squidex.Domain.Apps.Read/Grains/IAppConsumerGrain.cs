// ==========================================================================
//  IAppConsumerGrain.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using System.Threading.Tasks;
using Orleans;
using Squidex.Infrastructure.CQRS.Events;

namespace Squidex.Domain.Apps.Read.Grains
{
    public interface IAppConsumerGrain : IGrainWithGuidKey
    {
        Task StartAsync();

        Task RestartAsync();

        Task OnEventAsync(Guid subscriptionId, StoredEvent storedEvent);

        Task OnErrorAsync(Guid subscriptionId, string error);
    }
}
