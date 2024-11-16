using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public int TurnsPlayed = 0;

    public int playerOneHealth = 20;
    public int playerTwoHealth = 20;
    

    public void UpdateTurnsPlayed()
    {
        TurnsPlayed++;
    }
}
