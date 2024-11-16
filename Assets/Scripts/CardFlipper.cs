using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardFlipper : MonoBehaviour
{
    public Sprite CardFront;
    public Sprite CardBack;

    void Start()
    {
        CardFront = GetComponent<CardDisplay>().artworkImage;
    }

    public void Flip()
    {
        Sprite currentSprite = gameObject.GetComponent<Image>().sprite;

        if (currentSprite == CardFront)
        {
            gameObject.GetComponent<Image>().sprite = CardBack;
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = CardFront;
        }
    }
}
