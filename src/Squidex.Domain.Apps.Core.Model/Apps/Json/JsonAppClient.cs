﻿// ==========================================================================
//  JsonAppClient.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using Newtonsoft.Json;
using Squidex.Infrastructure.Reflection;

namespace Squidex.Domain.Apps.Core.Apps.Json
{
    public class JsonAppClient
    {
        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public string Secret { get; set; }

        [JsonProperty]
        public AppClientPermission Permission { get; set; }

        public JsonAppClient()
        {
        }

        public JsonAppClient(AppClient client)
        {
            SimpleMapper.Map(client, this);
        }

        public AppClient ToClient()
        {
            return new AppClient(Name, Secret, Permission);
        }
    }
}
