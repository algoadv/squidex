// ==========================================================================
//  OrleansHostBuilder.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using Orleans.Runtime.Host;

namespace Squidex.Config.Orleans
{
    public static class OrleansHostBuilder
    {
        public static void Run()
        {
            var localNode = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11111);

            var clusterConfiguration = new ClusterConfiguration();
            clusterConfiguration.Defaults.ProxyGatewayEndpoint = new IPEndPoint(IPAddress.Any, 30000);
            clusterConfiguration.Defaults.Port = 11111;
            clusterConfiguration.Defaults.SiloName = "squidex";
            clusterConfiguration.Defaults.HostNameOrIPAddress = "localhost";

            clusterConfiguration.Globals.SeedNodes.Add(localNode);
            clusterConfiguration.Globals.LivenessType = GlobalConfiguration.LivenessProviderType.MembershipTableGrain;
            clusterConfiguration.Globals.ReminderServiceType = GlobalConfiguration.ReminderServiceProviderType.ReminderTableGrain;
            clusterConfiguration.Globals.RegisterStorageProvider<MongoDBStorage>("Default",
                new Dictionary<string, string>
                {
                    { "ConnectionString", "mongodb://localhost" },
                    { "Database", "Orleans" }
                });

            clusterConfiguration.PrimaryNode = localNode;

            var siloHost = new SiloHost("squidex", clusterConfiguration);
            try
            {
                siloHost.LoadOrleansConfig();
                siloHost.InitializeOrleansSilo();

                if (siloHost.StartOrleansSilo())
                {
                    Console.WriteLine($"Successfully started Orleans silo '{siloHost.Name}' as a {siloHost.Type} node.");
                }
                else
                {
                    throw new OrleansException($"Failed to start Orleans silo '{siloHost.Name}' as a {siloHost.Type} node.");
                }
            }
            catch (Exception exc)
            {
                siloHost.ReportStartupError(exc);

                Console.Error.WriteLine(exc);

                throw new OrleansException($"Failed to start Orleans silo '{siloHost.Name}' as a {siloHost.Type} node.", exc);
            }

            var config = new ClientConfiguration()
            {
                GatewayProvider = ClientConfiguration.GatewayProviderType.Config
            };

            var hostEntry = Dns.GetHostEntryAsync("localhost").Result;
            var address = hostEntry.AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork);
            config.Gateways.Add(new IPEndPoint(address, 30000));

            GrainClient.Initialize(config);
        }
    }
}
