// ==========================================================================
//  AppEventDispatcher.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using NodaTime;
using Squidex.Domain.Apps.Core.Apps;
using Squidex.Infrastructure;
using Squidex.Infrastructure.CQRS.Events;

namespace Squidex.Domain.Apps.Events.Apps.Utils
{
    public static class AppEventDispatcher
    {
        private static readonly Dictionary<Type, Func<Instant, RefToken, AppEvent, App, App>> Handlers =
            new Dictionary<Type, Func<Instant, RefToken, AppEvent, App, App>>();

        static AppEventDispatcher()
        {
            void AddHandler<T>(Func<Instant, RefToken, T, App, App> handler) where T : AppEvent
            {
                Handlers[typeof(T)] = (now, actor, @event, app) => handler(now, actor, (T)@event, app);
            }

            AddHandler<AppCreated>((now, actor, ev, app) =>
            {
                return App.Create(ev.AppId.Id, now, actor, ev.Name);
            });

            AddHandler<AppClientAttached>((now, actor, ev, app) =>
            {
                return app.UpdateClients(now, actor, c => c.Add(ev.Id, ev.Secret));
            });

            AddHandler<AppClientRenamed>((now, actor, ev, app) =>
            {
                return app.UpdateClients(now, actor, c => c.Rename(ev.Id, ev.Name));
            });

            AddHandler<AppClientRevoked>((now, actor, ev, app) =>
            {
                return app.UpdateClients(now, actor, c => c.Revoke(ev.Id));
            });

            AddHandler<AppClientUpdated>((now, actor, ev, app) =>
            {
                return app.UpdateClients(now, actor, c => c.Update(ev.Id, ev.Permission));
            });

            AddHandler<AppContributorAssigned>((now, actor, ev, app) =>
            {
                return app.AssignContributor(now, actor, ev.ContributorId, ev.Permission);
            });

            AddHandler<AppContributorRemoved>((now, actor, ev, app) =>
            {
                return app.RemoveContributor(now, actor, ev.ContributorId);
            });

            AddHandler<AppLanguageAdded>((now, actor, ev, app) =>
            {
                return app.UpdateLanguages(now, actor, l => l.Add(ev.Language));
            });

            AddHandler<AppLanguageRemoved>((now, actor, ev, app) =>
            {
                return app.UpdateLanguages(now, actor, l => l.Remove(ev.Language));
            });

            AddHandler<AppLanguageUpdated>((now, actor, ev, app) =>
            {
                return app.UpdateLanguages(now, actor, l => l.Update(ev.Language, ev.IsOptional, ev.IsMaster, ev.Fallback));
            });

            AddHandler<AppMasterLanguageSet>((now, actor, ev, app) =>
            {
                return app.UpdateLanguages(now, actor, l => l.MakeMaster(ev.Language));
            });

            AddHandler<AppPlanChanged>((now, actor, ev, app) =>
            {
                return app.ChangePlan(now, actor, ev.PlanId);
            });
        }

        public static bool CanDispatch(Envelope<IEvent> @event)
        {
            return Handlers.ContainsKey(@event.Payload.GetType());
        }

        public static App Dispatch(Envelope<IEvent> @event, App app)
        {
            var now = @event.Headers.Timestamp();

            var payload = @event.Payload;

            if (payload is AppEvent appEvent && Handlers.TryGetValue(payload.GetType(), out var handler))
            {
                return handler(@event.Headers.Timestamp(), appEvent.Actor, appEvent, app);
            }

            return app;
        }
    }
}
