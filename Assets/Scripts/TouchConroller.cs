using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchConroller : MonoBehaviour
{
    public ITouchResponse touchResponse;
    public float secondsToWaitOnTouchOff = .5f;
    //public bool enablePinch; //to be implemented
    //public bool enableSwipe; //to be implemented
    public bool passiveSelection;
    public int maxAllowedTouch = 10;
    //public bool isUniqueResponse;
    public float timeOnHover = 2f;

    float _hoverTime;
    bool isClearing;
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

    float _pinchDifference;
    private void Awake()
    {
        touchResponse = GetComponent<ITouchResponse>();
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
        if (touch.phase == TouchPhase.Began && !isClearing)
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
        if (touch.phase == TouchPhase.Stationary && !isClearing)
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
        if(touch.phase == TouchPhase.Moved && !isClearing)
        {

            //already moving remove stationary object
            _selectedOnTouchStationary = null;
            Debug.Log("touch move", this);

            GameObject selectedObject = touchResponse.DetermineSelection(touch.position);
            if(selectedObject)
                touchResponse.IsSelected(selectedObject, touch.position);
            if(_currentSelection && _currentSelection != selectedObject)
            {
                if (passiveSelection)
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
        if(touch.phase == TouchPhase.Ended && !isClearing)
        {
            _selectedOnTouchOff = touchResponse.DetermineSelection(touch.position);
            if (_selectedOnTouchOff)
                touchResponse.Deselected(_selectedOnTouchOff, touch.position);
            _touchEndedPosition = new Vector2(touch.position.x, touch.position.y);
            _hoverTime = timeOnHover;


            if (_selectedOnTouchMove.Count > 0 && passiveSelection)
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
                touchResponse.SelectionConfirmed(_selectedOnTouchOff, touch.position);
            if (_selectedOnTouchBegan && _selectedOnTouchBegan == _selectedOnTouchOff && _selectedOnTouchMove.Count <= 1)
                touchResponse.IsSelectedUnique(_selectedOnTouchBegan, touch.position);

            _currentSelection = null;
            _selectedOnTouchBegan = null;
            _selectedOnTouchStationary = null;
            _selectedOnTouchMove.Clear();
            _selectedOnTouchOff = null;
        }
    }

    void DeterminDirection()
    {
        if (_touchBeganPosition != null && _touchEndedPosition != null)
            _touchDirection = _touchEndedPosition - _touchBeganPosition;
    }

    float TouchPinchDifferenceFromPrevious(Touch firstTouch, Touch secondTouch, float pinchAllowance)
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
