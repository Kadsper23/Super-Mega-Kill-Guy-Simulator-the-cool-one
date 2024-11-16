using System;
<<<<<<< Updated upstream
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
=======
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
>>>>>>> Stashed changes
using UnityEngine;

namespace Mirror
{
<<<<<<< Updated upstream
    /// <summary>
    /// A class that holds writers for the different types
    /// <para>Note that c# creates a different static variable for each type</para>
    /// <para>This will be populated by the weaver</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class Writer<T>
    {
        public static Action<NetworkWriter, T> write;
    }

    /// <summary>
    /// Binary stream Writer. Supports simple types, buffers, arrays, structs, and nested types
    /// <para>Use <see cref="NetworkWriterPool.GetWriter">NetworkWriter.GetWriter</see> to reduce memory allocation</para>
    /// </summary>
=======
    /// <summary>Network Writer for most simple types like floats, ints, buffers, structs, etc. Use NetworkWriterPool.GetReader() to avoid allocations.</summary>
>>>>>>> Stashed changes
    public class NetworkWriter
    {
        public const int MaxStringLength = 1024 * 32;

        // create writer immediately with it's own buffer so no one can mess with it and so that we can resize it.
        // note: BinaryWriter allocates too much, so we only use a MemoryStream
        // => 1500 bytes by default because on average, most packets will be <= MTU
        byte[] buffer = new byte[1500];

<<<<<<< Updated upstream
        // 'int' is the best type for .Position. 'short' is too small if we send >32kb which would result in negative .Position
        // -> converting long to int is fine until 2GB of data (MAX_INT), so we don't have to worry about overflows here
        int position;
        int length;

        /// <summary>
        /// Number of bytes writen to the buffer
        /// </summary>
        public int Length => length;

        /// <summary>
        /// Next position to write to the buffer
        /// </summary>
        public int Position
        {
            get => position;
            set
            {
                position = value;
                EnsureLength(value);
            }
        }

        /// <summary>
        /// Reset both the position and length of the stream
        /// </summary>
        /// <remarks>
        /// Leaves the capacity the same so that we can reuse this writer without extra allocations
        /// </remarks>
        public void Reset()
        {
            position = 0;
            length = 0;
        }

