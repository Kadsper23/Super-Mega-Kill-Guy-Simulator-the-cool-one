using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DragDrop : MonoBehaviour
{
    private bool isDragging = false;
    public GameObject Canvas;
    public float dragScale = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        Canvas = GameObject.Find("Main Canvas");
    }
    public void StartDrag()
    {
        isDragging = true;
        transform.localScale = transform.localScale * dragScale;
    }

    public void EndDrag()
    {
        isDragging = false;
        transform.localScale /= dragScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            transform.SetParent(Canvas.transform, true);
        }
    }
}
