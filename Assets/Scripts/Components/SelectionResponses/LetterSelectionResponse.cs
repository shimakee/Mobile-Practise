using System;
using System.Collections;
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
    private AudioSource _audioSource;

    private Vector3 _originalPosition;
    private Vector3 _originalScale;

    private Vector3 _growScale;
    private TextMeshProUGUI _textMeshPro;

    private Color32 _ColorDeselect = Color.white;
    private Color32 _ColorActive = new Color32(74, 150, 214, 255);
    private Color32 _ColorWasActive = new Color32(155, 191, 221, 255);

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _textMeshPro = GetComponent<TextMeshProUGUI>();

        if (!_audioSource)
            throw new NullReferenceException("no audio source comppnent attached.");
        if(!_textMeshPro)
            throw new NullReferenceException("no text mesh pro comppnent attached.");
        if (!GameOptions)
            throw new NullReferenceException("no options");
    }
    private void Start()
    {
        _originalPosition = transform.position;
        _originalScale = transform.localScale;
        _growScale = new Vector2((float)(_originalScale.x * 1.3), (float)(_originalScale.x * 1.3));
    }
    public void Initialize(char letter) // create method overload if necessary
    {
        Letter = Resources.Load<Letter>($"Scripts/Letters/{letter}");
        if (Letter == null)
        {
            Letter = ScriptableObject.CreateInstance<Letter>();
            Letter.Symbol = letter;
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

    public void IsSelected(GameObject gameObject, Vector3 inputPosition)
    {
        
        this.gameObject.transform.localScale = _growScale;
        _textMeshPro.color = _ColorActive;

        var parentWordComponent = transform.parent.GetComponent<IWordSelectionResponse>();
        if (parentWordComponent != null)
            parentWordComponent.OnChildLetterSelected(this, inputPosition);
    }

    public void Deselected(GameObject gameObject, Vector3 inputPosition)
    {
        //return to original scale
        this.gameObject.transform.localScale = _originalScale;
        _textMeshPro.color = _ColorDeselect;
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

        var parentWordComponent = transform.parent.GetComponent<IWordSelectionResponse>();
        if (parentWordComponent != null)
            parentWordComponent.OnChildLetterConfirmed(this, inputPosition, wasSelectedGameObjects);
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
