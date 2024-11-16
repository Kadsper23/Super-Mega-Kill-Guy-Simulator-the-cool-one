<<<<<<< Updated upstream
using System;

namespace Mirror
{
    /// <summary>
    /// NetworkWriter to be used with <see cref="NetworkWriterPool">NetworkWriterPool</see>
    /// </summary>
    public sealed class PooledNetworkWriter : NetworkWriter, IDisposable
    {
        public void Dispose() => NetworkWriterPool.Recycle(this);
    }

    /// <summary>
    /// Pool of NetworkWriters
    /// <para>Use this pool instead of <see cref="NetworkWriter">NetworkWriter</see> to reduce memory allocation</para>
    /// </summary>
=======
// API consistent with Microsoft's ObjectPool<T>.
using System;
using System.Runtime.CompilerServices;

namespace Mirror
{
    /// <summary>Pool of NetworkWriters to avoid allocations.</summary>
>>>>>>> Stashed changes
    public static class NetworkWriterPool
    {
        // reuse Pool<T>
        // we still wrap it in NetworkWriterPool.Get/Recycle so we can reset the
        // position before reusing.
        // this is also more consistent with NetworkReaderPool where we need to
        // assign the internal buffer before reusing.
<<<<<<< Updated upstream
        static readonly Pool<PooledNetworkWriter> Pool = new Pool<PooledNetworkWriter>(
            () => new PooledNetworkWriter()
        );

        /// <summary>
        /// Get the next writer in the pool
        /// <para>If pool is empty, creates a new Writer</para>
        /// </summary>
        public static PooledNetworkWriter GetWriter()
        {
            // grab from pool & reset position
            PooledNetworkWriter writer = Pool.Take();
=======
        static readonly Pool<NetworkWriterPooled> Pool = new Pool<NetworkWriterPooled>(
            () => new NetworkWriterPooled(),
            // initial capacity to avoid allocations in the first few frames
            // 1000 * 1200 bytes = around 1 MB.
            1000
        );

        // DEPRECATED 2022-03-10
        [Obsolete("GetWriter() was renamed to Get()")]
        public static NetworkWriterPooled GetWriter() => Get();

        /// <summary>Get a writer from the pool. Creates new one if pool is empty.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NetworkWriterPooled Get()
        {
            // grab from pool & reset position
            NetworkWriterPooled writer = Pool.Get();
>>>>>>> Stashed changes
            writer.Reset();
            return writer;
        }

<<<<<<< Updated upstream
        /// <summary>
        /// Puts writer back into pool
        /// <para>When pool is full, the extra writer is left for the GC</para>
        /// </summary>
        public static void Recycle(PooledNetworkWriter writer)
=======
        // DEPRECATED 2022-03-10
        [Obsolete("Recycle() was renamed to Return()")]
        public static void Recycle(NetworkWriterPooled writer) => Return(writer);

        /// <summary>Return a writer to the pool.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Return(NetworkWriterPooled writer)
>>>>>>> Stashed changes
        {
            Pool.Return(writer);
        }
    }
}
