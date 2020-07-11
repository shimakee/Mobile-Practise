using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchConroller : MonoBehaviour
{
    public ISelectionResponse touchResponse;
    //public bool enablePinch; //to be implemented
    //public bool enableSwipe; //to be implemented
    public bool enablePassiveSelection;
    public bool enableUnniqueSelection;
    public bool enableLastTouchConfirm;
    public bool enableDiselectOnlyOnTouchOff;
    public int maxAllowedTouch = 10;
    public float timeOnHover = 2f;
    public float timeOnTap = .5f;

    //timers
    float _hoverTime;
    float _tapTime;
    int _tapCount;

    //objects selected at touch phases
    Dictionary<int, GameObject> _currentSelection = new Dictionary<int, GameObject>();
    public Dictionary<int, List<GameObject>> _selectedOnTouchMove = new Dictionary<int, List<GameObject>>();
    Dictionary<int, GameObject> _selectedOnTouchBegan = new Dictionary<int, GameObject>();
    Dictionary<int, GameObject> _selectedOnTouchStationary = new Dictionary<int, GameObject>();
    Dictionary<int, GameObject> _selectedOnTouchOff = new Dictionary<int, GameObject>();

    //touch positions
    Vector2 _touchBeganPosition;
    Vector2 _touchEndedPosition;
    Vector2 _touchDirection;

    //for gestures
    float _pinchDifference;
    private void Awake()
    {
        touchResponse = GetComponent<ISelectionResponse>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0 && Input.touchCount <= maxAllowedTouch)
        {

            foreach (Touch touch in Input.touches)
            {
                //select object based on touch phase input.
                OnTouchBegan(touch);
                OnTouchStationary(touch);
                OnTouchMove(touch);
                OnTouchOff(touch);
            }
        }
    }

    void OnTouchBegan(Touch touch)
    {
        if (touch.phase == TouchPhase.Began)
        {
            int fingerId = touch.fingerId;
            Debug.Log($"touch began finger id {fingerId}", this);
            GameObject selectedObject = touchResponse.DetermineSelection(touch.position);

            if (!_currentSelection.ContainsKey(fingerId))
            {
                Debug.Log($"began id {fingerId} is added.", this);
                _currentSelection.Add(fingerId, selectedObject);
            }
            else
            {
                Debug.Log($"began id {fingerId} is replaced.", this);
                _currentSelection[fingerId] = selectedObject;
            }

            _selectedOnTouchBegan[fingerId] = _currentSelection[fingerId];
            if(_currentSelection[fingerId])
                touchResponse.IsSelected(_currentSelection[fingerId], touch.position);

            _touchBeganPosition = new Vector2(touch.position.x, touch.position.y);
        }
    }

    void OnTouchStationary(Touch touch)
    {
        if (touch.phase == TouchPhase.Stationary)
        {

            int fingerId = touch.fingerId;
            GameObject selectedObject = touchResponse.DetermineSelection(touch.position);
            if (!_currentSelection.ContainsKey(fingerId))
            {
                _currentSelection.Add(fingerId, selectedObject);
            }
            else
            {
                _currentSelection[fingerId] = selectedObject;
            }

            _hoverTime -= Time.deltaTime;
            if (_hoverTime <= 0)
            {
                Debug.Log($"touch stationary finger id {fingerId}", this);
                Debug.Log("touch stationary", this);
                if (_currentSelection[fingerId])
                    touchResponse.OnHoverSelected(_currentSelection[fingerId], touch.position);
                _selectedOnTouchStationary = _currentSelection;
                _hoverTime = timeOnHover;
            }
        }
    }

    void OnTouchMove(Touch touch)
    {
        if(touch.phase == TouchPhase.Moved)
        {
            int fingerId = touch.fingerId;
            Debug.Log($"touch move finger id {fingerId}", this);

            _hoverTime = timeOnHover;
            _selectedOnTouchStationary.Remove(fingerId);

            GameObject selectedObject = touchResponse.DetermineSelection(touch.position);

            if (!_selectedOnTouchMove.ContainsKey(fingerId))
                _selectedOnTouchMove.Add(fingerId, new List<GameObject>());
            if (!_currentSelection.ContainsKey(fingerId))
                _currentSelection.Add(fingerId, null);

            if (selectedObject)
            {
                if (_currentSelection[fingerId])
                {
                    if(_currentSelection[fingerId] != selectedObject)
                    {
                        if (enablePassiveSelection)
                        {
                            touchResponse.WasSelected(_currentSelection[fingerId], touch.position);
                        }
                        else
                        {
                            if (!enableDiselectOnlyOnTouchOff)
                                touchResponse.Deselected(_currentSelection[fingerId], touch.position);
                        }

                        //add current to touch move
                        if (_currentSelection[fingerId] && !_selectedOnTouchMove[fingerId].Contains(_currentSelection[fingerId]))
                            _selectedOnTouchMove[fingerId].Add(_currentSelection[fingerId]);
                        //assign selected as the new current object
                        _currentSelection[fingerId] = selectedObject;
                    }
                    //touch response
                    touchResponse.IsSelected(_currentSelection[fingerId], touch.position);
                }
                else
                {
                    touchResponse.IsSelected(selectedObject, touch.position);
                    _currentSelection[fingerId] = selectedObject;
                }

                // add current to touch move
                if (_currentSelection[fingerId] && !_selectedOnTouchMove[fingerId].Contains(_currentSelection[fingerId]))
                    _selectedOnTouchMove[fingerId].Add(_currentSelection[fingerId]);
            }
            else
            {
                if (enableDiselectOnlyOnTouchOff)
                {
                    //look for most recent item selected and make it current
                    if (!_currentSelection[fingerId])
                    {
                        int selectedOnTouchMoveCount = _selectedOnTouchMove[fingerId].Count;
                        if (selectedOnTouchMoveCount > 0)
                        {
                            _currentSelection[fingerId] = _selectedOnTouchMove[fingerId][selectedOnTouchMoveCount - 1];
                        }
                    }

                    if(_currentSelection[fingerId])
                        touchResponse.IsSelected(_currentSelection[fingerId], touch.position);
                }
                else
                {
                    //Allow null current selection
                    _currentSelection[fingerId] = selectedObject;
                }

                //add current to touch move
                if (_currentSelection[fingerId] && !_selectedOnTouchMove[fingerId].Contains(_currentSelection[fingerId]))
                    _selectedOnTouchMove[fingerId].Add(_currentSelection[fingerId]);
            }

            //if (!_currentSelection.ContainsKey(fingerId))
            //    _currentSelection.Add(fingerId, selectedObject);

            //if (_currentSelection[fingerId] && _currentSelection[fingerId] != selectedObject)
            //{
            //    if (!_selectedOnTouchMove[fingerId].Contains(_currentSelection[fingerId]))
            //        _selectedOnTouchMove[fingerId].Add(_currentSelection[fingerId]);

            //    if (enablePassiveSelection)
            //    {
            //        touchResponse.WasSelected(_currentSelection[fingerId], touch.position);
            //    }
            //    else
            //    {
            //        if (!enableDiselectOnlyOnTouchOff)
            //        {
            //            Debug.Log($"Deselected here", this);
            //            touchResponse.Deselected(_currentSelection[fingerId], touch.position);
            //        }
            //        else
            //        {
            //            touchResponse.IsSelected(_currentSelection[fingerId], touch.position);
            //        }
            //    }
            //}
            

            //if (_currentSelection[fingerId] != null && _currentSelection[fingerId] != selectedObject)
            //{
            //    if (!_selectedOnTouchMove[fingerId].Contains(_currentSelection[fingerId]))
            //        _selectedOnTouchMove[fingerId].Add(_currentSelection[fingerId]);
            //}

            //if (enableDiselectOnlyOnTouchOff)
            //{
            //    if (selectedObject)
            //    {
            //        touchResponse.IsSelected(selectedObject, touch.position);
            //        _currentSelection[fingerId] = selectedObject;
            //    }
            //    else
            //    {
            //        if (_currentSelection[fingerId])
            //        {
            //            touchResponse.IsSelected(_currentSelection[fingerId], touch.position);
            //        }
            //        else
            //        {

            //        }
            //    }
            //}
            //else
            //{
            //    Debug.Log($"current was replaced");
            //    //change current selection with new current.
            //    _currentSelection[fingerId] = selectedObject;
            //}
        }
    }

    void OnTouchOff(Touch touch)
    {
        if(touch.phase == TouchPhase.Ended)
        {
            int fingerId = touch.fingerId;
            Debug.Log($"touch off finger id {fingerId}", this);

            _hoverTime = timeOnHover;
            _selectedOnTouchOff[fingerId] = touchResponse.DetermineSelection(touch.position);
            _touchEndedPosition = new Vector2(touch.position.x, touch.position.y);
            _touchDirection = DetermineSwipeDirection(_touchBeganPosition, _touchEndedPosition);
            //Response
            if (_selectedOnTouchOff[fingerId])
            {
                if (enableLastTouchConfirm)
                {
                    touchResponse.OnSelectionConfirm(_selectedOnTouchOff[fingerId], touch.position);

                }
                else
                {
                    touchResponse.Deselected(_selectedOnTouchOff[fingerId], touch.position);
                }
            }


            if (_selectedOnTouchMove[fingerId].Count > 0 && enablePassiveSelection)
            {
                foreach (var gameObject in _selectedOnTouchMove[fingerId])
                {
                    //on deselect
                    if(gameObject)
                        touchResponse.Deselected(gameObject, touch.position);
                    //maybe remove it from list?
                }
            }

            if (_selectedOnTouchBegan[fingerId] && _selectedOnTouchBegan[fingerId] == _selectedOnTouchOff[fingerId])
                touchResponse.OnSelectionConfirm(_selectedOnTouchOff[fingerId], touch.position);
            if (_selectedOnTouchBegan[fingerId] && _selectedOnTouchBegan[fingerId] == _selectedOnTouchOff[fingerId] && _selectedOnTouchMove[fingerId].Count <= 1 && enableUnniqueSelection)
                touchResponse.IsSelectedUnique(_selectedOnTouchBegan[fingerId], touch.position);

            _currentSelection.Remove(fingerId);
            _selectedOnTouchBegan.Remove(fingerId);
            _selectedOnTouchStationary.Remove(fingerId);
            _selectedOnTouchMove[fingerId].Clear();
            _selectedOnTouchOff.Remove(fingerId);
        }
    }

    Vector2 DetermineSwipeDirection(Vector2 positionStart, Vector2 positionEnd)
    {
        if (positionStart != null && positionEnd != null)
            return positionEnd - positionStart;
        return new Vector2(0, 0);
    }

    void DetermineTapCount(Touch touch)
    {
        _tapTime -= Time.deltaTime;

        if(touch.phase == TouchPhase.Began)
        {
            if(_tapTime > 0)
            {
                _tapCount++;
            }
            else
            {
                _tapTime = timeOnTap;
                _tapCount = 0;
            }
        }
    }

    float GetPinchDifference(Touch firstTouch, Touch secondTouch, float pinchAllowance = 1f)
    {
        firstTouch = Input.GetTouch(0);
        secondTouch = Input.GetTouch(1);

        Vector2 firstTouchPreviousPosition = firstTouch.position - firstTouch.deltaPosition;
        Vector2 secondTouchPreviousPosition = secondTouch.position - secondTouch.deltaPosition;

        float touchDistancePreviousPosition = (firstTouchPreviousPosition - secondTouchPreviousPosition).magnitude;
        float touchDistanceCurrentPosition = (firstTouch.position - secondTouch.position).magnitude;

        float distanceDifferene = touchDistanceCurrentPosition - touchDistancePreviousPosition;

        //pinch allowance is for unexpected finer movement.
        if (distanceDifferene >= 0 - pinchAllowance && distanceDifferene <= 0 + pinchAllowance)
            return 0;

        //difference == 0 = noPinch
        //difference > 0 = pinchOut
        //difference < 0 = pinchIn
        return distanceDifferene;
    }
}
