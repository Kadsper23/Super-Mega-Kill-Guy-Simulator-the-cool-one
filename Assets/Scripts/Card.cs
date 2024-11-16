using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Minion")]
public class Card : ScriptableObject
{
    public new string name;
    public string description;

    public int health;
    public int attack;
    public int apCost;
}
