// ==========================================================================
//  AppConsumerInfo.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;

namespace Squidex.Domain.Apps.Read.Grains
{
    public sealed class AppConsumerInfo
    {
        public Exception Exception { get; set; }

        public bool IsRunning { get; set; }
    }
}
