using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelector : MonoBehaviour, IObjectSelector
{
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
}
