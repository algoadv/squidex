// ==========================================================================
//  AppStatePlan.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using Squidex.Infrastructure;

namespace Squidex.Domain.Apps.Write.Apps
{
    public sealed class AppStatePlan
    {
        public string PlanId { get; }

        public RefToken PlanOwner { get; }
    }
}
