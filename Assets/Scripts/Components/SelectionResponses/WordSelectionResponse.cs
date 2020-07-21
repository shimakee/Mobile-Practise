using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.iOS;
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
    public GameObject PictureBlockPrefab;

    //audio options
    public GameOptions GameOptions;

    //allowance for spacing
    public float PerLetterMargin = 0;

    private string _currentWord = "";
    private AudioSource _audioSource;
    private SpriteRenderer _spriteRenderer;
    private List<ILetterSelectionResponse> _lettersGameObjectSelected = new List<ILetterSelectionResponse>();

    //directories
    private string _selectedVoice;
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (!_audioSource)
            throw new NullReferenceException("no audio source comppnent attached.");
        if (!_spriteRenderer)
            throw new NullReferenceException("no sprite renderer comppnent attached.");
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
    public int InitializeWord(string wordString, float scaler = 1) //create method overload if necessary
    {
        if (String.IsNullOrWhiteSpace(wordString))
            throw new ArgumentNullException("wordString arguement cannot be null, as it is used to generate the word.");

        //create word class scriptable object
        Word wordScriptable = ScriptableObject.CreateInstance<Word>();
        Word = wordScriptable;

        //load reasource
        Word.WordSpelling = wordString;
        Word.WordAudio = Resources.Load<AudioClip>($"Packages/{GameOptions.VoicePackage}/audio/words/{wordString}");
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
        if (Word.Sprite == null || Word.Sfx == null)
            return 0;

        return 1;
    }

    private void InitializeWordImage(Word word) //create overload for offset and margins?
    {
        //if (word.Sprite != null)
        //{
        //    _spriteRenderer.sprite = word.Sprite;
        //}

        //check if sprite exist
        if (!word.Sprite)
        {
            this._spriteRenderer.enabled = false;
            return;
        }

        //instantiate object
        GameObject pictureGameObject = Instantiate(PictureBlockPrefab, transform);

        pictureGameObject.transform.position = new Vector2(transform.position.x, transform.position.y);
        pictureGameObject.name = word.WordSpelling+"Picture";

        //get component
        var pictureObjectComponent = pictureGameObject.GetComponent<IPictureSelectionResponse>();
        if(pictureObjectComponent != null && word != null)
        {
            Picture picture = ScriptableObject.CreateInstance<Picture>();
            //assign assets
            picture.Sfx = word.Sfx;
            picture.Sprite = word.Sprite;
            picture.Name = word.WordSpelling;
            picture.WordAudio = word.WordAudio;

            //connect scriptable to object
            pictureObjectComponent.InitializePicure(picture);
        }
    }

    private void InitializeLetters(Word word) // create overload for offset and margins?
    {
        int lettersInstantiated = 0;
        var letterBlockSpriteRenderer = LetterBlockPrefab.GetComponent<SpriteRenderer>();
        float letterWidth = letterBlockSpriteRenderer.bounds.size.x; // for starting position
        //float heightAllowance = letterBlockSpriteRenderer.bounds.size.y;
        //float totalWidth = (widthAllowance * word.WordSpelling.Length) + (PerLetterMargin * word.WordSpelling.Length-1);
        //float initialXPosition = transform.position.x - (totalWidth/2);
        //assemble the word using the letters
        foreach (char character in word.WordSpelling)
        {
            Letter letter = word.Letters.Where(l => l.Symbol == character).FirstOrDefault();
            if (letter)
            {

                //Vector3 objectPosition = new Vector3(transform.position.x + (widthAllowance * lettersInstantiated + PerLetterMargin * lettersInstantiated) + LetterXMargin, transform.position.y + LetterYMargin, transform.position.z);
                Vector3 objectPosition = new Vector3(transform.position.x + (letterWidth * lettersInstantiated), transform.position.y, transform.position.z);

                GameObject letterGameObject = Instantiate(LetterBlockPrefab, transform);
                letterGameObject.transform.position = objectPosition;
                letterGameObject.GetComponent<ILetterSelectionResponse>().Letter = letter;
            }
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

    //private Vector3 CalculatePosition(string word, float scaler, float allowance)
    //{
    //    //float widthCenterPosition = (Screen.width / 2)/Screen.width * (WorldUnitSize * WidthAspectRatio);
    //    //Debug.Log($"center position X: {widthCenterPosition}");

    //    //float heightCenterPosition = (Screen.height / 2)/Screen.height * (WorldUnitSize * HeightAspectRatio);
    //    //Debug.Log($"center position Y: {heightCenterPosition}");

    //    var letterBlockSpriteRenderer = LetterBlockPrefab.GetComponent<SpriteRenderer>();

    //    int length = word.Length;
    //    float LetterWidth = letterBlockSpriteRenderer.bounds.size.x; // for starting position
    //    float totalWordSizeX = length * LetterWidth;

    //    float PositionX = ((totalWordSizeX / 2 * -1) + LetterWidth / 2) * scaler;

    //    return new Vector2(PositionX, 0);
    //}
}
