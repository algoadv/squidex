// ==========================================================================
//  IHandler.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System.Threading.Tasks;
using Orleans;

namespace Squidex.Domain.Apps.Write
{
    public interface IHandler<TCommand> : IGrainWithGuidKey
    {
        Task<object> HandleAsync(TCommand command);
    }
}
