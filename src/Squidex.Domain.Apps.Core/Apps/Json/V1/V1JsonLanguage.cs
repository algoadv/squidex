// ==========================================================================
//  V1JsonLanguage.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Squidex.Infrastructure;

namespace Squidex.Domain.Apps.Core.Apps.Json.V1
{
    public sealed class V1JsonLanguage
    {
        [JsonProperty]
        public string Iso2Code { get; set; }

        [JsonProperty]
        public bool IsOptional { get; set; }

        [JsonProperty]
        public List<string> Fallback { get; set; }

        public static V1JsonLanguage Create(LanguageConfig config)
        {
            var fallback = config.LanguageFallbacks?.Select(f => f.Iso2Code).ToList();

            return new V1JsonLanguage { Iso2Code = config.Language, IsOptional = config.IsOptional, Fallback = fallback };
        }

        public LanguageConfig ToLanguageConfig()
        {
            var fallback = Fallback?.Select<string, Language>(f => f);

            return new LanguageConfig(Iso2Code, IsOptional, fallback);
        }
    }
}
