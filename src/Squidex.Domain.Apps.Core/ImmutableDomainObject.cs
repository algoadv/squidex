// ==========================================================================
//  ImmutableDomainObject.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using NodaTime;
using Squidex.Infrastructure;

namespace Squidex.Domain.Apps.Core
{
    public abstract class ImmutableDomainObject : Cloneable
    {
        private readonly Guid id;
        private readonly Instant created;
        private readonly RefToken createdBy;
        private long version;
        private Instant lastModified;
        private RefToken lastModifiedBy;

        public Guid Id
        {
            get { return id; }
        }

        public Instant Created
        {
            get { return created; }
        }

        public RefToken CreatedBy
        {
            get { return createdBy; }
        }

        public Instant LastModified
        {
            get { return lastModified; }
        }

        public RefToken LastModifiedBy
        {
            get { return lastModifiedBy; }
        }

        public long Version
        {
            get { return version; }
        }

        protected ImmutableDomainObject(Guid id, Instant now, RefToken actor)
        {
            this.id = id;

            created = now;
            createdBy = actor;
            lastModified = now;
            lastModifiedBy = actor;

            OnInit();
        }

        protected T Update<T>(Instant now, RefToken actor, Action<T> updater) where T : class
        {
            return (T)Clone(now, actor, true, x => updater(x as T));
        }

        protected T UpdateWithoutVersion<T>(Action<T> updater) where T : class
        {
            return (T)Clone(LastModified, LastModifiedBy, false, x => updater(x as T));
        }

        private object Clone(Instant now, RefToken actor, bool updateVersion, Action<object> updater)
        {
            Guard.NotNull(actor, nameof(actor));
            Guard.NotNull(updater, nameof(updater));

            var clone = (ImmutableDomainObject)MemberwiseClone();

            updater(clone);

            if (updateVersion)
            {
                clone.version++;
            }

            clone.lastModified = now;
            clone.lastModifiedBy = actor;

            return clone;
        }
    }
}
