// ==========================================================================
//  App.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using NodaTime;
using Squidex.Infrastructure;

namespace Squidex.Domain.Apps.Core.Apps
{
    public sealed class App : ImmutableDomainObject
    {
        private readonly string name;
        private ImmutableDictionary<string, AppContributorPermission> contributors = ImmutableDictionary<string, AppContributorPermission>.Empty;
        private AppClients clients = AppClients.Empty;
        private LanguagesConfig languages = LanguagesConfig.Create(Language.EN);
        private RefToken planOwner;
        private string planId;

        public string Name
        {
            get { return name; }
        }

        public string PlanId
        {
            get { return planId; }
        }

        public RefToken PlanOwner
        {
            get { return planOwner; }
        }

        public LanguagesConfig Languages
        {
            get { return languages; }
        }

        public PartitionResolver PartitionResolver
        {
            get { return languages.ToResolver(); }
        }

        public IReadOnlyDictionary<string, AppClient> Clients
        {
            get { return clients.Clients; }
        }

        public IReadOnlyDictionary<string, AppContributorPermission> Contributors
        {
            get { return contributors; }
        }

        private App(Guid id, Instant now, RefToken actor, string name)
            : base(id, now, actor)
        {
            this.name = name;
        }

        public static App Create(Guid id, Instant now, RefToken actor, string name)
        {
            Guard.NotNullOrEmpty(name, nameof(name));

            return new App(id, now, actor, name);
        }

        public App ChangePlan(Instant now, RefToken newPlanOwner, string newPlanId)
        {
            return Update<App>(now, newPlanOwner, clone =>
            {
                clone.planId = newPlanId;
                clone.planOwner = newPlanOwner;
            });
        }

        public App UpdateLanguages(Instant now, RefToken actor, Func<LanguagesConfig, LanguagesConfig> updater)
        {
            Guard.NotNull(updater, nameof(updater));

            return Update<App>(now, actor, clone =>
            {
                clone.languages = updater(languages) ?? clone.languages;
            });
        }

        public App UpdateClients(Instant now, RefToken actor, Func<AppClients, AppClients> updater)
        {
            Guard.NotNull(updater, nameof(updater));

            return Update<App>(now, actor, clone =>
            {
                clone.clients = updater(clients) ?? clone.clients;
            });
        }

        public App AssignContributor(Instant now, RefToken actor, string contributorId, AppContributorPermission permission)
        {
            Guard.Enum(permission, nameof(permission));

            return Update<App>(now, actor, clone =>
            {
                clone.contributors = contributors.SetItem(contributorId, permission);
            });
        }

        public App RemoveContributor(Instant now, RefToken actor, string contributorId)
        {
            return Update<App>(now, actor, clone =>
            {
                clone.contributors = contributors.Remove(contributorId);
            });
        }
    }
}
