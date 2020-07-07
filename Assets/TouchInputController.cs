using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TouchInputController : MonoBehaviour
{
    private ISelectionResponse _selectionResponse;
    private Dictionary<int, GameObject> _selectedObjects; // for touch

    private bool isPressed;

    private void Awake()
    {
        _selectionResponse = GetComponent<ISelectionResponse>();
        if (_selectionResponse == null)
            throw new NullReferenceException("Selection response component is null.");
    }
    // Start is called before the first frame update
    void Start()
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

                TouchStart(touch);
                TouchMove(touch);
                TouchRelease(touch);
                TouchCancelled(touch);

            }
        }
        
    }

    void TouchStart(Touch touch)
    {
        if (touch.phase == TouchPhase.Began)
        {
            GameObject selection = _selectionResponse.DetermineSelection(touch.position);

            if (!selection)
                return;

            if (!_selectedObjects.ContainsKey(touch.fingerId))
                _selectedObjects.Add(touch.fingerId,selection);
        }
    }
    void TouchMove(Touch touch)
    {
        if (touch.phase == TouchPhase.Moved)
            _selectionResponse.OnSelected(_selectedObjects[touch.fingerId], Camera.main.ScreenToWorldPoint(touch.position));

    }
    private void TouchRelease(Touch touch)
    {
        //remove or deselect object
        if (touch.phase == TouchPhase.Ended)
        {
            //check it exists
            if (_selectedObjects.ContainsKey(touch.fingerId))
            {
                _selectionResponse.OnSelectionConfirm(_selectedObjects[touch.fingerId], Camera.main.ScreenToWorldPoint(touch.position));
                _selectedObjects.Remove(touch.fingerId);
            }
        }
    }
    private void TouchCancelled(Touch touch)
    {
        if (touch.phase == TouchPhase.Canceled)
        {
            //check it exists
            if (_selectedObjects.ContainsKey(touch.fingerId))
            {
                _selectionResponse.OnDeselect(_selectedObjects[touch.fingerId]);
                _selectedObjects.Remove(touch.fingerId);
            }
        }
    }



}
