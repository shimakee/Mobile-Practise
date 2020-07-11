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
    public int maxAllowedTouch = 10;
    public float timeOnHover = 2f;
    public float timeOnTap = .5f;

    //timers
    float _hoverTime;
    float _tapTime;
    int _tapCount;

    //objects selected at touch phases
    GameObject _currentSelection;
    public List<GameObject> _selectedOnTouchMove = new List<GameObject>();
    GameObject _selectedOnTouchBegan;
    GameObject _selectedOnTouchStationary;
    GameObject _selectedOnTouchOff;

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
            Debug.Log("touch began", this);

            _currentSelection = touchResponse.DetermineSelection(touch.position);
            touchResponse.IsSelected(_currentSelection, touch.position);
            _selectedOnTouchBegan = _currentSelection;

            _touchBeganPosition = new Vector2(touch.position.x, touch.position.y);
            Debug.Log($"on touch began {_selectedOnTouchBegan.gameObject.name}", this);
        }
    }

    void OnTouchStationary(Touch touch)
    {
        if (touch.phase == TouchPhase.Stationary)
        {

            _currentSelection = touchResponse.DetermineSelection(touch.position);

            _hoverTime -= Time.deltaTime;
            if (_hoverTime <= 0)
            {
                Debug.Log("touch stationary", this);
                if (_currentSelection)
                    touchResponse.OnHoverSelected(_currentSelection, touch.position);
                _selectedOnTouchStationary = _currentSelection;
                _hoverTime = timeOnHover;
            }
        }
    }

    void OnTouchMove(Touch touch)
    {
        if(touch.phase == TouchPhase.Moved)
        {

            //already moving remove stationary object

            _hoverTime = timeOnHover;
            _selectedOnTouchStationary = null;
            Debug.Log("touch move", this);

            GameObject selectedObject = touchResponse.DetermineSelection(touch.position);
            if(selectedObject)
                touchResponse.IsSelected(selectedObject, touch.position);
            if(_currentSelection && _currentSelection != selectedObject)
            {
                if (enablePassiveSelection)
                {
                    touchResponse.WasSelected(_currentSelection, touch.position);
                }
                else
                {
                    touchResponse.Deselected(_currentSelection, touch.position);
                }
            }

            //determine if previous current selection != new current selection
            if (_currentSelection != selectedObject && _currentSelection != null)
            {
                //if its a new current then add previous current to the list of selected objects (only if it was not yet added)
                if (!_selectedOnTouchMove.Contains(_currentSelection))
                    _selectedOnTouchMove.Add(_currentSelection);
            }

            //change current selection with new current.
            _currentSelection = selectedObject;
        }
    }

    void OnTouchOff(Touch touch)
    {
        if(touch.phase == TouchPhase.Ended)
        {
            _hoverTime = timeOnHover;
            _selectedOnTouchOff = touchResponse.DetermineSelection(touch.position);
            _touchEndedPosition = new Vector2(touch.position.x, touch.position.y);
            _touchDirection = DetermineSwipeDirection(_touchBeganPosition, _touchEndedPosition);
            //Response
            if (_selectedOnTouchOff)
            {
                if (enableLastTouchConfirm)
                {
                    touchResponse.OnSelectionConfirm(_selectedOnTouchOff, touch.position);

                }
                else
                {
                    touchResponse.Deselected(_selectedOnTouchOff, touch.position);
                }
            }


            if (_selectedOnTouchMove.Count > 0 && enablePassiveSelection)
            {
                foreach (var gameObject in _selectedOnTouchMove)
                {
                    //on deselect
                    if(gameObject)
                        touchResponse.Deselected(gameObject, touch.position);
                    //maybe remove it from list?
                }
            }

            if (_selectedOnTouchBegan && _selectedOnTouchBegan == _selectedOnTouchOff)
                touchResponse.OnSelectionConfirm(_selectedOnTouchOff, touch.position);
            if (_selectedOnTouchBegan && _selectedOnTouchBegan == _selectedOnTouchOff && _selectedOnTouchMove.Count <= 1 && enableUnniqueSelection)
                touchResponse.IsSelectedUnique(_selectedOnTouchBegan, touch.position);

            _currentSelection = null;
            _selectedOnTouchBegan = null;
            _selectedOnTouchStationary = null;
            _selectedOnTouchMove.Clear();
            _selectedOnTouchOff = null;
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
