using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSelectionResponse : MonoBehaviour, ISelectionResponse
{
    public void IsSelected(GameObject gameObject, Vector3 inputPosition)
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer)
            spriteRenderer.color = Color.green;
    }
    public void WasSelected(GameObject gameObject, Vector3 inputPosition)
    {
        Debug.Log("was selected", this);
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer)
            spriteRenderer.color = Color.blue;

    }
    public void Deselected(GameObject gameObject, Vector3 inputPosition)
    {
        Debug.Log("deselected", this);
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer)
            spriteRenderer.color = Color.white;
        gameObject.transform.localScale = new Vector3(1, 1, 1);
    }
    public void IsSelectedUnique(GameObject gameObject, Vector3 inputPosition)
    {
        Debug.Log("isSelectedUnique", this);

        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer)
            spriteRenderer.color = Color.yellow;
    }

    public void OnHoverSelected(GameObject gameObject, Vector3 inputPosition)
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer)
            spriteRenderer.color = Color.red;
        Debug.Log("i was hovered", this);
    }

    public void OnSelectionConfirm(GameObject gameObject, Vector3 inputPositoin)
    {
        this.OnSelectionConfirm(gameObject, inputPositoin, new List<GameObject>());
    }

    public void OnSelectionConfirm(GameObject gameObject, Vector3 inputPosition, List<GameObject> wasSelectedGameObjects)
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer)
            spriteRenderer.color = Color.black;
        Debug.Log("i was confirmed", this);
    }
}
