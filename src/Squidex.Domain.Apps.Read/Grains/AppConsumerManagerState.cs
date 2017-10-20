// ==========================================================================
//  AppConsumerManagerState.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;

namespace Squidex.Domain.Apps.Read.Grains
{
    public sealed class AppConsumerManagerState
    {
        public List<Guid> Consumers { get; } = new List<Guid>();
    }
}