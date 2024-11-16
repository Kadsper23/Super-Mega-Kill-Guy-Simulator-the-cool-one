using System;
using System.Collections.Generic;
<<<<<<< Updated upstream
=======
using System.Runtime.CompilerServices;
>>>>>>> Stashed changes

namespace Mirror
{
    public class NetworkConnectionToClient : NetworkConnection
    {
<<<<<<< Updated upstream
        public override string address => Transport.activeTransport.ServerGetClientAddress(connectionId);

        // batching from server to client.
        // fewer transport calls give us significantly better performance/scale.
        //
        // for a 64KB max message transport and 64 bytes/message on average, we
        // reduce transport calls by a factor of 1000.
        //
        // depending on the transport, this can give 10x performance.
        //
        // Dictionary<channelId, batch> because we have multiple channels.
        internal class Batch
        {
            // batched messages
            internal Queue<PooledNetworkWriter> messages = new Queue<PooledNetworkWriter>();

            // each channel's batch has its own lastSendTime.
            // (use NetworkTime for maximum precision over days)
            //
            // channel batches are full and flushed at different times. using
            // one global time wouldn't make sense.
            // -> we want to be able to reset a channels send time after Send()
            //    flushed it because full. global time wouldn't allow that, so
            //    we would often flush in Send() and then flush again in Update
            //    even though we just flushed in Send().
            // -> initialize with current NetworkTime so first update doesn't
            //    calculate elapsed via 'now - 0'
            internal double lastSendTime = NetworkTime.time;
        }
        Dictionary<int, Batch> batches = new Dictionary<int, Batch>();

        // batching is optional because due to mirror's non-optimal update order
        // it would increase latency.

        // batching is still optional until we improve mirror's update order.
        // right now it increases latency because:
        //   enabling batching flushes all state updates in same frame, but
        //   transport processes incoming messages afterwards so server would
        //   batch them until next frame's flush
        // => disable it for super fast paced games
        // => enable it for high scale / cpu heavy games
        bool batching;

        // batch interval is 0 by default, meaning that we send immediately.
        // (useful to run tests without waiting for intervals too)
        float batchInterval;

        public NetworkConnectionToClient(int networkConnectionId, bool batching, float batchInterval)
            : base(networkConnectionId)
        {
            this.batching = batching;
            this.batchInterval = batchInterval;
        }

        Batch GetBatchForChannelId(int channelId)
        {
            // get existing or create new writer for the channelId
            Batch batch;
            if (!batches.TryGetValue(channelId, out batch))
            {
                batch = new Batch();
                batches[channelId] = batch;
            }
            return batch;
        }

        // send a batch. internal so we can test it.
        internal void SendBatch(int channelId, Batch batch)
        {
            // get max batch size for this channel
            int max = Transport.activeTransport.GetMaxBatchSize(channelId);

            // we need a writer to merge queued messages into a batch
            using (PooledNetworkWriter writer = NetworkWriterPool.GetWriter())
            {
                // for each queued message
                while (batch.messages.Count > 0)
                {
                    // get it
                    PooledNetworkWriter message = batch.messages.Dequeue();
                    ArraySegment<byte> segment = message.ToArraySegment();

                    // IF adding to writer would end up >= MTU then we should
                    // flush first. the goal is to always flush < MTU packets.
                    //
                    // IMPORTANT: if writer is empty and segment is > MTU
                    //            (which can happen for large max sized message)
                    //            then we would send an empty previous writer.
                    //            => don't do that.
                    //            => only send if not empty.
                    if (writer.Position > 0 &&
                        writer.Position + segment.Count >= max)
                    {
                        // flush & reset writer
                        Transport.activeTransport.ServerSend(connectionId, channelId, writer.ToArraySegment());
                        writer.SetLength(0);
                    }

                    // now add to writer in any case
                    // -> WriteBytes instead of WriteSegment because the latter
                    //    would add a size header. we want to write directly.
                    //
                    // NOTE: it's very possible that we add > MTU to writer if
                    //       message size is > MTU.
                    //       which is fine. next iteration will just flush it.
                    writer.WriteBytes(segment.Array, segment.Offset, segment.Count);

                    // return queued message to pool
                    NetworkWriterPool.Recycle(message);
                }

                // done iterating queued messages.
                // batch might still contain the last message.
                // send it.
                if (writer.Position > 0)
                {
                    Transport.activeTransport.ServerSend(connectionId, channelId, writer.ToArraySegment());
                    writer.SetLength(0);
                }
            }

            // reset send time for this channel's batch
            batch.lastSendTime = NetworkTime.time;
        }

