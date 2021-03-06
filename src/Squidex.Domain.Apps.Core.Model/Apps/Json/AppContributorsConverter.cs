﻿// ==========================================================================
//  AppContributorsConverter.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using Newtonsoft.Json;
using Squidex.Infrastructure.Json;

namespace Squidex.Domain.Apps.Core.Apps.Json
{
    public sealed class AppContributorsConverter : JsonClassConverter<AppContributors>
    {
        protected override void WriteValue(JsonWriter writer, AppContributors value, JsonSerializer serializer)
        {
            var json = new Dictionary<string, AppContributorPermission>(value.Count);

            foreach (var contributor in value)
            {
                json.Add(contributor.Key, contributor.Value);
            }

            serializer.Serialize(writer, json);
        }

        protected override AppContributors ReadValue(JsonReader reader, JsonSerializer serializer)
        {
            var json = serializer.Deserialize<Dictionary<string, AppContributorPermission>>(reader);

            var contributors = new AppContributors();

            foreach (var contributor in json)
            {
                contributors.Assign(contributor.Key, contributor.Value);
            }

            return contributors;
        }
    }
}
