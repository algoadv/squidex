// ==========================================================================
//  IJSONStateDataManager.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Squidex.Config.Orleans
{
    public interface IJSONStateDataManager : IDisposable
    {
        Task DeleteAsync(string collectionName, string key);

        Task WriteAsync(string collectionName, string key, JToken entityData);

        Task<JToken> ReadAsync(string collectionName, string key);
    }
}
