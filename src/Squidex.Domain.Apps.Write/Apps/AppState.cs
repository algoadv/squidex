// ==========================================================================
//  AppState.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using Squidex.Domain.Apps.Core;
using Squidex.Domain.Apps.Core.Apps;
using Squidex.Domain.Apps.Events.Apps;
using Squidex.Infrastructure;

namespace Squidex.Domain.Apps.Write.Apps
{
    public sealed class AppState
    {
        private readonly Dictionary<string, AppContributorPermission> contributors = new Dictionary<string, AppContributorPermission>();
        private LanguagesConfig languagesConfig = LanguagesConfig.Empty;
        private AppStatePlan plan;
        private string name;

        public IReadOnlyDictionary<string, AppContributorPermission> Contributors
        {
            get { return this.contributors; }
        }

        public LanguagesConfig Config
        {
            get { return languagesConfig; }
        }

        public AppStatePlan Plan
        {
            get { return plan; }
        }

        public string Name
        {
            get { return name; }
        }

        public void Apply(AppCreated @event)
        {
            name = @event.Name;
        }

        public void On(AppContributorAssigned @event)
        {
            contributors[@event.ContributorId] = @event.Permission;
        }

        public void Apply(AppContributorRemoved @event)
        {
            contributors.Remove(@event.ContributorId);
        }

        public void Apply(AppClientAttached @event)
        {
        }

        public void Apply(AppClientUpdated @event)
        {
        }

        public void Apply(AppClientRenamed @event)
        {
        }

        public void Apply(AppClientRevoked @event)
        {
        }

        public void Apply(AppLanguageAdded @event)
        {
            languagesConfig = Config.Add(@event.Language);
        }

        public void Apply(AppLanguageRemoved @event)
        {
            languagesConfig = Config.Remove(@event.Language);
        }

        public void Apply(AppLanguageUpdated @event)
        {
            languagesConfig = Config.Update(@event.Language, @event.IsOptional, @event.IsMaster, @event.Fallback);
        }

        public void Apply(AppPlanChanged @event)
        {
        }

        public void ThrowIfNotCreated()
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new DomainException("App has not been created.");
            }
        }

        public void ThrowIfCreated()
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                throw new DomainException("App has already been created.");
            }
        }
    }
}
