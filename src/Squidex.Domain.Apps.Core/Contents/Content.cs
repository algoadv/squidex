// ==========================================================================
//  Content.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using NodaTime;
using Squidex.Infrastructure;

namespace Squidex.Domain.Apps.Core.Contents
{
    public sealed class Content : ImmutableDomainObject
    {
        private NamedContentData data;
        private Status status;

        public NamedContentData Data
        {
            get { return data; }
        }

        public Status Status
        {
            get { return status; }
        }

        private Content(Guid id, Instant now, RefToken actor, NamedContentData data)
            : base(id, now, actor)
        {
            this.data = data;
        }

        public static Content Create(Guid id, Instant now, RefToken actor, NamedContentData data)
        {
            Guard.NotNull(data, nameof(data));

            return new Content(id, now, actor, data);
        }

        public Content UpdateData(Instant now, RefToken actor, NamedContentData newData)
        {
            Guard.NotNull(newData, nameof(newData));

            return Update<Content>(now, actor, clone =>
            {
                clone.data = newData;
            });
        }

        public Content ChangeStatus(Instant now, RefToken actor, Status newStatus)
        {
            Guard.Enum(newStatus, nameof(newStatus));

            return Update<Content>(now, actor, clone =>
            {
                clone.status = newStatus;
            });
        }

        public Content ReplaceContent(NamedContentData newData)
        {
            Guard.NotNull(newData, nameof(newData));

            return UpdateWithoutVersion<Content>(clone =>
            {
                clone.data = newData;
            });
        }
    }
}
