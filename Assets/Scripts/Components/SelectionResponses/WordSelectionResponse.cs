using System;
using System.Collections.Generic;
using UnityEngine;

public class WordSelectionResponse : MonoBehaviour, IWordSelectionResponse
{
    public Word WordScriptable; // it needs to be public so that unityeditor can assign scriptable objects
    public Word Word
    {
        get { return WordScriptable; }
        set { WordScriptable = value; }
    }
    public GameObject LetterBlockPrefab;

    //audio options
    public GameOptions GameOptions;

    //allowance for spacing
    public int WorldUnitSize = 10;
    [Range(0, 1)] public float PerLetterMargin = 0;
    [Range(0, 5)] public float Allowance = .5f;

    private string _currentWord = "";
    protected AudioSource _audioSource;
    protected AudioManager _audioManager;
    private List<ILetterSelectionResponse> _lettersGameObjectSelected = new List<ILetterSelectionResponse>();

    protected ILetterSelectionResponse[] _letterChildren;

    //private Color32 _ColorDeselect = Color.white;
    //private Color32 _ColorActive = new Color32(74, 150, 214, 255);
    //private Color32 _ColorWasActive = new Color32(155, 191, 221, 255);

    private void Awake()
    {
        _audioManager = FindObjectOfType<AudioManager>();
        _audioSource = GetComponent<AudioSource>();

        if(!_audioManager)
            throw new NullReferenceException("no audio manager comppnent attached.");
        if (!_audioSource)
            throw new NullReferenceException("no audio source comppnent attached.");
        if (!GameOptions)
            throw new NullReferenceException("no game options to load.");
    }

    /// <summary>
    /// <para>Initializes the wordBlock game object based on the the word arguement.</para>
    /// <para>assigns the componenets or assets needed by the components.</para>
    /// <para>Returns 0 if only necessary assets are loaded.</para>
    /// <para>Returns 1 if all assets are loaded.</para>
    /// </summary>
    /// <param name="wordString"></param>
    /// <returns>Returns an int 0 or 1</returns>
    public int Initialize(string wordString) //create method overload if necessary
    {
        if (String.IsNullOrWhiteSpace(wordString))
            throw new ArgumentNullException("wordString arguement cannot be null, as it is used to generate the word.");

        //name your word object
        transform.name = wordString;

        //create word class scriptable object
        if (Word == null)
            Word = ScriptableObject.CreateInstance<Word>();

        //load reasource
        if(String.IsNullOrWhiteSpace(Word.WordSpelling))
            Word.WordSpelling = wordString;

        if(Word.WordAudio == null)
            Word.WordAudio = Resources.Load<AudioClip>($"Packages/{GameOptions.VoicePackage}/audio/words/{wordString}");
        if(Word.Sprite == null)
            Word.Sprite = Resources.Load<Sprite>($"Sprites/default/{wordString}");
        if(Word.Sfx == null)
            Word.Sfx = Resources.Load<AudioClip>($"Audio/Sfxs/sfx_{wordString}");

        //assign components
        _audioSource.clip = Word.WordAudio;

        //initializing letters & place them in the world.
        AssembleLetters(Word.WordSpelling);

        //return 0 if only necessary components are loaded
        //return 1 if all components are loaded
        if (Word.Sprite == null || Word.Sfx == null)
            return 0;

        return 1;
    }
    protected virtual void AssembleLetters(string word) // create overload for offset and margins?
    {
        var letterBlockSpriteRenderer = LetterBlockPrefab.GetComponent<RectTransform>();
        int length = word.Length;
        _letterChildren = new ILetterSelectionResponse[length];
        float scale = CalculateScale(word);

        //float letterWidth = letterBlockSpriteRenderer.rect.width * scale; // for starting position
        float letterWidth = ((letterBlockSpriteRenderer.rect.width / Screen.width) * WorldUnitSize) * scale;
        float initialAllowanceToCenterPosition = ((letterWidth * length)  + (PerLetterMargin * length-1))/2 - (letterWidth / 2); //less half since pivot point is at the center.

        //assemble the word using the letters
        int lettersInstantiated = 0;
        for (int i = 0; i < length; i++)
        {
            Vector3 objectPosition = new Vector3(transform.position.x + (letterWidth * lettersInstantiated) - initialAllowanceToCenterPosition, transform.position.y, transform.position.z);
            objectPosition.x += PerLetterMargin * lettersInstantiated;
            GameObject letterGameObject = Instantiate(LetterBlockPrefab, transform);
            letterGameObject.transform.position = objectPosition;
            letterGameObject.transform.localScale = new Vector2(scale, scale);
            ILetterSelectionResponse letterComponent = letterGameObject.GetComponent<ILetterSelectionResponse>();
            if (letterComponent != null)
            {
                letterComponent.Initialize(word[i]);
                _letterChildren[i] = letterComponent;
            }
            lettersInstantiated++;
        }
    }

