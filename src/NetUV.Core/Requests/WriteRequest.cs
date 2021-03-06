﻿// Copyright (c) Johnny Z. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NetUV.Core.Requests
{
    using NetUV.Core.Common;
    using NetUV.Core.Native;

    sealed class WriteRequest : WriteBufferRequest
    {
        readonly ThreadLocalPool.Handle recyclerHandle;

        internal WriteRequest(ThreadLocalPool.Handle recyclerHandle)
            : base(uv_req_type.UV_WRITE)
        {
            this.recyclerHandle = recyclerHandle;
        }

        protected override void Release()
        {
            base.Release();
            this.recyclerHandle.Release(this);
        }
    }
}
