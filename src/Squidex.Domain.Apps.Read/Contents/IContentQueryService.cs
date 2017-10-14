// ==========================================================================
//  IContentQueryService.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Squidex.Domain.Apps.Core.Apps;
using Squidex.Domain.Apps.Core.Contents;
using Squidex.Domain.Apps.Core.Schemas;

namespace Squidex.Domain.Apps.Read.Contents
{
    public interface IContentQueryService
    {
        Task<(Schema Schema, long Total, IReadOnlyList<Content> Items)> QueryWithCountAsync(App app, string schemaIdOrName, ClaimsPrincipal user, bool archived, HashSet<Guid> ids);

        Task<(Schema Schema, long Total, IReadOnlyList<Content> Items)> QueryWithCountAsync(App app, string schemaIdOrName, ClaimsPrincipal user, bool archived, string query);

        Task<(Schema Schema, Content Content)> FindContentAsync(App app, string schemaIdOrName, ClaimsPrincipal user, Guid id);

        Task<Schema> FindSchemaAsync(App app, string schemaIdOrName);
    }
}
