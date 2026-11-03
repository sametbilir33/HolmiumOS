using System;
using Cosmos.Kernel.HAL.Devices.Storage;

namespace HolmiumOS.Storage
{
    public sealed class MemoryBlockDevice : IBlockDevice
    {
        private readonly byte[] storage;

        public string Name { get; }
        public ulong BlockSize { get; }
        public ulong BlockCount { get; }

        public MemoryBlockDevice(
            string name,
            ulong blockSize,
            ulong blockCount)
        {
            if (blockSize == 0 || blockCount == 0)
                throw new ArgumentOutOfRangeException();

            ulong totalSize = checked(blockSize * blockCount);

            if (totalSize > int.MaxValue)
                throw new ArgumentOutOfRangeException(
                    nameof(blockCount),
                    "RAM disk is too large.");

            Name = name;
            BlockSize = blockSize;
            BlockCount = blockCount;

            storage = new byte[(int)totalSize];
        }

        private (int Offset, int Length) GetRange(
            ulong blockNo,
            ulong blockCount)
        {
            if (blockNo > BlockCount ||
                blockCount > BlockCount - blockNo)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(blockNo));
            }

            ulong offset = checked(blockNo * BlockSize);
            ulong length = checked(blockCount * BlockSize);

            return ((int)offset, (int)length);
        }

        public void ReadBlock(
            ulong blockNo,
            ulong blockCount,
            Span<byte> data)
        {
            var range = GetRange(blockNo, blockCount);

            if (data.Length < range.Length)
                throw new ArgumentException(
                    "Destination buffer is too small.");

            storage.AsSpan(
                range.Offset,
                range.Length
            ).CopyTo(data);
        }

        public void WriteBlock(
            ulong blockNo,
            ulong blockCount,
            ReadOnlySpan<byte> data)
        {
            var range = GetRange(blockNo, blockCount);

            if (data.Length < range.Length)
                throw new ArgumentException(
                    "Source buffer is too small.");

            data.Slice(0, range.Length).CopyTo(
                storage.AsSpan(
                    range.Offset,
                    range.Length));
        }

        public void Flush()
        {
        }
    }
}