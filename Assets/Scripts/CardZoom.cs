using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Reflection;

public class CardZoom : NetworkBehaviour
{

    public GameObject Canvas;
    public GameObject ZoomCard;

    private GameObject zoomCard;
    private Sprite zoomSprite;

    public void Awake()
    {
        gameObject.GetComponent<Image>().sprite = gameObject.GetComponent<CardFlipper>().CardFront;
        Canvas = GameObject.Find("Main Canvas");
        
    }

    public void OnHoverEnter()
    {
        zoomSprite = gameObject.GetComponent<Image>().sprite;
        if (!hasAuthority && !gameObject.GetComponent<DragDrop>().isOverDropZone) return;
        zoomCard = Instantiate(ZoomCard, new Vector2(Input.mousePosition.x, Input.mousePosition.y + 250), Quaternion.identity);
        zoomCard.GetComponent<Image>().sprite = zoomSprite;
        zoomCard.transform.SetParent(Canvas.transform, true);
        RectTransform rect = zoomCard.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(240, 344);
    }

    public void OnHoverExit() 
    {
        Destroy(zoomCard);
    }
}
