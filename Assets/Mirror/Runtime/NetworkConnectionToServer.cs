using System;
<<<<<<< Updated upstream
=======
using System.Runtime.CompilerServices;
>>>>>>> Stashed changes

namespace Mirror
{
    public class NetworkConnectionToServer : NetworkConnection
    {
        public override string address => "";

<<<<<<< Updated upstream
        internal override void Send(ArraySegment<byte> segment, int channelId = Channels.DefaultReliable)
        {
            // Debug.Log("ConnectionSend " + this + " bytes:" + BitConverter.ToString(segment.Array, segment.Offset, segment.Count));

            // validate packet size first.
            if (ValidatePacketSize(segment, channelId))
            {
                Transport.activeTransport.ClientSend(channelId, segment);
            }
        }

        /// <summary>
        /// Disconnects this connection.
        /// </summary>
=======
        // Send stage three: hand off to transport
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void SendToTransport(ArraySegment<byte> segment, int channelId = Channels.Reliable) =>
            Transport.activeTransport.ClientSend(segment, channelId);

        /// <summary>Disconnects this connection.</summary>
>>>>>>> Stashed changes
        public override void Disconnect()
        {
            // set not ready and handle clientscene disconnect in any case
            // (might be client or host mode here)
<<<<<<< Updated upstream
            isReady = false;
            ClientScene.HandleClientDisconnect(this);
=======
            // TODO remove redundant state. have one source of truth for .ready!
            isReady = false;
            NetworkClient.ready = false;
>>>>>>> Stashed changes
            Transport.activeTransport.ClientDisconnect();
        }
    }
}
