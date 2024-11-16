<<<<<<< Updated upstream
using System;
using UnityEngine;
=======
using System.Collections.Generic;
>>>>>>> Stashed changes

namespace Mirror.Examples.Chat
{
    public class Player : NetworkBehaviour
    {
        public static readonly HashSet<string> playerNames = new HashSet<string>();

        [SyncVar(hook = nameof(OnPlayerNameChanged))]
        public string playerName;

<<<<<<< Updated upstream
        public static event Action<Player, string> OnMessage;

        [Command]
        public void CmdSend(string message)
=======
        // RuntimeInitializeOnLoadMethod -> fast playmode without domain reload
        [UnityEngine.RuntimeInitializeOnLoadMethod]
        static void ResetStatics()
        {
            playerNames.Clear();
        }

        void OnPlayerNameChanged(string _, string newName)
        {
            ChatUI.instance.localPlayerName = playerName;
        }

        public override void OnStartServer()
>>>>>>> Stashed changes
        {
            if (message.Trim() != "")
                RpcReceive(message.Trim());
        }
<<<<<<< Updated upstream

        [ClientRpc]
        public void RpcReceive(string message)
        {
            OnMessage?.Invoke(this, message);
        }
=======
>>>>>>> Stashed changes
    }
}
