using UnityEngine;

public class ScaleSelectionResponse : MonoBehaviour, ISelectionResponse
{
    private Vector3 _originalScale;
    private Vector3 _targetScale;
    public GameObject DetermineSelection(Vector3 selectionPosition)
    {
        //create ray
        Ray ray = Camera.main.ScreenPointToRay(selectionPosition);
        //draw raycast
        Debug.DrawLine(ray.origin, ray.direction, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        //check that it hit something
        if (!hit)
            return null;

        //get original scale
        _originalScale = hit.collider.gameObject.transform.localScale;
        return hit.collider.gameObject;
    }

    public void OnSelected(GameObject gameObject, Vector3 inputPosition)
    {
        //check it exists
        if (gameObject)
        {   //transform scale based on input position
            gameObject.transform.localScale = new Vector3(inputPosition.x, inputPosition.y, inputPosition.z);
            _targetScale = gameObject.transform.localScale;
        }
    }


    public void OnDeselect(GameObject gameObject)
    {
        //return to original scale
        if(_originalScale != null && gameObject != null)
        {
            gameObject.transform.localScale = _originalScale;
        }
    }

    public void OnSelectionConfirm(GameObject gameObject, Vector3 inputPosition)
    {
        //check it exists
        if (gameObject)
        {   //maintain target scale;
            gameObject.transform.localScale = _targetScale;
        }
    }
}
