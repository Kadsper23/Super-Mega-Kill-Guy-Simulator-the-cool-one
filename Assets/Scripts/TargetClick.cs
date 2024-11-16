using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TargetClick : NetworkBehaviour
{
    public PlayerManager PlayerManager;
    // Start is called before the first frame update
    public void OnTargetClick()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();

        if (hasAuthority)
        {
            PlayerManager.CmdTargetSelfCard();
        }
        else
        {
            PlayerManager.CmdTargetOtherCard(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
