﻿// ==========================================================================
//  IUserPictureStore.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System.IO;
using System.Threading.Tasks;

namespace Squidex.Domain.Users
{
    public interface IUserPictureStore
    {
        Task UploadAsync(string userId, Stream stream);

        Task<Stream> DownloadAsync(string userId);
    }
}