        internal override void Send(ArraySegment<byte> segment, int channelId = Channels.DefaultReliable)
        {
            // Debug.Log("ConnectionSend " + this + " bytes:" + BitConverter.ToString(segment.Array, segment.Offset, segment.Count));

            //Debug.Log("ConnectionSend " + this + " bytes:" + BitConverter.ToString(segment.Array, segment.Offset, segment.Count));

            // validate packet size first.
            if (ValidatePacketSize(segment, channelId))
            {
                // batching? then add to queued messages
                if (batching)
                {
                    // put into a (pooled) writer
                    // -> WriteBytes instead of WriteSegment because the latter
                    //    would add a size header. we want to write directly.
                    // -> will be returned to pool when sending!
                    PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
                    writer.WriteBytes(segment.Array, segment.Offset, segment.Count);

                    // add to batch queue
                    Batch batch = GetBatchForChannelId(channelId);
                    batch.messages.Enqueue(writer);
                }
                // otherwise send directly to minimize latency
                else
                {
                    Transport.activeTransport.ServerSend(connectionId, channelId, segment);
                }
            }
        }

        // flush batched messages every batchInterval to make sure that they are
        // sent out every now and then, even if the batch isn't full yet.
        // (avoids 30s latency if batches would only get full every 30s)
        internal void Update()
        {
            // batching?
            if (batching)
            {
                // go through batches for all channels
                foreach (KeyValuePair<int, Batch> kvp in batches)
                {
                    // enough time elapsed to flush this channel's batch?
                    // and not empty?
                    double elapsed = NetworkTime.time - kvp.Value.lastSendTime;
                    if (elapsed >= batchInterval && kvp.Value.messages.Count > 0)
                    {
                        // send the batch. time will be reset internally.
                        //Debug.Log($"sending batch of {kvp.Value.writer.Position} bytes for channel={kvp.Key} connId={connectionId}");
                        SendBatch(kvp.Key, kvp.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Disconnects this connection.
        /// </summary>
=======
        public override string address =>
            Transport.activeTransport.ServerGetClientAddress(connectionId);

        /// <summary>NetworkIdentities that this connection can see</summary>
        // TODO move to server's NetworkConnectionToClient?
        public new readonly HashSet<NetworkIdentity> observing = new HashSet<NetworkIdentity>();

        /// <summary>All NetworkIdentities owned by this connection. Can be main player, pets, etc.</summary>
        // IMPORTANT: this needs to be <NetworkIdentity>, not <uint netId>.
        //            fixes a bug where DestroyOwnedObjects wouldn't find the
        //            netId anymore: https://github.com/vis2k/Mirror/issues/1380
        //            Works fine with NetworkIdentity pointers though.
        public new readonly HashSet<NetworkIdentity> clientOwnedObjects = new HashSet<NetworkIdentity>();

        // unbatcher
        public Unbatcher unbatcher = new Unbatcher();

        public NetworkConnectionToClient(int networkConnectionId)
            : base(networkConnectionId) {}

        // Send stage three: hand off to transport
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void SendToTransport(ArraySegment<byte> segment, int channelId = Channels.Reliable) =>
            Transport.activeTransport.ServerSend(connectionId, segment, channelId);

        /// <summary>Disconnects this connection.</summary>
>>>>>>> Stashed changes
        public override void Disconnect()
        {
            // set not ready and handle clientscene disconnect in any case
            // (might be client or host mode here)
            isReady = false;
            Transport.activeTransport.ServerDisconnect(connectionId);
<<<<<<< Updated upstream
            RemoveObservers();
=======

            // IMPORTANT: NetworkConnection.Disconnect() is NOT called for
            // voluntary disconnects from the other end.
            // -> so all 'on disconnect' cleanup code needs to be in
            //    OnTransportDisconnect, where it's called for both voluntary
            //    and involuntary disconnects!
        }

        internal void AddToObserving(NetworkIdentity netIdentity)
        {
            observing.Add(netIdentity);

            // spawn identity for this conn
            NetworkServer.ShowForConnection(netIdentity, this);
        }

        internal void RemoveFromObserving(NetworkIdentity netIdentity, bool isDestroyed)
        {
            observing.Remove(netIdentity);

            if (!isDestroyed)
            {
                // hide identity for this conn
                NetworkServer.HideForConnection(netIdentity, this);
            }
        }

        internal void RemoveFromObservingsObservers()
        {
            foreach (NetworkIdentity netIdentity in observing)
            {
                netIdentity.RemoveObserver(this);
            }
            observing.Clear();
        }

        internal void AddOwnedObject(NetworkIdentity obj)
        {
            clientOwnedObjects.Add(obj);
        }

        internal void RemoveOwnedObject(NetworkIdentity obj)
        {
            clientOwnedObjects.Remove(obj);
        }

        internal void DestroyOwnedObjects()
        {
            // create a copy because the list might be modified when destroying
            HashSet<NetworkIdentity> tmp = new HashSet<NetworkIdentity>(clientOwnedObjects);
            foreach (NetworkIdentity netIdentity in tmp)
            {
                if (netIdentity != null)
                {
                    NetworkServer.Destroy(netIdentity.gameObject);
                }
            }

            // clear the hashset because we destroyed them all
            clientOwnedObjects.Clear();
>>>>>>> Stashed changes
        }
    }
}
