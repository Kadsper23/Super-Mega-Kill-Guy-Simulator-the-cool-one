<<<<<<< Updated upstream
using System;

namespace Mirror
{
    /// <summary>
    /// NetworkReader to be used with <see cref="NetworkReaderPool">NetworkReaderPool</see>
    /// </summary>
    public sealed class PooledNetworkReader : NetworkReader, IDisposable
    {
        internal PooledNetworkReader(byte[] bytes) : base(bytes) {}
        internal PooledNetworkReader(ArraySegment<byte> segment) : base(segment) {}
        public void Dispose() => NetworkReaderPool.Recycle(this);
    }

    /// <summary>
    /// Pool of NetworkReaders
    /// <para>Use this pool instead of <see cref="NetworkReader">NetworkReader</see> to reduce memory allocation</para>
    /// </summary>
=======
// API consistent with Microsoft's ObjectPool<T>.
using System;
using System.Runtime.CompilerServices;

namespace Mirror
{
    /// <summary>Pool of NetworkReaders to avoid allocations.</summary>
>>>>>>> Stashed changes
    public static class NetworkReaderPool
    {
        // reuse Pool<T>
        // we still wrap it in NetworkReaderPool.Get/Recyle so we can reset the
        // position and array before reusing.
<<<<<<< Updated upstream
        static readonly Pool<PooledNetworkReader> Pool = new Pool<PooledNetworkReader>(
            // byte[] will be assigned in GetReader
            () => new PooledNetworkReader(new byte[]{})
        );

        /// <summary>
        /// Get the next reader in the pool
        /// <para>If pool is empty, creates a new Reader</para>
        /// </summary>
        public static PooledNetworkReader GetReader(byte[] bytes)
        {
            // grab from pool & set buffer
            PooledNetworkReader reader = Pool.Take();
            reader.buffer = new ArraySegment<byte>(bytes);
            reader.Position = 0;
            return reader;
        }

        /// <summary>
        /// Get the next reader in the pool
        /// <para>If pool is empty, creates a new Reader</para>
        /// </summary>
        public static PooledNetworkReader GetReader(ArraySegment<byte> segment)
        {
            // grab from pool & set buffer
            PooledNetworkReader reader = Pool.Take();
            reader.buffer = segment;
            reader.Position = 0;
            return reader;
        }

        /// <summary>
        /// Puts reader back into pool
        /// <para>When pool is full, the extra reader is left for the GC</para>
        /// </summary>
        public static void Recycle(PooledNetworkReader reader)
=======
        static readonly Pool<NetworkReaderPooled> Pool = new Pool<NetworkReaderPooled>(
            // byte[] will be assigned in GetReader
            () => new NetworkReaderPooled(new byte[]{}),
            // initial capacity to avoid allocations in the first few frames
            1000
        );

        // DEPRECATED 2022-03-10
        [Obsolete("GetReader() was renamed to Get()")]
        public static NetworkReaderPooled GetReader(byte[] bytes) => Get(bytes);

        /// <summary>Get the next reader in the pool. If pool is empty, creates a new Reader</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NetworkReaderPooled Get(byte[] bytes)
        {
            // grab from pool & set buffer
            NetworkReaderPooled reader = Pool.Get();
            reader.SetBuffer(bytes);
            return reader;
        }

        // DEPRECATED 2022-03-10
        [Obsolete("GetReader() was renamed to Get()")]
        public static NetworkReaderPooled GetReader(ArraySegment<byte> segment) => Get(segment);

        /// <summary>Get the next reader in the pool. If pool is empty, creates a new Reader</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NetworkReaderPooled Get(ArraySegment<byte> segment)
        {
            // grab from pool & set buffer
            NetworkReaderPooled reader = Pool.Get();
            reader.SetBuffer(segment);
            return reader;
        }

        // DEPRECATED 2022-03-10
        [Obsolete("Recycle() was renamed to Return()")]
        public static void Recycle(NetworkReaderPooled reader) => Return(reader);

        /// <summary>Returns a reader to the pool.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Return(NetworkReaderPooled reader)
>>>>>>> Stashed changes
        {
            Pool.Return(reader);
        }
    }
}
