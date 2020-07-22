using System;
using System.Collections;
using System.Collections.Generic;
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
    private SpriteRenderer _spriteRenderer;

    private GameObject _selectedObjectBegin;
    private GameObject _selectedObjectEnd;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (!_audioSource)
            throw new NullReferenceException("no audio source comppnent attached.");
        if (!_spriteRenderer)
            throw new NullReferenceException("no sprite renderer comppnent attached.");
        if (!GameOptions)
            throw new NullReferenceException("no ooptions");
    }
    private void Start()
    {
    }
    public void Initialize(char letter) // create method overload if necessary
    {
        //if (Letter == null)
            Letter = Resources.Load<Letter>($"Scripts/Letters/{letter}");

        //check that all resources are there
        //if (Letter.PhonicAudio == null)
            Letter.PhonicAudio = Resources.Load<AudioClip>($"Packages/{GameOptions.VoicePackage}/audio/phonics/{Letter.Symbol}");
        //if (Letter.LetterAudio == null)
            Letter.LetterAudio = Resources.Load<AudioClip>($"Packages/{GameOptions.VoicePackage}/audio/letters/{Letter.Symbol}");
        if (Letter.Sprite == null)
            Letter.Sprite = Resources.Load<Sprite>($"Sprites/{Letter.Symbol}");

        //assign to component
        if (_audioSource)
            _audioSource.clip = Letter.PhonicAudio;
        if (_spriteRenderer)
            _spriteRenderer.sprite = Letter.Sprite;
    }

    public void IsSelected(GameObject gameObject, Vector3 inputPosition)
    {
        this.gameObject.transform.localScale = new Vector3(1.3f, 1.3f, 0);
        var parentWordComponent = transform.parent.GetComponent<IWordSelectionResponse>();
        if (parentWordComponent != null)
            parentWordComponent.OnChildLetterSelected(this, inputPosition);
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
        this.gameObject.transform.localScale = new Vector3(1.1f, 1.1f, 0);
    }

    public void IsSelectedUnique(GameObject gameObject, Vector3 inputPosition)
    {
        //maintain scale;
        this.gameObject.transform.localScale = new Vector3(1, 1, 0);

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
