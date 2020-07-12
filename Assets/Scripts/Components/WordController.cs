using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WordController : MonoBehaviour
{
    public Word Word;
    public GameObject LetterBlockPrefab;

    private ISelectionResponse _selectionResponse;
    private IObjectSelector _objectSelector;
    private AudioSource _audioSource;
    private SpriteRenderer _spriteRenderer;
    private string _selectedWord = "";
    private List<GameObject> _lettersGameObjectSelected = new List<GameObject>();
    private GameObject _singleGameObjectSelected;

    //TODO: resources path set as public string to be set in editor

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _selectionResponse = GetComponent<ISelectionResponse>();
        _objectSelector = GetComponent<IObjectSelector>();
    }

    private void Update()
    {
        if(Input.touchCount > 0)
        {
            foreach (var touch in Input.touches)
            {
                OnTouchBegin(touch);
                OnTouchMove(touch);
                OnTouchOff(touch);
            }
        }
        
    }


    /// <summary>
    /// <para>Initializes the wordBlock game object based on the the word arguement.</para>
    /// <para>assigns the componenets or assets needed by the components.</para>
    /// <para>Returns 0 if only necessary assets are loaded.</para>
    /// <para>Returns 1 if all assets are loaded.</para>
    /// </summary>
    /// <param name="wordString"></param>
    /// <returns>Returns an int 0 or 1</returns>
    public int InitializeWord(string wordString) //create method overload if necessary
    {
        if (String.IsNullOrWhiteSpace(wordString))
            throw new ArgumentNullException("wordString arguement cannot be null, as it is used to generate the word.");

        //create word class scriptable object
        Word wordScriptable = ScriptableObject.CreateInstance<Word>();
        Word = wordScriptable;

        //load reasource
        Word.WordSpelling = wordString;
        Word.WordAudio = Resources.Load<AudioClip>($"Audio/Words/{wordString}");
        Word.Sprite = Resources.Load<Sprite>($"Sprites/{wordString}");
        Word.Sfx = Resources.Load<AudioClip>($"Audio/Sfxs/sfx_{wordString}");
        foreach (var character in wordString)
        {
            Letter letter = Resources.Load<Letter>($"Scripts/Letters/{character}") ?? throw new NullReferenceException("letter cannot be null, it is the building block of the word.");

            if (!Word.Letters.Contains(letter))
                Word.Letters.Add(letter);
        }

        //assign components
        _audioSource.clip = Word.WordAudio;

        //initializing letters & place them in the world.
        InitializeLetters(Word);
        InitializeWordImage(Word);

        //return 0 if only necessary components are loaded
        //return 1 if all components are loaded
        if (wordScriptable.Sprite == null || wordScriptable.Sfx == null)
            return 0;

        return 1;
    }

    private void InitializeWordImage(Word word) //create overload for offset and margins?
    {
        if(word.Sprite != null)
        {
            _spriteRenderer.sprite = word.Sprite;
        }
    }

    private void InitializeLetters(Word word) // create overload for offset and margins?
    {
        int lettersInstantiated = 0;
        var letterBlockSpriteRenderer = LetterBlockPrefab.GetComponent<SpriteRenderer>();
        float widthAllowance = letterBlockSpriteRenderer.bounds.size.x;
        float heightAllowance = letterBlockSpriteRenderer.bounds.size.y;

        //assemble the word using the letters
        foreach (char character in word.WordSpelling)
        {
            Letter letter = word.Letters.Where(l => l.Symbol == character).FirstOrDefault();
            if (letter)
            {
                Vector3 objectPosition = new Vector3(transform.position.x + (widthAllowance * lettersInstantiated) - widthAllowance, transform.position.y - heightAllowance, transform.position.z);

                GameObject letterGameObject = Instantiate(LetterBlockPrefab, transform);
                letterGameObject.transform.position = objectPosition;
                letterGameObject.GetComponent<ILetterSelectionResponse>().Letter = letter;
            }
            lettersInstantiated++;
        }
    }

    private void OnTouchBegin(Touch touch)
    {
        if (touch.tapCount == 1 && touch.phase == TouchPhase.Began)
        {
            _singleGameObjectSelected = _objectSelector.DetermineSelection(touch.position);
            if (!_singleGameObjectSelected)
                return;

            if(_singleGameObjectSelected == this.gameObject)
                _selectionResponse.IsSelected(_singleGameObjectSelected, touch.position);
        }
    }

    private void OnTouchMove(Touch touch)
    {
        if(touch.phase == TouchPhase.Moved)
        {
            GameObject selection = _objectSelector.DetermineSelection(touch.position);
            //for the picture
            if (selection && selection == this.gameObject)
            {
                _selectionResponse.IsSelected(this.gameObject, touch.position);
                _selectedWord = "";
                _lettersGameObjectSelected.Clear();
                return;
            }
            else
            {
                _singleGameObjectSelected = selection;
                _selectionResponse.Deselected(this.gameObject, touch.position);
            }

            //for the letters word
            if (selection)
            {
                ILetterSelectionResponse letterController = selection.GetComponent<ILetterSelectionResponse>();
                if (letterController == null)
                {
                    _selectedWord = "";
                    _lettersGameObjectSelected.Clear();
                    return;
                }

                if (!_lettersGameObjectSelected.Contains(selection))
                {
                    _lettersGameObjectSelected.Add(selection);
                    _selectedWord = _selectedWord + letterController.Letter.Symbol;
                }
            }


        }
    }

    private void OnTouchOff(Touch touch)
    {
        if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {

            _selectionResponse.Deselected(this.gameObject, touch.position);

            GameObject selectionEnd = _objectSelector.DetermineSelection(touch.position);
            
            //for the image 
            if (_singleGameObjectSelected == selectionEnd && _singleGameObjectSelected == this.gameObject)
            {
                if (Word.Sfx != null && !_audioSource.isPlaying)
                    _audioSource.PlayOneShot(Word.Sfx);
            }

            //for the letters word
            if (_selectedWord.Length <= 1 || !Word)
                return;

            if (selectionEnd && selectionEnd.GetComponent<ILetterSelectionResponse>() == null)
            {
                _selectedWord = "";
                _lettersGameObjectSelected.Clear();
            }

            if (_selectedWord == Word.WordSpelling)
            {
                _selectionResponse.OnSelectionConfirm(this.gameObject, transform.position);
            }
            else {
                //check for cvc folders when it gets bigger
                AudioClip resourcedWord = Resources.Load<AudioClip>($"Audio/Words/{_selectedWord}");
                if (resourcedWord && !_audioSource.isPlaying)
                {
                    _audioSource.PlayOneShot(resourcedWord);
                }

            }

            _selectedWord = "";
            _lettersGameObjectSelected.Clear();
        }
    }
}
