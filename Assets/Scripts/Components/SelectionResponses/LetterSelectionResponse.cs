using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LetterSelectionResponse : MonoBehaviour, ILetterSelectionResponse
{
    public Letter LetterScriptable; // it needs to be public so that unityeditor can assign scriptable objects
    public Letter Letter
    {
        get { return LetterScriptable; }
        set { LetterScriptable = value; }
    }

    public GameOptions GameOptions;
    protected AudioSource _audioSource;

    protected Vector3 _originalPosition;
    protected Vector3 _originalScale;

    protected Vector3 _growScale;
    protected TextMeshProUGUI _textMeshPro;

    protected Color32 _ColorDeselect = Color.white;
    protected Color32 _ColorActive = new Color32(74, 150, 214, 255);
    protected Color32 _ColorWasActive = new Color32(155, 191, 221, 255);

    protected IWordSelectionResponse _parentWord;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _textMeshPro = GetComponent<TextMeshProUGUI>();
        _parentWord = transform.parent.GetComponent<IWordSelectionResponse>();

        if (!_audioSource)
            throw new NullReferenceException("no audio source comppnent attached.");
        if(!_textMeshPro)
            throw new NullReferenceException("no text mesh pro comppnent attached.");
        if (!GameOptions)
            throw new NullReferenceException("no options");

        _originalPosition = transform.position;
        _originalScale = transform.localScale;
    }
    private void Start()
    {
        _growScale = new Vector2((float)(_originalScale.x * 1.3), (float)(_originalScale.x * 1.3));
    }
    public void Initialize(char letter) // create method overload if necessary
    {
        Letter = Resources.Load<Letter>($"Scripts/Letters/{letter}");
        if (Letter == null)
        {
            Letter = ScriptableObject.CreateInstance<Letter>();
            var letterString = letter.ToString().ToLower();
            Letter.Symbol = letter;
            //consonant or vowel
            if (letterString == "a" || letterString == "e" || letterString == "i" || letterString == "o" || letterString == "u")
            {
                Letter.LetterType = LetterType.vowel;
            }
            else
            {
                Letter.LetterType = LetterType.consonant;
            }
        }
            

        //text mesh
        _textMeshPro.text = Letter.Symbol.ToString();

        //check that all resources are there
            Letter.PhonicAudio = Resources.Load<AudioClip>($"Packages/{GameOptions.VoicePackage}/audio/phonics/{Letter.Symbol}");
            Letter.LetterAudio = Resources.Load<AudioClip>($"Packages/{GameOptions.VoicePackage}/audio/letters/{Letter.Symbol}");

        //assign to component
        if (_audioSource)
            _audioSource.clip = Letter.PhonicAudio;
    }

    public virtual void IsSelected(GameObject gameObject, Vector3 inputPosition)
    {
        
        this.gameObject.transform.localScale = _growScale;
        _textMeshPro.color = _ColorActive;

        if (_parentWord != null)
            _parentWord.OnChildLetterSelected(this, inputPosition);
    }

    public virtual void Deselected(GameObject gameObject, Vector3 inputPosition)
    {
        //return to original scale
        this.gameObject.transform.localScale = _originalScale;
        _textMeshPro.color = _ColorDeselect;

        //Debug.Log("deselected", this);
        //var parentWordComponent = transform.parent.GetComponent<IWordSelectionResponse>();

        //if (_audioSource)
        //    _audioSource.Play();

        //if (_parentWord != null)
        //    _parentWord.OnChildLetterConfirmed(this, inputPosition, null);
    }

    public void OnSelectionConfirm(GameObject gameObject, Vector3 inputPosition)
    {


        this.OnSelectionConfirm(gameObject, inputPosition, new List<GameObject>());
    }
    public void OnSelectionConfirm(GameObject gameObject, Vector3 inputPosition, List<GameObject> wasSelectedGameObjects)
    {
        //return to original scale
        this.gameObject.transform.localScale = _originalScale;
        _textMeshPro.color = _ColorDeselect;

        //var parentWordComponent = transform.parent.GetComponent<IWordSelectionResponse>();
        if (_parentWord != null)
            _parentWord.OnChildLetterConfirmed(this, inputPosition, wasSelectedGameObjects);
    }

    public void OnHoverSelected(GameObject gameObject, Vector3 inputPosition)
    {
        //do nothing
    }

    public void WasSelected(GameObject gameObject, Vector3 inputPosition)
    {
        //do nothing
        this.gameObject.transform.localScale = _originalScale;
        _textMeshPro.color = _ColorWasActive;
    }

    public void IsSelectedUnique(GameObject gameObject, Vector3 inputPosition)
    {
        //maintain scale;
        this.gameObject.transform.localScale = _originalScale;
        _textMeshPro.color = _ColorDeselect;

        PlayAudio();
    }

    public void ToUpper()
    {
        _textMeshPro.text = Letter.Symbol.ToString().ToUpper();
        //_textMeshPro.text.ToUpper();
    }

    public void ToLower()
    {
        _textMeshPro.text = Letter.Symbol.ToString().ToLower();
        //_textMeshPro.text.ToLower();
    }

    protected virtual void PlayAudio()
    {
        if (!_audioSource.isPlaying)
        {
            if (GameOptions.LetterAudio == LetterAudioOptions.phonics)
            {
                _audioSource.Play();
            }
            else
            {
                _audioSource.PlayOneShot(Letter.LetterAudio);
            }
        }
    }
}
