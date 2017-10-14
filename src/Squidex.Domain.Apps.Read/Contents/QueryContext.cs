﻿// ==========================================================================
//  QueryContext.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Squidex.Domain.Apps.Core.Contents;
using Squidex.Domain.Apps.Read.Apps;
using Squidex.Domain.Apps.Read.Assets;
using Squidex.Domain.Apps.Read.Assets.Repositories;
using Squidex.Infrastructure;
using Squidex.Domain.Apps.Core.Apps;

namespace Squidex.Domain.Apps.Read.Contents
{
    public class QueryContext
    {
        private readonly ConcurrentDictionary<Guid, Content> cachedContents = new ConcurrentDictionary<Guid, Content>();
        private readonly ConcurrentDictionary<Guid, IAssetEntity> cachedAssets = new ConcurrentDictionary<Guid, IAssetEntity>();
        private readonly IContentQueryService contentQuery;
        private readonly IAssetRepository assetRepository;
        private readonly App app;
        private readonly ClaimsPrincipal user;

        public QueryContext(
            App app,
            IAssetRepository assetRepository,
            IContentQueryService contentQuery,
            ClaimsPrincipal user)
        {
            Guard.NotNull(assetRepository, nameof(assetRepository));
            Guard.NotNull(contentQuery, nameof(contentQuery));
            Guard.NotNull(app, nameof(app));
            Guard.NotNull(user, nameof(user));

            this.assetRepository = assetRepository;
            this.contentQuery = contentQuery;

            this.user = user;

            this.app = app;
        }

        public async Task<IAssetEntity> FindAssetAsync(Guid id)
        {
            var asset = cachedAssets.GetOrDefault(id);

            if (asset == null)
            {
                asset = await assetRepository.FindAssetAsync(id).ConfigureAwait(false);

                if (asset != null)
                {
                    cachedAssets[asset.Id] = asset;
                }
            }

            return asset;
        }

        public async Task<Content> FindContentAsync(Guid schemaId, Guid id)
        {
            var content = cachedContents.GetOrDefault(id);

            if (content == null)
            {
                content = (await contentQuery.FindContentAsync(app, schemaId.ToString(), user, id).ConfigureAwait(false)).Content;

                if (content != null)
                {
                    cachedContents[content.Id] = content;
                }
            }

            return content;
        }

        public async Task<IReadOnlyList<IAssetEntity>> QueryAssetsAsync(string query, int skip = 0, int take = 10)
        {
            var assets = await assetRepository.QueryAsync(app.Id, null, null, query, take, skip);

            foreach (var asset in assets)
            {
                cachedAssets[asset.Id] = asset;
            }

            return assets;
        }

        public async Task<IReadOnlyList<Content>> QueryContentsAsync(string schemaIdOrName, string query)
        {
            var contents = (await contentQuery.QueryWithCountAsync(app, schemaIdOrName, user, false, query).ConfigureAwait(false)).Items;

            foreach (var content in contents)
            {
                cachedContents[content.Id] = content;
            }

            return contents;
        }

        public async Task<IReadOnlyList<IAssetEntity>> GetReferencedAssetsAsync(ICollection<Guid> ids)
        {
            Guard.NotNull(ids, nameof(ids));

            var notLoadedAssets = new HashSet<Guid>(ids.Where(id => !cachedAssets.ContainsKey(id)));

            if (notLoadedAssets.Count > 0)
            {
                var assets = await assetRepository.QueryAsync(app.Id, null, notLoadedAssets, null, int.MaxValue).ConfigureAwait(false);

                foreach (var asset in assets)
                {
                    cachedAssets[asset.Id] = asset;
                }
            }

            return ids.Select(id => cachedAssets.GetOrDefault(id)).Where(x => x != null).ToList();
        }

        public async Task<IReadOnlyList<Content>> GetReferencedContentsAsync(Guid schemaId, ICollection<Guid> ids)
        {
            Guard.NotNull(ids, nameof(ids));

            var notLoadedContents = new HashSet<Guid>(ids.Where(id => !cachedContents.ContainsKey(id)));

            if (notLoadedContents.Count > 0)
            {
                var contents = (await contentQuery.QueryWithCountAsync(app, schemaId.ToString(), user, false, notLoadedContents).ConfigureAwait(false)).Items;

                foreach (var content in contents)
                {
                    cachedContents[content.Id] = content;
                }
            }

            return ids.Select(id => cachedContents.GetOrDefault(id)).Where(x => x != null).ToList();
        }
    }
}
