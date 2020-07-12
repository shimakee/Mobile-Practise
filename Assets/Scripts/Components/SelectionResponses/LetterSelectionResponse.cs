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
    }
    private void Start()
    {
        InitializeLetter(Letter);
    }
    public void InitializeLetter(Letter letter) // create method overload if necessary
    {
        //check that all resources are there
        if (letter.PhonicAudio == null)
            letter.PhonicAudio = Resources.Load<AudioClip>($"Audio/Letters/{letter.Symbol}");
        if (letter.Sprite == null)
            letter.Sprite = Resources.Load<Sprite>($"Sprites/{letter.Symbol}");

        //using resource.load change audio to
        //male or female
        //phonics or letter pronounciation
        if (_audioSource)
            _audioSource.clip = letter.PhonicAudio;

        //sprite to display
        if (_spriteRenderer)
            _spriteRenderer.sprite = letter.Sprite;
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
            _audioSource.Play();
        }
    }
}
