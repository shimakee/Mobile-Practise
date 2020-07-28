using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchConroller : MonoBehaviour
{
    public bool enablePassiveSelection;
    public bool enableUnniqueSelection;
    public bool enableLastTouchConfirm;
    public bool enableDiselectOnlyOnTouchOff;
    public int maxAllowedTouch = 10;
    public float timeOnHover = 2f;
    public float timeOnTap = .5f;

    //Selector and responses
    private IObjectSelector _objectSelector;
    private ISelectionResponse _selectionResponse;
    private ITouchResponse _touchResponse;

    //contorl options
    ITouchControlOptions _touchControlOptions;

    //timers
    Dictionary<int, float> _hoverTime = new Dictionary<int, float>();
    Dictionary<int, float> _tapTime = new Dictionary<int, float>();
    Dictionary<int, int> _tapCount = new Dictionary<int, int>();

    //objects selected at touch phases
    Dictionary<int, GameObject> _currentSelection = new Dictionary<int, GameObject>();
    public Dictionary<int, List<GameObject>> _selectedOnTouchMove = new Dictionary<int, List<GameObject>>();
    Dictionary<int, GameObject> _selectedOnTouchBegan = new Dictionary<int, GameObject>();
    Dictionary<int, GameObject> _selectedOnTouchStationary = new Dictionary<int, GameObject>();
    Dictionary<int, GameObject> _selectedOnTouchOff = new Dictionary<int, GameObject>();

    //touch positions
    Dictionary<int, Vector2> _touchBeganPosition = new Dictionary<int, Vector2>();
    Dictionary<int, Vector2> _touchEndedPosition = new Dictionary<int, Vector2>();
    Dictionary<int, Vector2> _touchDirection = new Dictionary<int, Vector2>();

    //for gestures
    float _pinchDifference;
    private void Awake()
    {
        _selectionResponse = GetComponent<ISelectionResponse>();
        _objectSelector = GetComponent<IObjectSelector>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && Input.touchCount <= maxAllowedTouch)
        {

            foreach (Touch touch in Input.touches)
            {
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    return;

                //select object based on touch phase input.
                OnTouchBegan(touch);
                OnTouchStationary(touch);
                OnTouchMove(touch);
                OnTouchOff(touch);
                //DetermineSwipeDirection(touch);
            }
        }
    }

    void OnTouchBegan(Touch touch)
    {
        int fingerId = touch.fingerId;

        if (touch.phase == TouchPhase.Began)
        {
            GameObject selectedObject = _objectSelector.DetermineSelection(touch.position);

            //assigning current touch objects
            if (!_currentSelection.ContainsKey(fingerId))
            {
                _currentSelection.Add(fingerId, selectedObject);
            }
            else
            {
                _currentSelection[fingerId] = selectedObject;
            }

            //assign selected object as beginning object selected
            _selectedOnTouchBegan[fingerId] = _currentSelection[fingerId];
            //selection respond to object being actively selected
            if (_currentSelection[fingerId])
            {
                _selectionResponse.IsSelected(_currentSelection[fingerId], touch.position);
            }
        }
    }

    void OnTouchStationary(Touch touch)
    {
        int fingerId = touch.fingerId;

        if (touch.phase == TouchPhase.Began)
        {
            //set initial hover time on touch began
            if (!_hoverTime.ContainsKey(fingerId))
            {
                _hoverTime.Add(fingerId, timeOnHover);
            }
            else
            {
                _hoverTime[fingerId] = timeOnHover;
            }
        }

        if(touch.phase == TouchPhase.Moved)
        {
            //reset hover time
            _hoverTime[fingerId] = timeOnHover;
            //reset stationary
            _selectedOnTouchStationary.Remove(fingerId);
        }

        if (touch.phase == TouchPhase.Stationary)
        {
            GameObject selectedObject = _objectSelector.DetermineSelection(touch.position);

            if (!_currentSelection.ContainsKey(fingerId))
            {
                _currentSelection.Add(fingerId, selectedObject);
            }
            else
            {
                _currentSelection[fingerId] = selectedObject;
            }

            //reduce hover time per tic
            _hoverTime[fingerId] -= Time.deltaTime;
            //determine reponse on stationary
            if (_hoverTime[fingerId] <= 0)
            {
                if (_currentSelection[fingerId])
                    _selectionResponse.OnHoverSelected(_currentSelection[fingerId], touch.position);
                if (!_selectedOnTouchStationary.ContainsKey(fingerId))
                {
                    _selectedOnTouchStationary.Add(fingerId, _currentSelection[fingerId]);
                }
                else
                {
                    _selectedOnTouchStationary[fingerId] = _currentSelection[fingerId];
                }
            }
        }
    }

    void OnTouchMove(Touch touch)
    {
        int fingerId = touch.fingerId;

        if (touch.phase == TouchPhase.Moved)
        {
            GameObject selectedObject = _objectSelector.DetermineSelection(touch.position);

            //determine if touch control option exist
            if (selectedObject)
            {
                _touchControlOptions = selectedObject.GetComponent<ITouchControlOptions>();
            }
            else
            {
                if(_currentSelection[fingerId])
                    _touchControlOptions = _currentSelection[fingerId].GetComponent<ITouchControlOptions>();
            }

            //predetermine if keys already exists
            if (!_selectedOnTouchMove.ContainsKey(fingerId))
                _selectedOnTouchMove.Add(fingerId, new List<GameObject>());
            if (!_currentSelection.ContainsKey(fingerId))
                _currentSelection.Add(fingerId, null);

            //determine response on touch move
            if (selectedObject)
            {
                if (_currentSelection[fingerId])
                {
                    if(_currentSelection[fingerId] != selectedObject)
                    {
                        if (_touchControlOptions == null)
                        {
                            if (enablePassiveSelection)
                            {
                                _selectionResponse.WasSelected(_currentSelection[fingerId], touch.position);
                            }
                            else
                            {
                                if (!enableDiselectOnlyOnTouchOff)
                                {
                                    _selectionResponse.Deselected(_currentSelection[fingerId], touch.position);
                                }
                            }
                        }
                        else
                        {
                            if (_touchControlOptions.enablePassiveSelection)
                            {
                                _selectionResponse.WasSelected(_currentSelection[fingerId], touch.position);
                            }
                            else
                            {
                                if (!_touchControlOptions.enableDiselectOnlyOnTouchOff)
                                {
                                    _selectionResponse.Deselected(_currentSelection[fingerId], touch.position);
                                }
                            }
                        }

                        //add current to touch move
                        if (_currentSelection[fingerId] && !_selectedOnTouchMove[fingerId].Contains(_currentSelection[fingerId]))
                            _selectedOnTouchMove[fingerId].Add(_currentSelection[fingerId]);

                        //assign selected as the new current object
                        _currentSelection[fingerId] = selectedObject;
                    }

                    //touch response
                    _selectionResponse.IsSelected(_currentSelection[fingerId], touch.position);

                    //add selected object to current
                    if (!_selectedOnTouchMove[fingerId].Contains(selectedObject))
                        _selectedOnTouchMove[fingerId].Add(selectedObject);
                }
                else
                {
                    _selectionResponse.IsSelected(selectedObject, touch.position);
                    _currentSelection[fingerId] = selectedObject;
                }

                // add current to touch move
                if (_currentSelection[fingerId] && !_selectedOnTouchMove[fingerId].Contains(_currentSelection[fingerId]))
                    _selectedOnTouchMove[fingerId].Add(_currentSelection[fingerId]);
            }
            else
            {
                if (_touchControlOptions == null)
                {
                    if (enableDiselectOnlyOnTouchOff)
                    {
                        //look for most recent item selected and make it current
                        //if (!_currentSelection[fingerId])
                        //{
                        //    int selectedOnTouchMoveCount = _selectedOnTouchMove[fingerId].Count;
                        //    if (selectedOnTouchMoveCount > 0)
                        //    {
                        //        _currentSelection[fingerId] = _selectedOnTouchMove[fingerId][selectedOnTouchMoveCount - 1];
                        //    }
                        //}

                        if (enablePassiveSelection)
                        {
                            if (_currentSelection[fingerId])
                                _selectionResponse.WasSelected(_currentSelection[fingerId], touch.position);
                        }
                        else
                        {
                            if (_currentSelection[fingerId])
                                _selectionResponse.IsSelected(_currentSelection[fingerId], touch.position);
                        }
                    }
                    else
                    {
                        if (enablePassiveSelection)
                        {
                            if (_currentSelection[fingerId])
                                _selectionResponse.WasSelected(_currentSelection[fingerId], touch.position);
                        }
                        else
                        {
                            if (_currentSelection[fingerId])
                                _selectionResponse.Deselected(_currentSelection[fingerId], touch.position);
                        }

                        //Allow null current selection
                        _currentSelection[fingerId] = selectedObject;
                    }
                }
                else
                {
                    if (_touchControlOptions.enableDiselectOnlyOnTouchOff)
                    {
                        if (_touchControlOptions.enablePassiveSelection)
                        {
                            if (_currentSelection[fingerId])
                                _selectionResponse.WasSelected(_currentSelection[fingerId], touch.position);
                        }
                        else
                        {
                            if (_currentSelection[fingerId])
                                _selectionResponse.IsSelected(_currentSelection[fingerId], touch.position);
                        }
                    }
                    else
                    {
                        if (_touchControlOptions.enablePassiveSelection)
                        {
                            if (_currentSelection[fingerId])
                                _selectionResponse.WasSelected(_currentSelection[fingerId], touch.position);
                        }
                        else
                        {
                            if (_currentSelection[fingerId])
                                _selectionResponse.Deselected(_currentSelection[fingerId], touch.position);
                        }

                        //Allow null current selection
                        _currentSelection[fingerId] = selectedObject;
                    }
                }

                //add current to touch move
                if (_currentSelection[fingerId] && !_selectedOnTouchMove[fingerId].Contains(_currentSelection[fingerId]))
                    _selectedOnTouchMove[fingerId].Add(_currentSelection[fingerId]);
            }
        }
    }

    void OnTouchOff(Touch touch)
    {
        int fingerId = touch.fingerId;

        if (touch.phase == TouchPhase.Ended ||  touch.phase == TouchPhase.Canceled)
        {
            GameObject selectedObject = _objectSelector.DetermineSelection(touch.position);
            if (selectedObject)
            {
                _touchControlOptions = selectedObject.GetComponent<ITouchControlOptions>();
            }
            else
            {
                if (_currentSelection.ContainsKey(fingerId))
                {
                    if (_currentSelection[fingerId])
                        _touchControlOptions = _currentSelection[fingerId].GetComponent<ITouchControlOptions>();
                }
            }

            if (!_selectedOnTouchOff.ContainsKey(fingerId))
            {
                _selectedOnTouchOff.Add(fingerId, selectedObject);
            }
            else
            {
                _selectedOnTouchOff[fingerId] = selectedObject;
            }

            //check it exist - touch off might happen before you could move - like taps
            if (_selectedOnTouchMove.ContainsKey(fingerId))
            {
                if(_selectedOnTouchMove[fingerId] != null)
                {
                    //clean was selected responses to deselected responses
                    if (_selectedOnTouchMove[fingerId].Count > 0)
                    {
                        foreach (var gameObject in _selectedOnTouchMove[fingerId])
                        {
                            //on deselect
                            if (gameObject)
                            {
                                _selectionResponse.Deselected(gameObject, touch.position);
                            }
                        }
                    }
                }
            }
            else
            {

                _selectedOnTouchMove.Add(fingerId, new List<GameObject>());
            }

            //Determine response to touch ended && touch phase cancelled
            if (_selectedOnTouchOff[fingerId])
            {
                if (_touchControlOptions == null)
                {
                    if (enableLastTouchConfirm)
                    {
                        if (_selectedOnTouchMove[fingerId].Count > 0)
                        {
                            _selectionResponse.OnSelectionConfirm(_selectedOnTouchOff[fingerId], touch.position, _selectedOnTouchMove[fingerId]);
                        }
                        else
                        {
                            _selectionResponse.OnSelectionConfirm(_selectedOnTouchOff[fingerId], touch.position);
                        }
                    }
                    else
                    {
                        _selectionResponse.Deselected(_selectedOnTouchOff[fingerId], touch.position);
                    }
                }
                else
                {
                    if (_touchControlOptions.enableLastTouchConfirm)
                    {
                        if (_selectedOnTouchMove[fingerId].Count > 0)
                        {
                            _selectionResponse.OnSelectionConfirm(_selectedOnTouchOff[fingerId], touch.position, _selectedOnTouchMove[fingerId]);
                        }
                        else
                        {
                            _selectionResponse.OnSelectionConfirm(_selectedOnTouchOff[fingerId], touch.position);
                        }
                    }
                    else
                    {
                        _selectionResponse.Deselected(_selectedOnTouchOff[fingerId], touch.position);
                    }
                }
            }
            else
            {
                if (_currentSelection[fingerId])
                {
                    if (_touchControlOptions == null)
                    { 
                        if (enableLastTouchConfirm)
                        {
                            if (_selectedOnTouchMove[fingerId].Count > 0)
                            {
                                _selectionResponse.OnSelectionConfirm(_currentSelection[fingerId], touch.position, _selectedOnTouchMove[fingerId]);
                            }
                            else
                            {
                                _selectionResponse.OnSelectionConfirm(_currentSelection[fingerId], touch.position);
                            }
                        }
                        else
                        {
                            _selectionResponse.Deselected(_currentSelection[fingerId], touch.position);
                        }
                    }
                    else
                    {
                        if (_touchControlOptions.enableLastTouchConfirm)
                        {
                            if (_selectedOnTouchMove[fingerId].Count > 0)
                            {
                                _selectionResponse.OnSelectionConfirm(_currentSelection[fingerId], touch.position, _selectedOnTouchMove[fingerId]);
                            }
                            else
                            {
                                _selectionResponse.OnSelectionConfirm(_currentSelection[fingerId], touch.position);
                            }
                        }
                        else
                        {
                            _selectionResponse.Deselected(_currentSelection[fingerId], touch.position);
                        }
                    }
                }
            }

            //determine touch end response confirmation
            if (_selectedOnTouchBegan[fingerId] && _selectedOnTouchBegan[fingerId] == _selectedOnTouchOff[fingerId])
            {
                if (_selectedOnTouchMove[fingerId].Count > 0)
                {
                    _selectionResponse.OnSelectionConfirm(_selectedOnTouchOff[fingerId], touch.position, _selectedOnTouchMove[fingerId]);
                }
                else
                {
                    _selectionResponse.OnSelectionConfirm(_selectedOnTouchOff[fingerId], touch.position);
                }
            }

            if(_touchControlOptions == null)
            {
                if (enableUnniqueSelection)
                {
                    if (_selectedOnTouchBegan[fingerId] && _selectedOnTouchBegan[fingerId] == _selectedOnTouchOff[fingerId] && _selectedOnTouchMove[fingerId].Count <= 1)
                        _selectionResponse.IsSelectedUnique(_selectedOnTouchBegan[fingerId], touch.position);
                }
            }
            else
            {
                if (_touchControlOptions.enableUnniqueSelection)
                {
                    if (_selectedOnTouchBegan[fingerId] && _selectedOnTouchBegan[fingerId] == _selectedOnTouchOff[fingerId] && _selectedOnTouchMove[fingerId].Count <= 1)
                        _selectionResponse.IsSelectedUnique(_selectedOnTouchBegan[fingerId], touch.position);
                }
            }

            //reset on touch off
            //clear all previously assigned touches.
            _currentSelection[fingerId] = null;
            _selectedOnTouchBegan[fingerId] = null;
            _selectedOnTouchStationary[fingerId] = null;
            _selectedOnTouchMove[fingerId].Clear();
            _selectedOnTouchOff[fingerId] = null;
        }


    }

    void DetermineSwipeDirection(Touch touch)
    {
        int fingerId = touch.fingerId;

        if(touch.phase == TouchPhase.Began)
        {
            //determining swipe direction
            if (!_touchBeganPosition.ContainsKey(fingerId))
            {
                _touchBeganPosition.Add(fingerId, new Vector2(touch.position.x, touch.position.y));
            }
            else
            {
                _touchBeganPosition[fingerId] = touch.position;
            }
        }

        if(touch.phase == TouchPhase.Ended)
        {
            //determin touch end position - used to determine swipe direction
            if (!_touchEndedPosition.ContainsKey(fingerId))
            {
                _touchEndedPosition.Add(fingerId, new Vector2(touch.position.x, touch.position.y));
            }
            else
            {
                _touchEndedPosition[fingerId] = touch.position;
            }
        }

        //determining swipe direction
        if (!_touchDirection.ContainsKey(fingerId))
        {
            _touchDirection.Add(fingerId, CalculateSwipeDirection(_touchBeganPosition[fingerId], _touchEndedPosition[fingerId]));
        }
        else
        {
            _touchDirection[fingerId] = CalculateSwipeDirection(_touchBeganPosition[fingerId], _touchEndedPosition[fingerId]);
        }
    }

    Vector2 CalculateSwipeDirection(Vector2 positionStart, Vector2 positionEnd, float swipeJitterAllowance = 1f)
    {
        if (positionStart != null && positionEnd != null)
        {
            Vector2 swipeDirection = positionEnd - positionStart;
            if (swipeDirection.magnitude > swipeJitterAllowance)
                return swipeDirection;
        }

        return new Vector2(0, 0);
    }

    float GetPinchDifference(Touch firstTouch, Touch secondTouch, float pinchJitterAllowance = 1f)
    {
        firstTouch = Input.GetTouch(0);
        secondTouch = Input.GetTouch(1);

        Vector2 firstTouchPreviousPosition = firstTouch.position - firstTouch.deltaPosition;
        Vector2 secondTouchPreviousPosition = secondTouch.position - secondTouch.deltaPosition;

        float touchDistancePreviousPosition = (firstTouchPreviousPosition - secondTouchPreviousPosition).magnitude;
        float touchDistanceCurrentPosition = (firstTouch.position - secondTouch.position).magnitude;

        float distanceDifferene = touchDistanceCurrentPosition - touchDistancePreviousPosition;

        //pinch allowance is for unexpected finer movement.
        if (distanceDifferene >= 0 - pinchJitterAllowance && distanceDifferene <= 0 + pinchJitterAllowance)
            return 0;

        //difference == 0 = noPinch
        //difference > 0 = pinchOut
        //difference < 0 = pinchIn
        return distanceDifferene;
    }

    //void DetermineTapCount(Touch touch)
    //{
    //    _tapTime -= Time.deltaTime;

    //    if(touch.phase == TouchPhase.Began)
    //    {
    //        if(_tapTime > 0)
    //        {
    //            _tapCount++;
    //        }
    //        else
    //        {
    //            _tapTime = timeOnTap;
    //            _tapCount = 0;
    //        }
    //    }
    //}
}
