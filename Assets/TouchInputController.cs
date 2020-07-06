using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInputController : MonoBehaviour
{
    private Dictionary<int, GameObject> _selectedObjects;

    // Start is called before the first frame update
    void Start()
    {
        _selectedObjects = new Dictionary<int, GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.touchCount > 0)
        {

            foreach (var touch in Input.touches)
            {
                //create ray
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                //draw raycast
                Debug.DrawLine(ray.origin, ray.direction, Color.red);

                //find & add selected object using raycast
                if(touch.phase == TouchPhase.Began)
                {
                    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                    //check that it hit something
                    if (hit)
                    {
                        _selectedObjects.Add(touch.fingerId, hit.collider.gameObject);
                    }
                }

                //move selected object with touch input
                if(touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    //check it exists
                    if(_selectedObjects.ContainsKey(touch.fingerId))
                        _selectedObjects[touch.fingerId].transform.position = Camera.main.ScreenToWorldPoint(touch.position);
                }

                //remove or deselect object
                if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    //check it exist
                    if(_selectedObjects.ContainsKey(touch.fingerId))
                        _selectedObjects.Remove(touch.fingerId);
                }
            }
        }
        
    }
}
