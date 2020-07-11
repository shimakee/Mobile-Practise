using UnityEngine;

public class DefaultSelectionResponse : MonoBehaviour, ISelectionResponse
{
    Vector3 _originalPosition;
    Vector3 _targetPosition;
    //Vector3 _originalScale;
    //Quaternion _originalRotation;

    //Transform _originalTransform;

    public virtual GameObject DetermineSelection(Vector3 inputPosition)
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
        //_originalScale = hit.collider.gameObject.transform.localScale;
        //_originalRotation = hit.collider.gameObject.transform.rotation;
        //_originalTransform = hit.collider.gameObject.transform;
        return hit.collider.gameObject;
    }

    public virtual void IsSelected(GameObject gameObject, Vector3 inputPosition)
    {
        //check it exists
        if (gameObject)
        {   //follow input position
            var position = Camera.main.ScreenToWorldPoint(inputPosition);
            _targetPosition = gameObject.transform.position;
            gameObject.transform.position = new Vector2(position.x, position.y);
        }
    }

    public virtual void Deselected(GameObject gameObject, Vector3 inputPosition)
    {
        //return to original location
        if (gameObject != null && _originalPosition != null)
            gameObject.transform.position = _originalPosition;
    }

    public virtual void OnSelectionConfirm(GameObject gameObject, Vector3 inputPosition)
    {
        //stay at target location
        if (gameObject != null && _targetPosition != null)
            gameObject.transform.position = _targetPosition;
    }

    public virtual void OnHoverSelected(GameObject gameObject, Vector3 inputPosition)
    {
        this.IsSelected(gameObject, inputPosition);
    }

    public virtual void WasSelected(GameObject gameObject, Vector3 inputPosition)
    {
        this.Deselected(gameObject, inputPosition);
    }

    public virtual void IsSelectedUnique(GameObject gameObject, Vector3 inputPosition)
    {
        this.OnSelectionConfirm(gameObject, inputPosition);
    }
}