using System;
using UnityEngine;
using UnityEngine.Events;

namespace Mirror
{
<<<<<<< Updated upstream
    /// <summary>
    /// Unity Event for the NetworkConnection
    /// </summary>
    [Serializable] public class UnityEventNetworkConnection : UnityEvent<NetworkConnection> {}

    /// <summary>
    /// Base class for implementing component-based authentication during the Connect phase
    /// </summary>
    [HelpURL("https://mirror-networking.com/docs/Articles/Guides/Authentication.html")]
    public abstract class NetworkAuthenticator : MonoBehaviour
    {
        [Header("Event Listeners (optional)")]

        /// <summary>
        /// Notify subscribers on the server when a client is authenticated
        /// </summary>
        [Tooltip("Mirror has an internal subscriber to this event. You can add your own here.")]
        public UnityEventNetworkConnection OnServerAuthenticated = new UnityEventNetworkConnection();

        /// <summary>
        /// Notify subscribers on the client when the client is authenticated
        /// </summary>
        [Tooltip("Mirror has an internal subscriber to this event. You can add your own here.")]
        public UnityEventNetworkConnection OnClientAuthenticated = new UnityEventNetworkConnection();

        #region server

        /// <summary>
        /// Called on server from StartServer to initialize the Authenticator
        /// <para>Server message handlers should be registered in this method.</para>
        /// </summary>
        public virtual void OnStartServer() {}

        /// <summary>
        /// Called on server from StopServer to reset the Authenticator
        /// <para>Server message handlers should be unregistered in this method.</para>
        /// </summary>
        public virtual void OnStopServer() {}

        /// <summary>
        /// Called on server from OnServerAuthenticateInternal when a client needs to authenticate
        /// </summary>
        /// <param name="conn">Connection to client.</param>
        public abstract void OnServerAuthenticate(NetworkConnection conn);

        protected void ServerAccept(NetworkConnection conn)
=======
    [Serializable] public class UnityEventNetworkConnection : UnityEvent<NetworkConnectionToClient> {}

    /// <summary>Base class for implementing component-based authentication during the Connect phase</summary>
    [HelpURL("https://mirror-networking.gitbook.io/docs/components/network-authenticators")]
    public abstract class NetworkAuthenticator : MonoBehaviour
    {
        /// <summary>Notify subscribers on the server when a client is authenticated</summary>
        [Header("Event Listeners (optional)")]
        [Tooltip("Mirror has an internal subscriber to this event. You can add your own here.")]
        public UnityEventNetworkConnection OnServerAuthenticated = new UnityEventNetworkConnection();

        /// <summary>Notify subscribers on the client when the client is authenticated</summary>
        [Tooltip("Mirror has an internal subscriber to this event. You can add your own here.")]
        public UnityEvent OnClientAuthenticated = new UnityEvent();

        /// <summary>Called when server starts, used to register message handlers if needed.</summary>
        public virtual void OnStartServer() {}

        /// <summary>Called when server stops, used to unregister message handlers if needed.</summary>
        public virtual void OnStopServer() {}

        /// <summary>Called on server from OnServerConnectInternal when a client needs to authenticate</summary>
        public virtual void OnServerAuthenticate(NetworkConnectionToClient conn) {}

        protected void ServerAccept(NetworkConnectionToClient conn)
>>>>>>> Stashed changes
        {
            OnServerAuthenticated.Invoke(conn);
        }

<<<<<<< Updated upstream
        protected void ServerReject(NetworkConnection conn)
=======
        protected void ServerReject(NetworkConnectionToClient conn)
>>>>>>> Stashed changes
        {
            conn.Disconnect();
        }

<<<<<<< Updated upstream
        #endregion

        #region client

        /// <summary>
        /// Called on client from StartClient to initialize the Authenticator
        /// <para>Client message handlers should be registered in this method.</para>
        /// </summary>
        public virtual void OnStartClient() {}

        /// <summary>
        /// Called on client from StopClient to reset the Authenticator
        /// <para>Client message handlers should be unregistered in this method.</para>
        /// </summary>
        public virtual void OnStopClient() {}

        /// <summary>
        /// Called on client from OnClientAuthenticateInternal when a client needs to authenticate
        /// </summary>
        /// <param name="conn">Connection of the client.</param>
        public abstract void OnClientAuthenticate(NetworkConnection conn);

        protected void ClientAccept(NetworkConnection conn)
        {
            OnClientAuthenticated.Invoke(conn);
        }

        protected void ClientReject(NetworkConnection conn)
        {
            // Set this on the client for local reference
            conn.isAuthenticated = false;

            // disconnect the client
            conn.Disconnect();
        }

        #endregion

        void OnValidate()
=======
        /// <summary>Called when client starts, used to register message handlers if needed.</summary>
        public virtual void OnStartClient() {}

        /// <summary>Called when client stops, used to unregister message handlers if needed.</summary>
        public virtual void OnStopClient() {}

        /// <summary>Called on client from OnClientConnectInternal when a client needs to authenticate</summary>
        public virtual void OnClientAuthenticate() {}

        protected void ClientAccept()
        {
            OnClientAuthenticated.Invoke();
        }

        protected void ClientReject()
        {
            // Set this on the client for local reference
            NetworkClient.connection.isAuthenticated = false;

            // disconnect the client
            NetworkClient.connection.Disconnect();
        }
        
        // Reset() instead of OnValidate():
        // Any NetworkAuthenticator assigns itself to the NetworkManager, this is fine on first adding it, 
        // but if someone intentionally sets Authenticator to null on the NetworkManager again then the 
        // Authenticator will reassign itself if a value in the inspector is changed.
        // My change switches OnValidate to Reset since Reset is only called when the component is first 
        // added (or reset is pressed).
        void Reset()
>>>>>>> Stashed changes
        {
#if UNITY_EDITOR
            // automatically assign authenticator field if we add this to NetworkManager
            NetworkManager manager = GetComponent<NetworkManager>();
            if (manager != null && manager.authenticator == null)
            {
<<<<<<< Updated upstream
                manager.authenticator = this;
                UnityEditor.Undo.RecordObject(gameObject, "Assigned NetworkManager authenticator");
=======
                // undo has to be called before the change happens
                UnityEditor.Undo.RecordObject(manager, "Assigned NetworkManager authenticator");
                manager.authenticator = this;
>>>>>>> Stashed changes
            }
#endif
        }
    }
}
