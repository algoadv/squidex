// ==========================================================================
//  IAppRepository.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squidex.Domain.Apps.Core.Apps;

namespace Squidex.Domain.Apps.Read.Apps.Repositories
{
    public interface IAppRepository
    {
        Task<IReadOnlyList<App>> QueryAllAsync(string subjectId);

        Task<App> FindAppAsync(Guid appId);

        Task<App> FindAppAsync(string name);

        Task ClearAsync();

        Task CreateAsync(long version, App app);

        Task UpdateAsync(long version, Guid id, Func<App, App> app);
    }
}
