// ==========================================================================
//  AppClients.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Collections.Immutable;
using Squidex.Infrastructure;

namespace Squidex.Domain.Apps.Core.Apps
{
    public sealed class AppClients : Cloneable<AppClients>
    {
        public static readonly AppClients Empty = new AppClients();

        private ImmutableDictionary<string, AppClient> clients = ImmutableDictionary<string, AppClient>.Empty;

        public IReadOnlyDictionary<string, AppClient> Clients
        {
            get { return clients; }
        }

        private AppClients()
        {
        }

        public AppClients Add(string clientId, AppClient client)
        {
            Guard.NotNull(client, nameof(client));

            return Clone(clone =>
            {
                clone.clients = clients.SetItem(clientId, client);
            });
        }

        public AppClients Add(string clientId, string secret)
        {
            return Clone(clone =>
            {
                clone.clients = clients.SetItem(clientId, new AppClient(secret, clientId, AppClientPermission.Editor));
            });
        }

        public AppClients Rename(string clientId, string name)
        {
            return Clone(clone =>
            {
                clone.clients = clients.SetItem(clientId, clients[clientId].Rename(name));
            });
        }

        public AppClients Update(string clientId, AppClientPermission permission)
        {
            return Clone(clone =>
            {
                clone.clients = clients.SetItem(clientId, clients[clientId].Update(permission));
            });
        }

        public AppClients Revoke(string clientId)
        {
            return Clone(clone =>
            {
                clone.clients = clients.Remove(clientId);
            });
        }
    }
}