        /// <summary>
        /// Sets length, moves position if it is greater than new length
        /// </summary>
        /// <param name="newLength"></param>
        /// <remarks>
        /// Zeros out any extra length created by setlength
        /// </remarks>
        public void SetLength(int newLength)
        {
            int oldLength = length;

            // ensure length & capacity
            EnsureLength(newLength);

            // zero out new length
            if (oldLength < newLength)
            {
                Array.Clear(buffer, oldLength, newLength - oldLength);
            }

            length = newLength;
            position = Mathf.Min(position, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void EnsureLength(int value)
        {
            if (length < value)
            {
                length = value;
                EnsureCapacity(value);
            }
        }

=======
        /// <summary>Next position to write to the buffer</summary>
        public int Position;

        /// <summary>Reset both the position and length of the stream</summary>
        // Leaves the capacity the same so that we can reuse this writer without
        // extra allocations
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            Position = 0;
        }

        // NOTE that our runtime resizing comes at no extra cost because:
        // 1. 'has space' checks are necessary even for fixed sized writers.
        // 2. all writers will eventually be large enough to stop resizing.
>>>>>>> Stashed changes
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void EnsureCapacity(int value)
        {
            if (buffer.Length < value)
            {
                int capacity = Math.Max(value, buffer.Length * 2);
                Array.Resize(ref buffer, capacity);
            }
        }

<<<<<<< Updated upstream
        /// <summary>
        /// Copies buffer to new array with the size of <see cref="Length"/>
        /// <para></para>
        /// </summary>
        /// <returns>all the data we have written, regardless of the current position</returns>
        public byte[] ToArray()
        {
            byte[] data = new byte[length];
            Array.ConstrainedCopy(buffer, 0, data, 0, length);
            return data;
        }

        /// <summary>
        /// Create an ArraySegment using the buffer and <see cref="Length"/>
        /// <para>
        ///     Don't modify the NetworkWriter while using the ArraySegment as this can overwrite the bytes
        /// </para>
        /// <para>
        ///     Use ToArraySegment instead of ToArray to avoid allocations
        /// </para>
        /// </summary>
        /// <returns>all the data we have written, regardless of the current position</returns>
        public ArraySegment<byte> ToArraySegment()
        {
            return new ArraySegment<byte>(buffer, 0, length);
        }

        public void WriteByte(byte value)
        {
            EnsureLength(position + 1);
            buffer[position++] = value;
        }

=======
        /// <summary>Copies buffer until 'Position' to a new array.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ToArray()
        {
            byte[] data = new byte[Position];
            Array.ConstrainedCopy(buffer, 0, data, 0, Position);
            return data;
        }

        /// <summary>Returns allocation-free ArraySegment until 'Position'.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ArraySegment<byte> ToArraySegment()
        {
            return new ArraySegment<byte>(buffer, 0, Position);
        }

        // WriteBlittable<T> from DOTSNET.
        // this is extremely fast, but only works for blittable types.
        //
        // Benchmark:
        //   WriteQuaternion x 100k, Macbook Pro 2015 @ 2.2Ghz, Unity 2018 LTS (debug mode)
        //
        //                | Median |  Min  |  Max  |  Avg  |  Std  | (ms)
        //     before     |  30.35 | 29.86 | 48.99 | 32.54 |  4.93 |
        //     blittable* |   5.69 |  5.52 | 27.51 |  7.78 |  5.65 |
        //
        //     * without IsBlittable check
        //     => 4-6x faster!
        //
        //   WriteQuaternion x 100k, Macbook Pro 2015 @ 2.2Ghz, Unity 2020.1 (release mode)
        //
        //                | Median |  Min  |  Max  |  Avg  |  Std  | (ms)
        //     before     |   9.41 |  8.90 | 23.02 | 10.72 |  3.07 |
        //     blittable* |   1.48 |  1.40 | 16.03 |  2.60 |  2.71 |
        //
        //     * without IsBlittable check
        //     => 6x faster!
        //
        // Note:
        //   WriteBlittable assumes same endianness for server & client.
        //   All Unity 2018+ platforms are little endian.
        //   => run NetworkWriterTests.BlittableOnThisPlatform() to verify!
        //
        // Note: inlining WriteBlittable is enough. don't inline WriteInt etc.
        //       we don't want WriteBlittable to be copied in place everywhere.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe void WriteBlittable<T>(T value)
            where T : unmanaged
        {
            // check if blittable for safety
#if UNITY_EDITOR
            if (!UnsafeUtility.IsBlittable(typeof(T)))
            {
                Debug.LogError($"{typeof(T)} is not blittable!");
                return;
            }
#endif
            // calculate size
            //   sizeof(T) gets the managed size at compile time.
            //   Marshal.SizeOf<T> gets the unmanaged size at runtime (slow).
            // => our 1mio writes benchmark is 6x slower with Marshal.SizeOf<T>
            // => for blittable types, sizeof(T) is even recommended:
            // https://docs.microsoft.com/en-us/dotnet/standard/native-interop/best-practices
            int size = sizeof(T);

            // ensure capacity
            // NOTE that our runtime resizing comes at no extra cost because:
            // 1. 'has space' checks are necessary even for fixed sized writers.
            // 2. all writers will eventually be large enough to stop resizing.
            EnsureCapacity(Position + size);

            // write blittable
            fixed (byte* ptr = &buffer[Position])
            {
#if UNITY_ANDROID
                // on some android systems, assigning *(T*)ptr throws a NRE if
                // the ptr isn't aligned (i.e. if Position is 1,2,3,5, etc.).
                // here we have to use memcpy.
                //
                // => we can't get a pointer of a struct in C# without
                //    marshalling allocations
                // => instead, we stack allocate an array of type T and use that
                // => stackalloc avoids GC and is very fast. it only works for
                //    value types, but all blittable types are anyway.
                //
                // this way, we can still support blittable reads on android.
                // see also: https://github.com/vis2k/Mirror/issues/3044
                // (solution discovered by AIIO, FakeByte, mischa)
                T* valueBuffer = stackalloc T[1]{value};
                UnsafeUtility.MemCpy(ptr, valueBuffer, size);
#else
                // cast buffer to T* pointer, then assign value to the area
                *(T*)ptr = value;
#endif
            }
            Position += size;
        }

        // blittable'?' template for code reuse
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteBlittableNullable<T>(T? value)
            where T : unmanaged
        {
            // bool isn't blittable. write as byte.
            WriteByte((byte)(value.HasValue ? 0x01 : 0x00));

            // only write value if exists. saves bandwidth.
            if (value.HasValue)
                WriteBlittable(value.Value);
        }

        public void WriteByte(byte value) => WriteBlittable(value);

>>>>>>> Stashed changes
        // for byte arrays with consistent size, where the reader knows how many to read
        // (like a packet opcode that's always the same)
        public void WriteBytes(byte[] buffer, int offset, int count)
        {
<<<<<<< Updated upstream
            EnsureLength(position + count);
            Array.ConstrainedCopy(buffer, offset, this.buffer, position, count);
            position += count;
        }

        /// <summary>
        /// Writes any type that mirror supports
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
=======
            EnsureCapacity(Position + count);
            Array.ConstrainedCopy(buffer, offset, this.buffer, Position, count);
            Position += count;
        }

