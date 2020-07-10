using UnityEngine;

public class DefaultTouchResponse : MonoBehaviour, ITouchResponse
{
    Vector3 _originalPosition;
    Vector3 _originalRotation;
    Vector3 _originalScale;

    Transform _originalTransform;

    public GameObject DetermineSelection(Vector3 inputPosition)
    {
        //create ray
        Ray ray = Camera.main.ScreenPointToRay(inputPosition);
        //draw raycast
        Debug.DrawLine(ray.origin, ray.direction, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        //check that it hit something
        if (!hit)
            return null;

        //store original position 
        _originalPosition = hit.collider.gameObject.transform.position;
        return hit.collider.gameObject;
    }
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

    public void SelectionConfirmed(GameObject gameObject, Vector3 inputPositoin)
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer)
            spriteRenderer.color = Color.black;
        Debug.Log("i was confirmed", this);
    }

}