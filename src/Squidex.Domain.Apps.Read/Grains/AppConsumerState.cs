// ==========================================================================
//  AppConsumerState.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

namespace Squidex.Domain.Apps.Read.Grains
{
    public sealed class AppConsumerState
    {
        public string Position { get; set; }

        public string Error { get; set; }

        public bool IsRunning { get; set; } = true;
    }
}
