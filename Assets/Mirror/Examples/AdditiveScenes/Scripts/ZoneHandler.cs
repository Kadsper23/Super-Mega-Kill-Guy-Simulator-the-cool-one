using UnityEngine;

namespace Mirror.Examples.Additive
{
<<<<<<< Updated upstream
    // This script is attached to a scene object called Zone that is on the Player layer and has:
    // - Sphere Collider with isTrigger = true
    // - Network Identity with Server Only checked
    // These OnTrigger events only run on the server and will only send a message to the player
    // that entered the Zone to load the subscene assigned to the subscene property.
=======
    // This script is attached to a prefab called Zone that is on the Player layer
    // AdditiveNetworkManager, in OnStartServer, instantiates the prefab only on the server.
    // It never exists for clients (other than host client if there is one).
    // The prefab has a Sphere Collider with isTrigger = true.
    // These OnTrigger events only run on the server and will only send a message to the
    // client that entered the Zone to load the subscene assigned to the subscene property.
>>>>>>> Stashed changes
    public class ZoneHandler : MonoBehaviour
    {
        [Scene]
        [Tooltip("Assign the sub-scene to load for this zone")]
        public string subScene;

        void OnTriggerEnter(Collider other)
        {
<<<<<<< Updated upstream
            if (!NetworkServer.active) return;

            // Debug.LogFormat(LogType.Log, "Loading {0}", subScene);

=======
            // Debug.Log($"Loading {subScene}");

>>>>>>> Stashed changes
            NetworkIdentity networkIdentity = other.gameObject.GetComponent<NetworkIdentity>();
            SceneMessage message = new SceneMessage{ sceneName = subScene, sceneOperation = SceneOperation.LoadAdditive };
            networkIdentity.connectionToClient.Send(message);
        }

        void OnTriggerExit(Collider other)
        {
<<<<<<< Updated upstream
            if (!NetworkServer.active) return;

            // Debug.LogFormat(LogType.Log, "Unloading {0}", subScene);

=======
            // Debug.Log($"Unloading {subScene}");

>>>>>>> Stashed changes
            NetworkIdentity networkIdentity = other.gameObject.GetComponent<NetworkIdentity>();
            SceneMessage message = new SceneMessage{ sceneName = subScene, sceneOperation = SceneOperation.UnloadAdditive };
            networkIdentity.connectionToClient.Send(message);
        }
    }
}