        /// <summary>Writes any type that mirror supports. Uses weaver populated Writer(T).write.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
>>>>>>> Stashed changes
        public void Write<T>(T value)
        {
            Action<NetworkWriter, T> writeDelegate = Writer<T>.write;
            if (writeDelegate == null)
            {
<<<<<<< Updated upstream
                Debug.LogError($"No writer found for {typeof(T)}. Use a type supported by Mirror or define a custom writer");
=======
                Debug.LogError($"No writer found for {typeof(T)}. This happens either if you are missing a NetworkWriter extension for your custom type, or if weaving failed. Try to reimport a script to weave again.");
>>>>>>> Stashed changes
            }
            else
            {
                writeDelegate(this, value);
            }
        }
    }

<<<<<<< Updated upstream

    // Mirror's Weaver automatically detects all NetworkWriter function types,
    // but they do all need to be extensions.
    public static class NetworkWriterExtensions
    {
        // cache encoding instead of creating it with BinaryWriter each time
        // 1000 readers before:  1MB GC, 30ms
        // 1000 readers after: 0.8MB GC, 18ms
        static readonly UTF8Encoding encoding = new UTF8Encoding(false, true);
        static readonly byte[] stringBuffer = new byte[NetworkWriter.MaxStringLength];

        public static void WriteByte(this NetworkWriter writer, byte value) => writer.WriteByte(value);

        public static void WriteSByte(this NetworkWriter writer, sbyte value) => writer.WriteByte((byte)value);

        public static void WriteChar(this NetworkWriter writer, char value) => writer.WriteUInt16(value);

        public static void WriteBoolean(this NetworkWriter writer, bool value) => writer.WriteByte((byte)(value ? 1 : 0));

        public static void WriteUInt16(this NetworkWriter writer, ushort value)
        {
            writer.WriteByte((byte)value);
            writer.WriteByte((byte)(value >> 8));
        }

        public static void WriteInt16(this NetworkWriter writer, short value) => writer.WriteUInt16((ushort)value);

        public static void WriteUInt32(this NetworkWriter writer, uint value)
        {
            writer.WriteByte((byte)value);
            writer.WriteByte((byte)(value >> 8));
            writer.WriteByte((byte)(value >> 16));
            writer.WriteByte((byte)(value >> 24));
        }

        public static void WriteInt32(this NetworkWriter writer, int value) => writer.WriteUInt32((uint)value);

        public static void WriteUInt64(this NetworkWriter writer, ulong value)
        {
            writer.WriteByte((byte)value);
            writer.WriteByte((byte)(value >> 8));
            writer.WriteByte((byte)(value >> 16));
            writer.WriteByte((byte)(value >> 24));
            writer.WriteByte((byte)(value >> 32));
            writer.WriteByte((byte)(value >> 40));
            writer.WriteByte((byte)(value >> 48));
            writer.WriteByte((byte)(value >> 56));
        }

        public static void WriteInt64(this NetworkWriter writer, long value) => writer.WriteUInt64((ulong)value);

        public static void WriteSingle(this NetworkWriter writer, float value)
        {
            UIntFloat converter = new UIntFloat
            {
                floatValue = value
            };
            writer.WriteUInt32(converter.intValue);
        }

        public static void WriteDouble(this NetworkWriter writer, double value)
        {
            UIntDouble converter = new UIntDouble
            {
                doubleValue = value
            };
            writer.WriteUInt64(converter.longValue);
        }

