using System.Collections;
using System.Collections.Generic;
<<<<<<< Updated upstream
=======
using UnityEngine.Experimental;
>>>>>>> Stashed changes
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    public GameObject Card1;
    public GameObject Card2;
    public GameObject PlayerArea;
    public GameObject EnemyArea;
    public GameObject DropZone;

    List<GameObject> cards = new List<GameObject>();

    public override void OnStartClient()
    {
        base.OnStartClient();

        PlayerArea = GameObject.Find("PlayerArea");
        EnemyArea = GameObject.Find("EnemyArea");
        DropZone = GameObject.Find("DropZOne");
    }

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();

        cards.Add(Card1);
        cards.Add(Card2);
        Debug.Log(cards);
<<<<<<< Updated upstream
=======

>>>>>>> Stashed changes
    }

    [Command]
    public void CmdDealCards()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject card = Instantiate(cards[Random.Range(0, cards.Count)], new Vector2(0, 0), Quaternion.identity);
            NetworkServer.Spawn(card, connectionToClient);
            RpcShowCard(card, "Dealt");
        }
    }

<<<<<<< Updated upstream
 [ClientRpc]
=======
    [ClientRpc]
>>>>>>> Stashed changes
    void RpcShowCard(GameObject card, string type)
    {
        if (type == "Dealt")
        {
<<<<<<< Updated upstream
            if (isOwned)
=======
            if (hasAuthority)
>>>>>>> Stashed changes
            {
                card.transform.SetParent(PlayerArea.transform, false);
            }
            else
            {
                card.transform.SetParent(EnemyArea.transform, false);
            }
<<<<<<< Updated upstream
        } else if (type == "Played")
=======
        }
        else if (type == "Played")
>>>>>>> Stashed changes
        {

        }
    }
}
