// straight forward Vector3.Distance based interest management.
using System.Collections.Generic;
using UnityEngine;

namespace Mirror
{
    public class DistanceInterestManagement : InterestManagement
    {
<<<<<<< Updated upstream
        [Tooltip("The maximum range that objects will be visible at.")]
=======
        [Tooltip("The maximum range that objects will be visible at. Add DistanceInterestManagementCustomRange onto NetworkIdentities for custom ranges.")]
>>>>>>> Stashed changes
        public int visRange = 10;

        [Tooltip("Rebuild all every 'rebuildInterval' seconds.")]
        public float rebuildInterval = 1;
        double lastRebuildTime;

<<<<<<< Updated upstream
        public override bool OnCheckObserver(NetworkIdentity identity, NetworkConnection newObserver)
        {
            return Vector3.Distance(identity.transform.position, newObserver.identity.transform.position) <= visRange;
        }

        public override void OnRebuildObservers(NetworkIdentity identity, HashSet<NetworkConnection> newObservers, bool initialize)
        {
            // 'transform.' calls GetComponent, only do it once
=======
        // helper function to get vis range for a given object, or default.
        int GetVisRange(NetworkIdentity identity)
        {
            return identity.TryGetComponent(out DistanceInterestManagementCustomRange custom) ? custom.visRange : visRange;
        }

        [ServerCallback]
        public override void Reset()
        {
            lastRebuildTime = 0D;
        }

        public override bool OnCheckObserver(NetworkIdentity identity, NetworkConnectionToClient newObserver)
        {
            int range = GetVisRange(identity);
            return Vector3.Distance(identity.transform.position, newObserver.identity.transform.position) < range;
        }

        public override void OnRebuildObservers(NetworkIdentity identity, HashSet<NetworkConnectionToClient> newObservers)
        {
            // cache range and .transform because both call GetComponent.
            int range = GetVisRange(identity);
>>>>>>> Stashed changes
            Vector3 position = identity.transform.position;

            // brute force distance check
            // -> only player connections can be observers, so it's enough if we
            //    go through all connections instead of all spawned identities.
            // -> compared to UNET's sphere cast checking, this one is orders of
            //    magnitude faster. if we have 10k monsters and run a sphere
            //    cast 10k times, we will see a noticeable lag even with physics
            //    layers. but checking to every connection is fast.
            foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
            {
                // authenticated and joined world with a player?
                if (conn != null && conn.isAuthenticated && conn.identity != null)
                {
                    // check distance
                    if (Vector3.Distance(conn.identity.transform.position, position) < visRange)
                    {
                        newObservers.Add(conn);
                    }
                }
            }
        }

        void Update()
        {
            // only on server
            if (!NetworkServer.active) return;

            // rebuild all spawned NetworkIdentity's observers every interval
            if (NetworkTime.time >= lastRebuildTime + rebuildInterval)
            {
                RebuildAll();
                lastRebuildTime = NetworkTime.time;
            }
        }
    }
}
