using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


public class TouchInputController : MonoBehaviour
{

    ISelectionResponse _selectionResponse;
    Dictionary<int, GameObject> _selectedObjects; // for touch

    private void Awake()
    {
        _selectionResponse = GetComponent<ISelectionResponse>();
        if (_selectionResponse == null)
            throw new NullReferenceException("Selection response component is null.");
    }
    private void Start()
    {
        _selectedObjects = new Dictionary<int, GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.touchCount > 0)
        {
            foreach (var touch in Input.touches)
            {

                OnTouchCanceled(touch);
                OnTouchEnd(touch);
                OnTouchMoved(touch);
                OnTouchBegin(touch);
            }

            if(Input.touchCount == 2)
            {

                float pinch = TouchPinchDifferenceFromPrevious(Input.touches);

                if (pinch < 0)
                {
                    OnPinchIn();
                }
                if (pinch > 0)
                {
                    OnPinchOut();
                }
            }
        }


    }

    ///<summary>
    ///<para>Gets the difference between current pinch distance less previous pinch distance.</para>
    ///<para>Where if difference > 0 = pinch out.</para>
    ///<para>And difference < 0 = pinch in.</para>
    ///</summary>
    /// <returns>Returns float</returns>
    private float TouchPinchDifferenceFromPrevious(Touch[] touches)
    {
        if (touches.Length != 2)
            throw new InvalidOperationException("Can only do pinch method when touch array is equal to 2.");


        Touch firstTouch = Input.GetTouch(0);
        Touch secondTouch = Input.GetTouch(1);

        Vector2 firstTouchPreviousPosition = firstTouch.position - firstTouch.deltaPosition;
        Vector2 secondTouchPreviousPosition = secondTouch.position - secondTouch.deltaPosition;

        float touchDistancePreviousPosition = (firstTouchPreviousPosition - secondTouchPreviousPosition).magnitude;
        float touchDistanceCurrentPosition = (firstTouch.position - secondTouch.position).magnitude;

        float distanceDifferene = touchDistanceCurrentPosition - touchDistancePreviousPosition;

        return distanceDifferene;

    }


    public virtual void OnTouchBegin(Touch touch)
    {
        if (touch.phase == TouchPhase.Began)
        {
            GameObject selection = _selectionResponse.DetermineSelection(touch.position);

            if (!selection)
                return;

            if (!_selectedObjects.ContainsKey(touch.fingerId))
                _selectedObjects.Add(touch.fingerId, selection);
        }
    }
    public virtual void OnTouchMoved(Touch touch)
    {
        if (touch.phase == TouchPhase.Moved && _selectedObjects.ContainsKey(touch.fingerId))
            _selectionResponse.IsSelected(_selectedObjects[touch.fingerId], Camera.main.ScreenToWorldPoint(touch.position));

    }
    public virtual void OnTouchEnd(Touch touch)
    {
        //remove or deselect object
        if (touch.phase == TouchPhase.Ended)
        {
            //check it exists
            if (_selectedObjects.ContainsKey(touch.fingerId))
            {
                //_selectionResponse.OnSelectionConfirm(_selectedObjects[touch.fingerId], Camera.main.ScreenToWorldPoint(touch.position));
                _selectedObjects.Remove(touch.fingerId);
            }
        }
    }
    public virtual void OnTouchCanceled(Touch touch)
    {
        if (touch.phase == TouchPhase.Canceled)
        {
            //check it exists
            if (_selectedObjects.ContainsKey(touch.fingerId))
            {
                _selectionResponse.Deselected(_selectedObjects[touch.fingerId], touch.position);
                _selectedObjects.Remove(touch.fingerId);
            }
        }
    }

    public virtual void OnTouchTap(Touch touch)
    {
        int tapcount = touch.tapCount;
    }

    public virtual void OnPinchIn()
    {
        //your pinching in
        Debug.Log("pinching In");
    }

    public virtual void OnPinchOut()
    {
        //your pinching out
        Debug.Log("pinching out");
    }

    public void OnTouchStationary(Touch touch)
    {
        throw new NotImplementedException();
    }
}

