// ==========================================================================
//  OrleansCommandMiddleware.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using System.Reflection;
using System.Threading.Tasks;
using Orleans;
using Squidex.Infrastructure.CQRS.Commands;

namespace Squidex.Domain.Apps.Write
{
    public sealed class OrleansCommandMiddleware : ICommandMiddleware
    {
        public async Task HandleAsync(CommandContext context, Func<Task> next)
        {
            var method =
                GetType().GetMethod(
                    nameof(HandleGenericAsync),
                    BindingFlags.Static |
                    BindingFlags.NonPublic)
                .MakeGenericMethod(context.Command.GetType());

            var task = (Task)method.Invoke(null, new object[] { context.Command, context });

            await task;
        }

        private static async Task HandleGenericAsync<T>(T command, CommandContext context)
        {
            var handler = GrainClient.GrainFactory.GetGrain<IHandler<T>>(Guid.Empty);

            var result = await handler.HandleAsync(command);

            context.Complete(result);
        }
    }
}
