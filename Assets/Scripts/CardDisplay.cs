using Microsoft.Unity.VisualStudio.Editor;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CardDisplay : NetworkBehaviour
{
    public int randNum;
    public Card card;
    public Sprite artworkImage;
    public List<Card> cards;

    // Start is called before the first frame update
    void Start()
    {
        Randomize();
        card = cards[randNum];
        artworkImage = card.artwork;
        gameObject.GetComponent<CardFlipper>().CardFront = artworkImage;
        gameObject.GetComponent<UnityEngine.UI.Image>().sprite = artworkImage;
        
    }
    void Randomize()
    {
        randNum = Random.Range(0, cards.Count);
    }
}