        public static void WriteDecimal(this NetworkWriter writer, decimal value)
        {
            // the only way to read it without allocations is to both read and
            // write it with the FloatConverter (which is not binary compatible
            // to writer.Write(decimal), hence why we use it here too)
            UIntDecimal converter = new UIntDecimal
            {
                decimalValue = value
            };
            writer.WriteUInt64(converter.longValue1);
            writer.WriteUInt64(converter.longValue2);
        }

        public static void WriteString(this NetworkWriter writer, string value)
        {
            // write 0 for null support, increment real size by 1
            // (note: original HLAPI would write "" for null strings, but if a
            //        string is null on the server then it should also be null
            //        on the client)
            if (value == null)
            {
                writer.WriteUInt16(0);
                return;
            }

            // write string with same method as NetworkReader
            // convert to byte[]
            int size = encoding.GetBytes(value, 0, value.Length, stringBuffer, 0);

            // check if within max size
            if (size >= NetworkWriter.MaxStringLength)
            {
                throw new IndexOutOfRangeException("NetworkWriter.Write(string) too long: " + size + ". Limit: " + NetworkWriter.MaxStringLength);
            }

            // write size and bytes
            writer.WriteUInt16(checked((ushort)(size + 1)));
            writer.WriteBytes(stringBuffer, 0, size);
        }

        // for byte arrays with dynamic size, where the reader doesn't know how many will come
        // (like an inventory with different items etc.)
        public static void WriteBytesAndSize(this NetworkWriter writer, byte[] buffer, int offset, int count)
        {
            // null is supported because [SyncVar]s might be structs with null byte[] arrays
            // write 0 for null array, increment normal size by 1 to save bandwidth
            // (using size=-1 for null would limit max size to 32kb instead of 64kb)
            if (buffer == null)
            {
                writer.WriteUInt32(0u);
                return;
            }
            writer.WriteUInt32(checked((uint)count) + 1u);
            writer.WriteBytes(buffer, offset, count);
        }

        // Weaver needs a write function with just one byte[] parameter
        // (we don't name it .Write(byte[]) because it's really a WriteBytesAndSize since we write size / null info too)
        public static void WriteBytesAndSize(this NetworkWriter writer, byte[] buffer)
        {
            // buffer might be null, so we can't use .Length in that case
            writer.WriteBytesAndSize(buffer, 0, buffer != null ? buffer.Length : 0);
        }

        public static void WriteBytesAndSizeSegment(this NetworkWriter writer, ArraySegment<byte> buffer)
        {
            writer.WriteBytesAndSize(buffer.Array, buffer.Offset, buffer.Count);
        }

        public static void WriteVector2(this NetworkWriter writer, Vector2 value)
        {
            writer.WriteSingle(value.x);
            writer.WriteSingle(value.y);
        }

        public static void WriteVector3(this NetworkWriter writer, Vector3 value)
        {
            writer.WriteSingle(value.x);
            writer.WriteSingle(value.y);
            writer.WriteSingle(value.z);
        }

        public static void WriteVector4(this NetworkWriter writer, Vector4 value)
        {
            writer.WriteSingle(value.x);
            writer.WriteSingle(value.y);
            writer.WriteSingle(value.z);
            writer.WriteSingle(value.w);
        }

        public static void WriteVector2Int(this NetworkWriter writer, Vector2Int value)
        {
            writer.WriteInt32(value.x);
            writer.WriteInt32(value.y);
        }

        public static void WriteVector3Int(this NetworkWriter writer, Vector3Int value)
        {
            writer.WriteInt32(value.x);
            writer.WriteInt32(value.y);
            writer.WriteInt32(value.z);
        }

        public static void WriteColor(this NetworkWriter writer, Color value)
        {
            writer.WriteSingle(value.r);
            writer.WriteSingle(value.g);
            writer.WriteSingle(value.b);
            writer.WriteSingle(value.a);
        }

        public static void WriteColor32(this NetworkWriter writer, Color32 value)
        {
            writer.WriteByte(value.r);
            writer.WriteByte(value.g);
            writer.WriteByte(value.b);
            writer.WriteByte(value.a);
        }

        public static void WriteQuaternion(this NetworkWriter writer, Quaternion value)
        {
            writer.WriteSingle(value.x);
            writer.WriteSingle(value.y);
            writer.WriteSingle(value.z);
            writer.WriteSingle(value.w);
        }

