// ==========================================================================
//  AppEventConsumer.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using System.Threading.Tasks;
using Squidex.Domain.Apps.Core.Apps;
using Squidex.Domain.Apps.Events;
using Squidex.Domain.Apps.Events.Apps;
using Squidex.Domain.Apps.Events.Apps.Utils;
using Squidex.Domain.Apps.Read.Apps.Repositories;
using Squidex.Domain.Apps.Read.Apps.Services;
using Squidex.Infrastructure;
using Squidex.Infrastructure.CQRS.Events;

namespace Squidex.Domain.Apps.Read.Apps
{
    public sealed class AppEventConsumer : IEventConsumer
    {
        private readonly IAppRepository appRepository;
        private readonly IAppProvider appProvider;

        public AppEventConsumer(IAppRepository appRepository, IAppProvider appProvider)
        {
            Guard.NotNull(appRepository, nameof(appRepository));
            Guard.NotNull(appProvider, nameof(appProvider));

            this.appRepository = appRepository;
            this.appProvider = appProvider;
        }

        public string Name
        {
            get { return GetType().Name; }
        }

        public string EventsFilter
        {
            get { return "^app-"; }
        }

        public Task ClearAsync()
        {
            return appRepository.ClearAsync();
        }

        public async Task On(Envelope<IEvent> @event)
        {
            var version = @event.Headers.EventStreamNumber();

            if (@event.Payload is AppCreated appCreated)
            {
                var app = AppEventDispatcher.Dispatch(@event, null);

                await appRepository.CreateAsync(version, app);
            }
            else if (AppEventDispatcher.CanDispatch(@event))
            {
                var app = new Func<App, App>(a => AppEventDispatcher.Dispatch(@event, a));

                await appRepository.UpdateAsync(version, @event.Headers.AggregateId(), app);
            }
        }
    }
}
