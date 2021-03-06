﻿// Copyright (c) Johnny Z. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NetUV.Core.Tests.Buffers
{
    using System;
    using NetUV.Core.Buffers;
    using Xunit;

    public abstract class AbstractByteBufferAllocatorTests : ByteBufferAllocatorTests
    {
        internal abstract IByteBufferAllocator NewUnpooledAllocator();

        protected sealed override int DefaultMaxCapacity => AbstractByteBufferAllocator.DefaultMaxCapacity;

        protected sealed override int DefaultMaxComponents => AbstractByteBufferAllocator.DefaultMaxComponents;

        [Fact]
        public void CalculateNewCapacity()
        {
            IByteBufferAllocator allocator = this.NewAllocator();
            Assert.Equal(8, allocator.CalculateNewCapacity(1, 8));
            Assert.Equal(7, allocator.CalculateNewCapacity(1, 7));
            Assert.Equal(64, allocator.CalculateNewCapacity(1, 129));

            Assert.Throws<ArgumentOutOfRangeException>(() => allocator.CalculateNewCapacity(8, 7));
            Assert.Throws<ArgumentOutOfRangeException>(() => allocator.CalculateNewCapacity(-1, 8));
        }

        internal static void AssertInstanceOf<T>(T buffer) where T : IByteBuffer
        {
            Assert.IsAssignableFrom<T>(buffer is SimpleLeakAwareByteBuffer ? buffer.Unwrap() : buffer);
        }

        [Fact]
        public void UsedHeapMemory()
        {
            IByteBufferAllocator allocator = this.NewAllocator();
            IByteBufferAllocatorMetric metric = ((IByteBufferAllocatorMetricProvider)allocator).Metric;

            Assert.Equal(0, metric.UsedHeapMemory);
            IByteBuffer buffer = allocator.HeapBuffer(1024, 4096);
            int capacity = buffer.Capacity;
            Assert.Equal(this.ExpectedUsedMemory(allocator, capacity), metric.UsedHeapMemory);

            // Double the size of the buffer
            buffer.AdjustCapacity(capacity << 1);
            capacity = buffer.Capacity;
            Assert.Equal(this.ExpectedUsedMemory(allocator, capacity), metric.UsedHeapMemory);

            buffer.Release();
            Assert.Equal(this.ExpectedUsedMemoryAfterRelease(allocator, capacity), metric.UsedHeapMemory);
        }

        internal virtual long ExpectedUsedMemory(IByteBufferAllocator allocator, int capacity) => capacity;

        internal virtual long ExpectedUsedMemoryAfterRelease(IByteBufferAllocator allocator, int capacity) => 0;
    }
}
 