using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSelectionResponse : DefaultSelectionResponse, ISelectionResponse
{
    public override void IsSelected(GameObject gameObject, Vector3 inputPosition)
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer)
            spriteRenderer.color = Color.green;
    }
    public override void WasSelected(GameObject gameObject, Vector3 inputPosition)
    {
        Debug.Log("was selected", this);
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer)
            spriteRenderer.color = Color.blue;

    }
    public override void Deselected(GameObject gameObject, Vector3 inputPosition)
    {
        Debug.Log("deselected", this);
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer)
            spriteRenderer.color = Color.white;
        gameObject.transform.localScale = new Vector3(1, 1, 1);
    }
    public override void IsSelectedUnique(GameObject gameObject, Vector3 inputPosition)
    {
        Debug.Log("isSelectedUnique", this);

        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer)
            spriteRenderer.color = Color.yellow;
    }

    public override void OnHoverSelected(GameObject gameObject, Vector3 inputPosition)
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer)
            spriteRenderer.color = Color.red;
        Debug.Log("i was hovered", this);
    }

    public override void OnSelectionConfirm(GameObject gameObject, Vector3 inputPositoin)
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer)
            spriteRenderer.color = Color.black;
        Debug.Log("i was confirmed", this);
    }
}
