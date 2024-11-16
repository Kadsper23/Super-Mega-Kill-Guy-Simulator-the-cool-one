using System;
<<<<<<< Updated upstream
=======
using System.Runtime.CompilerServices;
>>>>>>> Stashed changes
using UnityEngine;

namespace Mirror
{
    // message packing all in one place, instead of constructing headers in all
    // kinds of different places
    //
<<<<<<< Updated upstream
    //   MsgType     (1-n bytes)
    //   Content     (ContentSize bytes)
    //
    // -> we use varint for headers because most messages will result in 1 byte
    //    type/size headers then instead of always
    //    using 2 bytes for shorts.
    // -> this reduces bandwidth by 10% if average message size is 20 bytes
    //    (probably even shorter)
    public static class MessagePacking
    {
        /// <summary>
        /// this is the minimum size of a message that mirror will accept
        /// </summary>
        internal const int HeaderSize = sizeof(ushort);

        public static int GetId<T>() where T : struct, NetworkMessage
        {
            // paul: 16 bits is enough to avoid collisions
            //  - keeps the message size small because it gets varinted
            //  - in case of collisions,  Mirror will display an error
            return typeof(T).FullName.GetStableHashCode() & 0xFFFF;
        }

        // pack message before sending
        // -> NetworkWriter passed as arg so that we can use .ToArraySegment
        //    and do an allocation free send before recycling it.
        public static void Pack<T>(T message, NetworkWriter writer)
            where T : struct, NetworkMessage
        {
            int msgType = GetId<T>();
            writer.WriteUInt16((ushort)msgType);
=======
    //   MsgType     (2 bytes)
    //   Content     (ContentSize bytes)
    public static class MessagePacking
    {
        // message header size
        public const int HeaderSize = sizeof(ushort);

        // max message content size (without header) calculation for convenience
        // -> Transport.GetMaxPacketSize is the raw maximum
        // -> Every message gets serialized into <<id, content>>
        // -> Every serialized message get put into a batch with a header
        public static int MaxContentSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Transport.activeTransport.GetMaxPacketSize()
                   - HeaderSize
                   - Batcher.HeaderSize;
        }

        // paul: 16 bits is enough to avoid collisions
        //  - keeps the message size small
        //  - in case of collisions,  Mirror will display an error
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort GetId<T>() where T : struct, NetworkMessage =>
            (ushort)(typeof(T).FullName.GetStableHashCode() & 0xFFFF);

        // pack message before sending
        // -> NetworkWriter passed as arg so that we can use .ToArraySegment
        //    and do an allocation free send before recycling it.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Pack<T>(T message, NetworkWriter writer)
            where T : struct, NetworkMessage
        {
            ushort msgType = GetId<T>();
            writer.WriteUShort(msgType);
>>>>>>> Stashed changes

            // serialize message into writer
            writer.Write(message);
        }

        // unpack message after receiving
        // -> pass NetworkReader so it's less strange if we create it in here
        //    and pass it upwards.
        // -> NetworkReader will point at content afterwards!
<<<<<<< Updated upstream
        public static bool Unpack(NetworkReader messageReader, out int msgType)
        {
            // read message type (varint)
            try
            {
                msgType = messageReader.ReadUInt16();
=======
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Unpack(NetworkReader messageReader, out ushort msgType)
        {
            // read message type
            try
            {
                msgType = messageReader.ReadUShort();
>>>>>>> Stashed changes
                return true;
            }
            catch (System.IO.EndOfStreamException)
            {
                msgType = 0;
                return false;
            }
        }

<<<<<<< Updated upstream
        [Obsolete("MessagePacker.UnpackMessage was renamed to Unpack for consistency with Pack.")]
        public static bool UnpackMessage(NetworkReader messageReader, out int msgType) =>
            Unpack(messageReader, out msgType);

        internal static NetworkMessageDelegate WrapHandler<T, C>(Action<C, T> handler, bool requireAuthentication)
=======
        // version for handlers with channelId
        // inline! only exists for 20-30 messages and they call it all the time.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static NetworkMessageDelegate WrapHandler<T, C>(Action<C, T, int> handler, bool requireAuthentication)
>>>>>>> Stashed changes
            where T : struct, NetworkMessage
            where C : NetworkConnection
            => (conn, reader, channelId) =>
        {
            // protect against DOS attacks if attackers try to send invalid
            // data packets to crash the server/client. there are a thousand
            // ways to cause an exception in data handling:
            // - invalid headers
            // - invalid message ids
            // - invalid data causing exceptions
            // - negative ReadBytesAndSize prefixes
            // - invalid utf8 strings
            // - etc.
            //
            // let's catch them all and then disconnect that connection to avoid
            // further attacks.
            T message = default;
<<<<<<< Updated upstream
=======
            // record start position for NetworkDiagnostics because reader might contain multiple messages if using batching
            int startPos = reader.Position;
>>>>>>> Stashed changes
            try
            {
                if (requireAuthentication && !conn.isAuthenticated)
                {
                    // message requires authentication, but the connection was not authenticated
                    Debug.LogWarning($"Closing connection: {conn}. Received message {typeof(T)} that required authentication, but the user has not authenticated yet");
                    conn.Disconnect();
                    return;
                }

                //Debug.Log($"ConnectionRecv {conn} msgType:{typeof(T)} content:{BitConverter.ToString(reader.buffer.Array, reader.buffer.Offset, reader.buffer.Count)}");

                // if it is a value type, just use default(T)
                // otherwise allocate a new instance
                message = reader.Read<T>();
            }
            catch (Exception exception)
            {
                Debug.LogError($"Closed connection: {conn}. This can happen if the other side accidentally (or an attacker intentionally) sent invalid data. Reason: {exception}");
                conn.Disconnect();
                return;
            }
            finally
            {
<<<<<<< Updated upstream
                // TODO: Figure out the correct channel
                NetworkDiagnostics.OnReceive(message, channelId, reader.Length);
=======
                int endPos = reader.Position;
                // TODO: Figure out the correct channel
                NetworkDiagnostics.OnReceive(message, channelId, endPos - startPos);
>>>>>>> Stashed changes
            }

            // user handler exception should not stop the whole server
            try
            {
                // user implemented handler
<<<<<<< Updated upstream
                handler((C)conn, message);
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception in MessageHandler: {e.GetType().Name} {e.Message}\n{e.StackTrace}");
                conn.Disconnect();
            }
        };
=======
                handler((C)conn, message, channelId);
            }
            catch (Exception e)
            {
                Debug.LogError($"Disconnecting connId={conn.connectionId} to prevent exploits from an Exception in MessageHandler: {e.GetType().Name} {e.Message}\n{e.StackTrace}");
                conn.Disconnect();
            }
        };

        // version for handlers without channelId
        // TODO obsolete this some day to always use the channelId version.
        //      all handlers in this version are wrapped with 1 extra action.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static NetworkMessageDelegate WrapHandler<T, C>(Action<C, T> handler, bool requireAuthentication)
            where T : struct, NetworkMessage
            where C : NetworkConnection
        {
            // wrap action as channelId version, call original
            void Wrapped(C conn, T msg, int _) => handler(conn, msg);
            return WrapHandler((Action<C, T, int>) Wrapped, requireAuthentication);
        }
>>>>>>> Stashed changes
    }
}
