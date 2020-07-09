using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WordController : MonoBehaviour
{
    public Word Word;
    public ISelectionResponse _selectionResponse;
    public GameObject LetterBlockPrefab;

    private AudioSource _audioSource;
    private SpriteRenderer _spriteRenderer;
    private string _selectedWord = "";
    private List<GameObject> _lettersGameObjectSelected = new List<GameObject>();


    //NOTE - will pick another random word if all the NECCESSARY assets are not present for the word scriptable object.
    //(WordAudio, Letters) - needed || - (Sfx, Sprite) may not be needed
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _selectionResponse = GetComponent<ISelectionResponse>();

    }

    private void Start()
    {
        //assign values here. - create a method that initializes so that it can be recalled when instantiating an object
        //if (Word)
        //{
        //    Debug.Log($"Initialize wordblock on start -{Word.WordSpelling}-");
        //    InitializeWord(Word.WordSpelling);
        //}
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

    //TODO: create touch movement for Sfx
    //TODO: create touch movement for when displaying picture sprite as a whole figure nto the letters.

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
        Debug.Log($"Initializing word -{wordString}-");
        if (String.IsNullOrWhiteSpace(wordString))
            throw new ArgumentNullException("wordString arguement cannot be null, as it is used to generate the word.");

        //create word class scriptable object
        Word wordScriptable = ScriptableObject.CreateInstance<Word>();
        Word = wordScriptable;

        //load reasource components & assign them
        Word.WordSpelling = wordString;
        Debug.Log($"finding components for {wordString}");
        Word.WordAudio = Resources.Load<AudioClip>($"Audio/Words/{wordString}");
        Debug.Log($"Audio/Words/{wordString}", this);

        Word.Sprite = Resources.Load<Sprite>($"Sprites/Words/{wordString}");
        Word.Sfx = Resources.Load<AudioClip>($"Audio/Sfxs/{wordString}");
        foreach (var character in wordString)
        {
            Debug.Log($"finding character letter for {character}");
            Letter letter = Resources.Load<Letter>($"Letters/{character}") ?? throw new NullReferenceException("letter cannot be null, it is the building block of the word.");

            if (!Word.Letters.Contains(letter))
                Word.Letters.Add(letter);
        }


        //change based on options.
        //male to female
        //find word audio using resource.load
        _audioSource.clip = Word.WordAudio;

        //initializing letters & place them in the world.
        InitializeLetters(wordScriptable);

        //return 0 if only necessary components are loaded
        //return 1 if all components are loaded
        if (wordScriptable.Sprite == null || wordScriptable.Sfx == null)
            return 0;

        //initializing the picture (proper placement);
        //change based on package
        //_spriteRenderer.sprite = letter.Sprite;

        return 1;
    }

    private void InitializeLetters(Word word)
    {
        Debug.Log($"Assembling letters -{word.WordSpelling}-");
        int lettersInstantiated = 0;
        float widthAllowance = LetterBlockPrefab.GetComponent<SpriteRenderer>().bounds.size.x;

        //assemble the word using the letters
        foreach (char character in word.WordSpelling)
        {
            Letter letter = word.Letters.Where(l => l.Symbol == character).FirstOrDefault();
            if (letter)
            {
                Vector3 objectPosition = new Vector3(transform.position.x + (widthAllowance * lettersInstantiated), transform.position.y, transform.position.z);

                GameObject letterGameObject = Instantiate(LetterBlockPrefab, transform);
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
        if(touch.phase == TouchPhase.Ended)
        {
            Debug.Log($"selected word on release -{_selectedWord}-", this);
            if(_selectedWord.Length > 1 && Word)
            {
                if (_selectedWord == Word.WordSpelling)
                {
                    if (!_audioSource.isPlaying)
                        _audioSource.Play();
                }
                else {

                    //find if there is any word that matches such spelling
                    //change PATH depending on the word searched. cv/cc/cvc/ etc.
                    //use word.letters to determin if vowel or consonant to get to the desired directory
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
