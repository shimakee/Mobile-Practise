using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.iOS;
using UnityEngine;
using UnityEngine.EventSystems;

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
    private AudioSource _audioSource;
    //private MeshRenderer _renderer;
    //private SpriteRenderer _spriteRenderer;
    private List<ILetterSelectionResponse> _lettersGameObjectSelected = new List<ILetterSelectionResponse>();

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        //_renderer = GetComponent<MeshRenderer>();
        //_spriteRenderer = GetComponent<SpriteRenderer>();

        if (!_audioSource)
            throw new NullReferenceException("no audio source comppnent attached.");
        //if (!_spriteRenderer)
        //    throw new NullReferenceException("no sprite renderer comppnent attached.");
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

        //create word class scriptable object
        if(Word == null)
            Word = ScriptableObject.CreateInstance<Word>();

        //load reasource
        if(String.IsNullOrWhiteSpace(Word.WordSpelling))
            Word.WordSpelling = wordString;
        if(Word.WordAudio == null)
            Word.WordAudio = Resources.Load<AudioClip>($"Packages/{GameOptions.VoicePackage}/audio/words/{wordString}");
        if(Word.Sprite == null)
            Word.Sprite = Resources.Load<Sprite>($"Sprites/{wordString}");
        if(Word.Sfx == null)
            Word.Sfx = Resources.Load<AudioClip>($"Audio/Sfxs/sfx_{wordString}");

        //assign components
        _audioSource.clip = Word.WordAudio;

        //initializing letters & place them in the world.
        AssembleLetters(Word.WordSpelling);
        //InitializeWordImage(Word);

        //return 0 if only necessary components are loaded
        //return 1 if all components are loaded
        if (Word.Sprite == null || Word.Sfx == null)
            return 0;

        return 1;
    }
    private void AssembleLetters(string word) // create overload for offset and margins?
    {
        var letterBlockSpriteRenderer = LetterBlockPrefab.GetComponent<RectTransform>();
        float scale = CalculateScale(word);
        Debug.Log($"scale {scale}");

        float letterWidth = letterBlockSpriteRenderer.rect.width * scale; // for starting position
        //float letterWidth = letterBlockSpriteRenderer.bounds.size.x; // for starting position
        Debug.Log($"Letter width scaled {letterWidth}");
        float initialAllowanceToCenterPosition = ((letterWidth * word.Length) / 2) - (letterWidth / 2); //less half since pivot point is at the center.
        Debug.Log($"Initial allowance {initialAllowanceToCenterPosition}");

        //assemble the word using the letters
        int lettersInstantiated = 0;
        foreach (char character in word)
        {
            //Letter letter = word.Letters.Where(l => l.Symbol == character).FirstOrDefault();
            //if (letter)
            //{
            Vector3 objectPosition = new Vector3(transform.position.x + (letterWidth * lettersInstantiated) - initialAllowanceToCenterPosition, transform.position.y, transform.position.z);
                GameObject letterGameObject = Instantiate(LetterBlockPrefab, transform);
                letterGameObject.transform.position = objectPosition;
                //letterGameObject.transform.localScale = new Vector2(scale, scale);
            letterGameObject.GetComponent<ILetterSelectionResponse>().Initialize(character);
            //}
            lettersInstantiated++;
        }
    }

    public void IsSelected(GameObject gameObject, Vector3 inputPosition)
    {
        this.gameObject.transform.localScale = new Vector3(1.3f, 1.3f, 0);
    }

    public void OnHoverSelected(GameObject gameObject, Vector3 inputPosition)
    {
        //do nothing
    }

    public void WasSelected(GameObject gameObject, Vector3 inputPosition)
    {
        this.gameObject.transform.localScale = new Vector3(1.1f, 1.1f, 0);
    }

    public void Deselected(GameObject gameObject, Vector3 inputPosition)
    {
        //return to original scale
        this.gameObject.transform.localScale = new Vector3(1, 1, 0);
    }

    public void OnSelectionConfirm(GameObject gameObject, Vector3 inputPosition)
    {
        //return to original scale
        this.gameObject.transform.localScale = new Vector3(1, 1, 0);

        this.OnSelectionConfirm(gameObject, inputPosition, new List<GameObject>());
    }

    public void OnSelectionConfirm(GameObject gameObject, Vector3 inputPosition, List<GameObject> wasSelectedGameObjects)
    {
        //return to original scale
        this.gameObject.transform.localScale = new Vector3(1, 1, 0);

        //check if selected matches.
        if (_lettersGameObjectSelected.Count == Word.WordSpelling.Length && wasSelectedGameObjects.Count == Word.WordSpelling.Length)
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
            if (_lettersGameObjectSelected.Count < Word.WordSpelling.Length && wasSelectedGameObjects.Count == _lettersGameObjectSelected.Count)
            {
                foreach (var item in _lettersGameObjectSelected)
                {
                    _currentWord += item.Letter.Symbol;
                }

                //find if we have audio that matches current selected
                AudioClip resourcedWord = Resources.Load<AudioClip>($"Packages/{GameOptions.VoicePackage}/audio/words/{_currentWord}");
                if (resourcedWord && !_audioSource.isPlaying)
                {
                    _audioSource.PlayOneShot(resourcedWord);
                }
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

    public void OnChildLetterSelected(ILetterSelectionResponse childObjectSelected, Vector3 inputPosition)
    {
        if (childObjectSelected != null)
        {
            if (!_lettersGameObjectSelected.Contains(childObjectSelected))
                _lettersGameObjectSelected.Add(childObjectSelected);
        }
    }

    public void OnChildLetterConfirmed(ILetterSelectionResponse childObjectConfirmed, Vector3 inputPosition, List<GameObject> wasSelectedObjects)
    {
        if (childObjectConfirmed != null)
        {
            if (!_lettersGameObjectSelected.Contains(childObjectConfirmed))
                _lettersGameObjectSelected.Add(childObjectConfirmed);
        }

        OnSelectionConfirm(this.gameObject, inputPosition, wasSelectedObjects);
    }

    float CalculateScale(string word)
    {
        var letterBlockSpriteRenderer = LetterBlockPrefab.GetComponent<RectTransform>();

        int length = word.Length;
        float LetterWidth = letterBlockSpriteRenderer.rect.width; // for starting position
        float totalWordSizeX = (length * LetterWidth);

        float totalWidthInUnits = (WorldUnitSize * (Screen.width / Screen.height)) - Allowance;
        if (totalWidthInUnits < totalWordSizeX)
            return totalWidthInUnits / totalWordSizeX;

        //Debug.Log($"word size {totalWordSizeX}");
        //Debug.Log($"width in units {totalWidthInUnits}");
        //Debug.Log($"divisor {divisor}");

        return 1;
    }

}
