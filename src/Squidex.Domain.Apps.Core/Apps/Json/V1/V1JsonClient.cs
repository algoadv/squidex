// ==========================================================================
//  V1JsonClient.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using Newtonsoft.Json;

namespace Squidex.Domain.Apps.Core.Apps.Json.V1
{
    public sealed class V1JsonClient
    {
        [JsonProperty]
        public string Id { get; set; }

        [JsonProperty]
        public string Secret { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public AppClientPermission Permission { get; set; }

        public static V1JsonClient Create(string id, AppClient client)
        {
            return new V1JsonClient { Id = id, Name = client.Name, Secret = client.Secret, Permission = client.Permission };
        }

        public AppClient ToClient()
        {
            return new AppClient(Secret, Name, Permission);
        }
    }
}
