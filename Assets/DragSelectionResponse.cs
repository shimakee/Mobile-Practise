using UnityEngine;

public class DragSelectionResponse : MonoBehaviour, ISelectionResponse
{
    private Vector3 _originalPosition;
    private Vector3 _targetPosition;

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

    public void OnSelected(GameObject gameObject, Vector3 inputPosition)
    {
        //check it exists
        if (gameObject)
        {   //follow input position
            _targetPosition = gameObject.transform.position;
            gameObject.transform.position = new Vector2(inputPosition.x, inputPosition.y);
        }
    }


    public void OnDeselect(GameObject gameObject)
    {
        //return to original location
        if (gameObject != null && _originalPosition != null)
            gameObject.transform.position = _originalPosition;
    }

    public void OnSelectionConfirm(GameObject gameObject, Vector3 inputPosition)
    {
        //stay at target location
        if (gameObject != null && _targetPosition != null)
            gameObject.transform.position = _targetPosition;
    }
}
