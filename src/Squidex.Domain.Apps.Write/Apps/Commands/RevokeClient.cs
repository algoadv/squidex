﻿// ==========================================================================
//  RevokeClient.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

namespace Squidex.Domain.Apps.Write.Apps.Commands
{
    public sealed class RevokeClient : AppAggregateCommand
    {
        public string Id { get; set; }
    }
}
