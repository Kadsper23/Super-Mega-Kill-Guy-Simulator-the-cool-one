using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCards : MonoBehaviour
{
    public GameObject Card1;
    public GameObject Card2;
    public GameObject PlayerArea;
    public int handLimit = 10;
    private int heldCards;

    public void OnClick()
    {
        for (int i = 0; i < 5; i++)
        {
            if (heldCards < handLimit)
            {
                GameObject card = Instantiate(Card1, new Vector2(25 * i, 0), Quaternion.identity);
                card.transform.SetParent(PlayerArea.transform, false);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        heldCards = GameObject.FindGameObjectsWithTag("PlayerCard").Length;
    }
}