    public void IsSelected(GameObject gameObject, Vector3 inputPosition)
    {
        //this.gameObject.transform.localScale = new Vector3(1.3f, 1.3f, 0);
    }

    public void OnHoverSelected(GameObject gameObject, Vector3 inputPosition)
    {
        //do nothing
    }

    public void WasSelected(GameObject gameObject, Vector3 inputPosition)
    {
        //this.gameObject.transform.localScale = new Vector3(1.1f, 1.1f, 0);
    }

    public void Deselected(GameObject gameObject, Vector3 inputPosition)
    {
        //return to original scale
        //this.gameObject.transform.localScale = new Vector3(1, 1, 0);
    }

    public void OnSelectionConfirm(GameObject gameObject, Vector3 inputPosition)
    {
        //return to original scale
        //this.gameObject.transform.localScale = new Vector3(1, 1, 0);

        this.OnSelectionConfirm(gameObject, inputPosition, new List<GameObject>());
    }

    public void OnSelectionConfirm(GameObject gameObject, Vector3 inputPosition, List<GameObject> wasSelectedGameObjects)
    {
        //return to original scale
        //this.gameObject.transform.localScale = new Vector3(1, 1, 0);

        //check if selected matches.
        //if (_lettersGameObjectSelected.Count == Word.WordSpelling.Length && wasSelectedGameObjects.Count == Word.WordSpelling.Length)

        if (_lettersGameObjectSelected.Count == Word.WordSpelling.Length)
        {
            for (int i = 0; i < _lettersGameObjectSelected.Count; i++)
            {
                if (_lettersGameObjectSelected[i].Letter.Symbol != Word.WordSpelling[i])
                {
                    _currentWord = "";
                    _lettersGameObjectSelected.Clear();
                    return;
                }
            }

            //play audio
            if (!_audioSource.isPlaying)
            {
                _audioSource.Play();
            }
        }
        else
        {

            //if (_lettersGameObjectSelected.Count < Word.WordSpelling.Length && wasSelectedGameObjects.Count == _lettersGameObjectSelected.Count)
            if (_lettersGameObjectSelected.Count < Word.WordSpelling.Length)
                {
                //concatinate characters
                foreach (var item in _lettersGameObjectSelected)
                {   
                    _currentWord += item.Letter.Symbol;
                }

                _audioManager.PlayVoice(_currentWord);
            }
        }

        _currentWord = "";
        _lettersGameObjectSelected.Clear();
    }

    public void IsSelectedUnique(GameObject gameObject, Vector3 inputPosition)
    {
        //play audio
        if (!_audioSource.isPlaying)
        {
            _audioSource.Play();
        }
    }

    public virtual void OnChildLetterSelected(ILetterSelectionResponse childObjectSelected, Vector3 inputPosition)
    {
        if (childObjectSelected != null)
        {
            if (!_lettersGameObjectSelected.Contains(childObjectSelected))
                _lettersGameObjectSelected.Add(childObjectSelected);
        }
    }

    public virtual void OnChildLetterConfirmed(ILetterSelectionResponse childObjectConfirmed, Vector3 inputPosition, List<GameObject> wasSelectedObjects)
    {
        if (childObjectConfirmed != null)
        {
            if (!_lettersGameObjectSelected.Contains(childObjectConfirmed))
                _lettersGameObjectSelected.Add(childObjectConfirmed);
        }

        OnSelectionConfirm(this.gameObject, inputPosition, wasSelectedObjects);
    }

    public void ToUpper()
    {
        for (int i = 0; i < _letterChildren.Length; i++)
        {
            _letterChildren[i].ToUpper();
        }
    }

    public void ToLower()
    {
        for (int i = 0; i < _letterChildren.Length; i++)
        {
            _letterChildren[i].ToLower();
        }
    }

    public void ToStandard()
    {
        for (int i = 0; i < _letterChildren.Length; i++)
        {
            if(i == 0)
            {
                _letterChildren[i].ToUpper();
            }
            else
            {
                _letterChildren[i].ToLower();
            }
        }
    }

    protected float CalculateScale(string word)
    {
        var letterBlockSpriteRenderer = LetterBlockPrefab.GetComponent<RectTransform>();

        int length = word.Length;
        float LetterWidth = (letterBlockSpriteRenderer.rect.width / Screen.width) * WorldUnitSize;
        //float LetterWidth = letterBlockSpriteRenderer.rect.width; // for starting position
        float totalWordSizeX = (length * LetterWidth)+ (PerLetterMargin * word.Length - 1);
        
        float totalWidthInUnits = (WorldUnitSize * (Screen.width / Screen.height)) - Allowance;

        if (totalWidthInUnits < totalWordSizeX)
            return totalWidthInUnits / totalWordSizeX;

        return 1;
    }

    public void PlayWordAudio()
    {
            _audioSource.Play();
    }
}
