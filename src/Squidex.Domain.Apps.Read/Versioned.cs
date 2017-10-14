// ==========================================================================
//  Versioned.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

namespace Squidex.Domain.Apps.Read
{
    public sealed class Versioned<T>
    {
        public T Payload { get; }

        public long Version { get; }

        public Versioned(T payload, long version)
        {
            Payload = payload;
            Version = version;
        }

        public static implicit operator T(Versioned<T> versioned)
        {
            return versioned.Payload;
        }
    }
}
