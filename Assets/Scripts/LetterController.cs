using UnityEngine;

public class LetterController : MonoBehaviour
{
    public Letter letter;

    private AudioSource _audioSource;
    private SpriteRenderer _spriteRenderer;
    private ISelectionResponse _selectionResponse;

    private GameObject _selectedObjectBegin;
    private GameObject _selectedObjectEnd;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _selectionResponse = GetComponent<ISelectionResponse>();

    }
    private void Start()
    {
        //change based on options.
        _audioSource.clip = letter.PhonicAudio;
        //_spriteRenderer.sprite = letter.Sprite;
    }

    private void Update()
    {
        if(Input.touchCount > 0)
        {
            foreach (var touch in Input.touches)
            {

                OnTouchOff(touch);
                OnTouchMove(touch);
                OnTouchTap(touch);
                OnTouchBegin(touch);
            }
        }
    }

    private void OnTouchTap(Touch touch)
    {
        if (touch.tapCount == 1 && touch.phase == TouchPhase.Ended)
        {
            GameObject self = this.gameObject;
            if(_selectedObjectEnd == self && _selectedObjectBegin == self)
                _selectionResponse.OnSelectionConfirm(this.gameObject, touch.position);

        }
    }

    private void OnTouchBegin(Touch touch)
    {
        if (touch.phase == TouchPhase.Began)
        {
            _selectedObjectBegin = _selectionResponse.DetermineSelection(touch.position);
            if (_selectedObjectBegin == this.gameObject)
            {
                _selectionResponse.OnSelected(this.gameObject, touch.position);
            }
        }
    }


    private void OnTouchMove(Touch touch)
    {
        if(touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
        {
            GameObject selection = _selectionResponse.DetermineSelection(touch.position);
            if (selection && selection == this.gameObject)
                _selectionResponse.OnSelected(this.gameObject, touch.position);
            if(!selection || selection != this.gameObject)
                _selectionResponse.OnDeselect(this.gameObject);
        }
    }

    private void OnTouchOff(Touch touch)
    {
        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            _selectedObjectEnd = _selectionResponse.DetermineSelection(touch.position);
            _selectionResponse.OnDeselect(this.gameObject);
        }
    }
}
