// ==========================================================================
//  V1JsonApp.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NodaTime;
using Squidex.Infrastructure;

#pragma warning disable IDE0017 // Simplify object initialization

namespace Squidex.Domain.Apps.Core.Apps.Json.V1
{
    public sealed class V1JsonApp
    {
        [JsonProperty]
        public Guid Id { get; set; }

        [JsonProperty]
        public Instant Created { get; set; }

        [JsonProperty]
        public Instant LastModified { get; set; }

        [JsonProperty]
        public RefToken CreatedBy { get; set; }

        [JsonProperty]
        public RefToken LastModifiedBy { get; set; }

        [JsonProperty]
        public RefToken PlanOwner { get; set; }

        [JsonProperty]
        public string PlanId { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public string MasterLanguage { get; set; }

        [JsonProperty]
        public List<V1JsonClient> Clients { get; set; }

        [JsonProperty]
        public List<V1JsonLanguage> Languages { get; set; }

        [JsonProperty]
        public List<V1JsonContributor> Contributors { get; set; }

        public static V1JsonApp Create(App app)
        {
            var jsonApp = new V1JsonApp();

            jsonApp.Id = app.Id;
            jsonApp.Name = app.Name;
            jsonApp.PlanId = app.PlanId;
            jsonApp.PlanOwner = app.PlanOwner;
            jsonApp.Created = app.Created;
            jsonApp.CreatedBy = app.CreatedBy;
            jsonApp.LastModified = app.LastModified;
            jsonApp.LastModifiedBy = app.LastModifiedBy;
            jsonApp.MasterLanguage = app.Languages.Master.Language;

            if (app.Contributors.Count > 0)
            {
                jsonApp.Contributors = new List<V1JsonContributor>();

                foreach (var kvp in app.Contributors)
                {
                    jsonApp.Contributors.Add(V1JsonContributor.Create(kvp.Key, kvp.Value));
                }
            }

            if (app.Clients.Count > 0)
            {
                jsonApp.Clients = new List<V1JsonClient>();

                foreach (var kvp in app.Clients)
                {
                    jsonApp.Clients.Add(V1JsonClient.Create(kvp.Key, kvp.Value));
                }
            }

            if (app.Languages.Count > 0)
            {
                jsonApp.Languages = new List<V1JsonLanguage>();

                foreach (var language in app.Languages.OfType<LanguageConfig>())
                {
                    jsonApp.Languages.Add(V1JsonLanguage.Create(language));
                }
            }

            return jsonApp;
        }

        public App ToApp()
        {
            var app = App.Create(Id, Created, CreatedBy, Name);

            var date = LastModified;
            var user = LastModifiedBy;

            if (PlanId != null && PlanOwner != null)
            {
                app = app.ChangePlan(date, PlanOwner, PlanId);
            }

            if (Contributors != null)
            {
                foreach (var contributor in Contributors)
                {
                    app = app.AssignContributor(date, user, contributor.ContributorId, contributor.Permission);
                }
            }

            if (Clients != null)
            {
                foreach (var client in Clients)
                {
                    app = app.UpdateClients(date, user, c => c.Add(client.Id, client.ToClient()));
                }
            }

            if (Languages != null)
            {
                app = app.UpdateLanguages(date, user, x => LanguagesConfig.Create(Languages.Select(l => l.ToLanguageConfig())));

                if (MasterLanguage != null)
                {
                    app = app.UpdateLanguages(date, user, l => l.MakeMaster(MasterLanguage));
                }
            }

            return app;
        }
    }
}
