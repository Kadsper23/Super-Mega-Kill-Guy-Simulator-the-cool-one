// interest management component for custom solutions like
// distance based, spatial hashing, raycast based, etc.
using System.Collections.Generic;
using UnityEngine;

namespace Mirror
{
    [DisallowMultipleComponent]
<<<<<<< Updated upstream
    public abstract class InterestManagement : MonoBehaviour
    {
        // Awake configures InterestManagement in NetworkServer
=======
    [HelpURL("https://mirror-networking.gitbook.io/docs/guides/interest-management")]
    public abstract class InterestManagement : MonoBehaviour
    {
        // Awake configures InterestManagement in NetworkServer/Client
        // Do NOT check for active server or client here.
        // Awake must always set the static aoi references.
>>>>>>> Stashed changes
        void Awake()
        {
            if (NetworkServer.aoi == null)
            {
                NetworkServer.aoi = this;
            }
            else Debug.LogError($"Only one InterestManagement component allowed. {NetworkServer.aoi.GetType()} has been set up already.");
<<<<<<< Updated upstream
        }

=======

            if (NetworkClient.aoi == null)
            {
                NetworkClient.aoi = this;
            }
            else Debug.LogError($"Only one InterestManagement component allowed. {NetworkClient.aoi.GetType()} has been set up already.");
        }

        [ServerCallback]
        public virtual void Reset() {}

>>>>>>> Stashed changes
        // Callback used by the visibility system to determine if an observer
        // (player) can see the NetworkIdentity. If this function returns true,
        // the network connection will be added as an observer.
        //   conn: Network connection of a player.
        //   returns True if the player can see this object.
<<<<<<< Updated upstream
        public abstract bool OnCheckObserver(NetworkIdentity identity, NetworkConnection newObserver);
=======
        public abstract bool OnCheckObserver(NetworkIdentity identity, NetworkConnectionToClient newObserver);
>>>>>>> Stashed changes

        // rebuild observers for the given NetworkIdentity.
        // Server will automatically spawn/despawn added/removed ones.
        //   newObservers: cached hashset to put the result into
        //   initialize: true if being rebuilt for the first time
        //
        // IMPORTANT:
        // => global rebuild would be more simple, BUT
        // => local rebuild is way faster for spawn/despawn because we can
        //    simply rebuild a select NetworkIdentity only
        // => having both .observers and .observing is necessary for local
        //    rebuilds
        //
        // in other words, this is the perfect solution even though it's not
        // completely simple (due to .observers & .observing).
        //
        // Mirror maintains .observing automatically in the background. best of
        // both worlds without any worrying now!
<<<<<<< Updated upstream
        public abstract void OnRebuildObservers(NetworkIdentity identity, HashSet<NetworkConnection> newObservers, bool initialize);
=======
        public abstract void OnRebuildObservers(NetworkIdentity identity, HashSet<NetworkConnectionToClient> newObservers);
>>>>>>> Stashed changes

        // helper function to trigger a full rebuild.
        // most implementations should call this in a certain interval.
        // some might call this all the time, or only on team changes or
        // scene changes and so on.
        //
        // IMPORTANT: check if NetworkServer.active when using Update()!
<<<<<<< Updated upstream
        protected void RebuildAll()
        {
            foreach (NetworkIdentity identity in NetworkIdentity.spawned.Values)
=======
        [ServerCallback]
        protected void RebuildAll()
        {
            foreach (NetworkIdentity identity in NetworkServer.spawned.Values)
>>>>>>> Stashed changes
            {
                NetworkServer.RebuildObservers(identity, false);
            }
        }
<<<<<<< Updated upstream
=======

        // Callback used by the visibility system for objects on a host.
        // Objects on a host (with a local client) cannot be disabled or
        // destroyed when they are not visible to the local client. So this
        // function is called to allow custom code to hide these objects. A
        // typical implementation will disable renderer components on the
        // object. This is only called on local clients on a host.
        // => need the function in here and virtual so people can overwrite!
        // => not everyone wants to hide renderers!
        [ServerCallback]
        public virtual void SetHostVisibility(NetworkIdentity identity, bool visible)
        {
            foreach (Renderer rend in identity.GetComponentsInChildren<Renderer>())
                rend.enabled = visible;
        }

        /// <summary>Called on the server when a new networked object is spawned.</summary>
        // (useful for 'only rebuild if changed' interest management algorithms)
        [ServerCallback]
        public virtual void OnSpawned(NetworkIdentity identity) {}

        /// <summary>Called on the server when a networked object is destroyed.</summary>
        // (useful for 'only rebuild if changed' interest management algorithms)
        [ServerCallback]
        public virtual void OnDestroyed(NetworkIdentity identity) {}
>>>>>>> Stashed changes
    }
}
