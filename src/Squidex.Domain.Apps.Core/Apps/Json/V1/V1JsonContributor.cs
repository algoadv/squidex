// ==========================================================================
//  V1JsonContributor.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using Newtonsoft.Json;

namespace Squidex.Domain.Apps.Core.Apps.Json.V1
{
    public sealed class V1JsonContributor
    {
        [JsonProperty]
        public string ContributorId { get; set; }

        [JsonProperty]
        public AppContributorPermission Permission { get; set; }

        public static V1JsonContributor Create(string id, AppContributorPermission permission)
        {
            return new V1JsonContributor { ContributorId = id, Permission = permission };
        }
    }
}
