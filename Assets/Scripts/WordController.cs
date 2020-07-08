using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WordController : MonoBehaviour
{
    public Word word;
    public ISelectionResponse _selectionResponse;
    public GameObject LetterPrefab;

    private AudioSource _audioSource;
    private SpriteRenderer _spriteRenderer;
    private string _selectedWord = "";
    private List<GameObject> _lettersGameObjectSelected = new List<GameObject>();


    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _selectionResponse = GetComponent<ISelectionResponse>();
    }

    private void Start()
    {
        //change based on options.
        _audioSource.clip = word.WordAudio;
        //_spriteRenderer.sprite = letter.Sprite;

        InitializeLetters();
    }

    

    private void Update()
    {
        if(Input.touchCount > 0)
        {
            foreach (var touch in Input.touches)
            {
                OnTouchOff(touch);
                OnTouchMove(touch);
            }
        }
        
    }

    private void InitializeLetters()
    {
        int lettersInstantiated = 0;
        float widthAllowance = LetterPrefab.GetComponent<SpriteRenderer>().bounds.size.x;

        foreach (char character in word.WordSpelling)
        {
            Letter letter = word.letters.Where(l => l.Symbol == character).FirstOrDefault();
            if (letter)
            {
                Vector3 objectPosition = new Vector3(transform.position.x + (widthAllowance * lettersInstantiated), transform.position.y, transform.position.z);

                GameObject letterGameObject = Instantiate(LetterPrefab, transform);
                letterGameObject.transform.position = objectPosition;
                letterGameObject.GetComponent< LetterController>().letter = letter;
            }
            lettersInstantiated++;
        }
    }

    private void OnTouchMove(Touch touch)
    {
        if(touch.phase == TouchPhase.Moved && touch.tapCount == 1)
        {
            GameObject _selection = _selectionResponse.DetermineSelection(touch.position);

            if (!_selection)
                return;

            Letter letter = _selection.GetComponent<LetterController>().letter;
            if (letter == null)
                return;

            if (!_lettersGameObjectSelected.Contains(_selection))
            {
                _lettersGameObjectSelected.Add(_selection);
                _selectedWord = _selectedWord + letter.Symbol;

                Debug.Log($"current selected word -{_selectedWord}-", this);
            }

        }
    }

    private void OnTouchOff(Touch touch)
    {
        if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            Debug.Log($"selected word on release -{_selectedWord}-", this);
            if(_selectedWord.Length > 1)
            {
                if (_selectedWord == word.WordSpelling)
                {
                    if (!_audioSource.isPlaying)
                        _audioSource.Play();
                }
                else {
                    //find if there is any word that matches such spelling
                    AudioClip resourcedWord = Resources.Load<AudioClip>($"Audio/Words/{_selectedWord}");
                    if (resourcedWord && !_audioSource.isPlaying)
                    {
                        _audioSource.PlayOneShot(resourcedWord);
                    }

                }
            }
            
            _selectedWord = "";
            _lettersGameObjectSelected.Clear();

        }
    }
}