        public static void WriteRect(this NetworkWriter writer, Rect value)
        {
            writer.WriteSingle(value.xMin);
            writer.WriteSingle(value.yMin);
            writer.WriteSingle(value.width);
            writer.WriteSingle(value.height);
        }

        public static void WritePlane(this NetworkWriter writer, Plane value)
        {
            writer.WriteVector3(value.normal);
            writer.WriteSingle(value.distance);
        }

        public static void WriteRay(this NetworkWriter writer, Ray value)
        {
            writer.WriteVector3(value.origin);
            writer.WriteVector3(value.direction);
        }

        public static void WriteMatrix4x4(this NetworkWriter writer, Matrix4x4 value)
        {
            writer.WriteSingle(value.m00);
            writer.WriteSingle(value.m01);
            writer.WriteSingle(value.m02);
            writer.WriteSingle(value.m03);
            writer.WriteSingle(value.m10);
            writer.WriteSingle(value.m11);
            writer.WriteSingle(value.m12);
            writer.WriteSingle(value.m13);
            writer.WriteSingle(value.m20);
            writer.WriteSingle(value.m21);
            writer.WriteSingle(value.m22);
            writer.WriteSingle(value.m23);
            writer.WriteSingle(value.m30);
            writer.WriteSingle(value.m31);
            writer.WriteSingle(value.m32);
            writer.WriteSingle(value.m33);
        }

        public static void WriteGuid(this NetworkWriter writer, Guid value)
        {
            byte[] data = value.ToByteArray();
            writer.WriteBytes(data, 0, data.Length);
        }

        public static void WriteNetworkIdentity(this NetworkWriter writer, NetworkIdentity value)
        {
            if (value == null)
            {
                writer.WriteUInt32(0);
                return;
            }
            writer.WriteUInt32(value.netId);
        }

        public static void WriteNetworkBehaviour(this NetworkWriter writer, NetworkBehaviour value)
        {
            if (value == null)
            {
                writer.WriteUInt32(0);
                return;
            }
            writer.WriteUInt32(value.netId);
            writer.WriteByte((byte)value.ComponentIndex);
        }

        public static void WriteTransform(this NetworkWriter writer, Transform value)
        {
            if (value == null)
            {
                writer.WriteUInt32(0);
                return;
            }
            NetworkIdentity identity = value.GetComponent<NetworkIdentity>();
            if (identity != null)
            {
                writer.WriteUInt32(identity.netId);
            }
            else
            {
                Debug.LogWarning("NetworkWriter " + value + " has no NetworkIdentity");
                writer.WriteUInt32(0);
            }
        }

        public static void WriteGameObject(this NetworkWriter writer, GameObject value)
        {
            if (value == null)
            {
                writer.WriteUInt32(0);
                return;
            }
            NetworkIdentity identity = value.GetComponent<NetworkIdentity>();
            if (identity != null)
            {
                writer.WriteUInt32(identity.netId);
            }
            else
            {
                Debug.LogWarning("NetworkWriter " + value + " has no NetworkIdentity");
                writer.WriteUInt32(0);
            }
        }

        public static void WriteUri(this NetworkWriter writer, Uri uri)
        {
            writer.WriteString(uri.ToString());
        }

        public static void WriteList<T>(this NetworkWriter writer, List<T> list)
        {
            if (list is null)
            {
                writer.WriteInt32(-1);
                return;
            }
            writer.WriteInt32(list.Count);
            for (int i = 0; i < list.Count; i++)
                writer.Write(list[i]);
        }

        public static void WriteArray<T>(this NetworkWriter writer, T[] array)
        {
            if (array is null)
            {
                writer.WriteInt32(-1);
                return;
            }
            writer.WriteInt32(array.Length);
            for (int i = 0; i < array.Length; i++)
                writer.Write(array[i]);
        }

        public static void WriteArraySegment<T>(this NetworkWriter writer, ArraySegment<T> segment)
        {
            int length = segment.Count;
            writer.WriteInt32(length);
            for (int i = 0; i < length; i++)
            {
                writer.Write(segment.Array[segment.Offset + i]);
            }
        }
=======
    /// <summary>Helper class that weaver populates with all writer types.</summary>
    // Note that c# creates a different static variable for each type
    // -> Weaver.ReaderWriterProcessor.InitializeReaderAndWriters() populates it
    public static class Writer<T>
    {
        public static Action<NetworkWriter, T> write;
>>>>>>> Stashed changes
    }
}
