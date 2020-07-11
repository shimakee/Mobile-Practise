using System.Collections.Generic;
using UnityEngine;

public class DefaultSelectionResponse : MonoBehaviour, ISelectionResponse
{
    //Vector3 _originalPosition;
    //Vector3 _targetPosition;
    //Vector3 _originalScale;
    //Quaternion _originalRotation;


    private void Awake()
    {
    }

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

        return hit.collider.gameObject;
    }

    public virtual void IsSelected(GameObject gameObject, Vector3 inputPosition)
    {
        //check it exists
        if (gameObject)
        {
            //check if it has its own selection response
            ISelectionResponse selectionResponse = gameObject.GetComponent<ISelectionResponse>();
            if (selectionResponse != null)
            {
                selectionResponse.IsSelected(gameObject, inputPosition);
            }
            else
            {
                //use default implementation
                //follow input position
                var position = Camera.main.ScreenToWorldPoint(inputPosition);
                gameObject.transform.position = new Vector2(position.x, position.y);
            }
        }
    }


    public virtual void Deselected(GameObject gameObject, Vector3 inputPosition)
    {
        //return to original location
        if (gameObject)
        {
            //check if it has its own selection response
            ISelectionResponse selectionResponse = gameObject.GetComponent<ISelectionResponse>();
            if (selectionResponse != null)
            {
                selectionResponse.Deselected(gameObject, inputPosition);
            }
            else
            {
                //use default implementation
                //no implementation yet
            }
        }
    }

    public virtual void OnSelectionConfirm(GameObject gameObject, Vector3 inputPosition)
    {
        //check if it has its own selection response
        ISelectionResponse selectionResponse = gameObject.GetComponent<ISelectionResponse>();
        if (selectionResponse != null)
        {
            selectionResponse.OnSelectionConfirm(gameObject, inputPosition);
        }
        else
        {
            //use default implementation
            //no implementation yet
        }
    }

    public virtual void OnHoverSelected(GameObject gameObject, Vector3 inputPosition)
    {
        //check if it has its own selection response
        ISelectionResponse selectionResponse = gameObject.GetComponent<ISelectionResponse>();
        if (selectionResponse != null)
        {
            selectionResponse.OnHoverSelected(gameObject, inputPosition);
        }
        else
        {
            //use default implementation
            //just follow is selected method and drag object to touch location
            this.IsSelected(gameObject, inputPosition);
        }
    }

    public virtual void WasSelected(GameObject gameObject, Vector3 inputPosition)
    {
        //check if it has its own selection response
        ISelectionResponse selectionResponse = gameObject.GetComponent<ISelectionResponse>();
        if (selectionResponse != null)
        {
            selectionResponse.WasSelected(gameObject, inputPosition);
        }
        else
        {
            //use default implementation
            //no implementation yet
        }
    }

    public virtual void IsSelectedUnique(GameObject gameObject, Vector3 inputPosition)
    {
        //check if it has its own selection response
        ISelectionResponse selectionResponse = gameObject.GetComponent<ISelectionResponse>();
        if (selectionResponse != null)
        {
            selectionResponse.IsSelectedUnique(gameObject, inputPosition);
        }
        else
        {
            //use default implementation
            //no implementation yet
        }
    }
}