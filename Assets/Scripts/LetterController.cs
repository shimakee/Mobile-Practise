using UnityEngine;

public class LetterController : MonoBehaviour
{
    public Letter letter;

    private AudioSource _audioSource;
    private SpriteRenderer _spriteRenderer;
    private ISelectionResponse _selectionResponse;

    private GameObject _selectedObjectBegin;
    private GameObject _selectedObjectEnd;

    //TODO: resources path set as public string to be set in editor
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _selectionResponse = GetComponent<ISelectionResponse>();

    }
    private void Start()
    {
        InitializeLetter(letter);
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

    public void InitializeLetter(Letter letter) // create method overload if necessary
    {
        //using resource.load change audio to
        //male or female
        //phonics or letter pronounciation
        _audioSource.clip = letter.PhonicAudio;

        if (letter.Sprite == null)
            letter.Sprite = Resources.Load<Sprite>($"Sprites/{letter.Symbol}");

            _spriteRenderer.sprite = letter.Sprite;
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
                _selectionResponse.IsSelected(this.gameObject, touch.position);
            }
        }
    }


    private void OnTouchMove(Touch touch)
    {
        if(touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
        {
            GameObject selection = _selectionResponse.DetermineSelection(touch.position);
            if (selection && selection == this.gameObject)
                _selectionResponse.IsSelected(this.gameObject, touch.position);
            if(!selection || selection != this.gameObject)
                _selectionResponse.Deselected(this.gameObject, touch.position);
        }
    }

    private void OnTouchOff(Touch touch)
    {
        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            _selectedObjectEnd = _selectionResponse.DetermineSelection(touch.position);
            _selectionResponse.Deselected(this.gameObject, touch.position);
        }
    }
}
