using System;
using System.Collections.Generic;
using UnityEngine;

public class PictureSelectionResponse : MonoBehaviour, IPictureSelectionResponse
{
    public Picture PictureScriptable;
    public Picture Picture
    {
        get { return PictureScriptable; }
        set { PictureScriptable = value; }
    }

    public GameOptions GameOptions;

    private AudioSource _audioSource;
    private SpriteRenderer _spriteRenderer;

    private Vector3 _originalScale;
    private Vector3 _originalPosition;
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (!_audioSource)
            throw new NullReferenceException("no audio source comppnent attached.");
        if (!_spriteRenderer)
            throw new NullReferenceException("no sprite renderer comppnent attached.");
        if (!GameOptions)
            throw new NullReferenceException("no options.");
    }

    private void Start()
    {
        _originalScale = this.gameObject.transform.localScale;
        _originalPosition = this.gameObject.transform.position;
    }
    public void InitializePicture(string picture) //create overload for offset and margins?
    {
        if(Picture == null)
            Picture = ScriptableObject.CreateInstance<Picture>();
        if(String.IsNullOrWhiteSpace(Picture.Name))
            Picture.Name = picture + "Image";

        //assign assets
        if (Picture.Sfx == null)
            Picture.Sfx = Resources.Load<AudioClip>($"Audio/Sfxs/sfx_{picture}");
        if (Picture.Sprite == null)
            Picture.Sprite = Resources.Load<Sprite>($"Sprites/default/{picture}");
        if (Picture.WordAudio == null)
            Picture.WordAudio = Resources.Load<AudioClip>($"Packages/{GameOptions.VoicePackage}/audio/words/{picture}");

        //name your object
        transform.name = this.Picture.Name;


        //check that all resources are there
        _spriteRenderer.sprite = Picture.Sprite;
        _audioSource.clip = Picture.Sfx;


    }
    public void Deselected(GameObject gameObject, Vector3 inputPosition)
    {
        this.gameObject.transform.localScale = _originalScale;
    }

    public void IsSelected(GameObject gameObject, Vector3 inputPosition)
    {
        var sizeUp = new Vector3(0,0,0);
        sizeUp.x = (float)(_originalScale.x * 1.3);
        sizeUp.y = (float)(_originalScale.x * 1.3);
        this.gameObject.transform.localScale = sizeUp;
    }

    public void IsSelectedUnique(GameObject gameObject, Vector3 inputPosition)
    {
        if (!_audioSource.isPlaying)
        {
            if (GameOptions.ImageAudio == ImageAudioOptions.sfxs)
            {
                _audioSource.Play();
            }
            else
            {
                //play the word
                _audioSource.PlayOneShot(Picture.WordAudio);
            }
        }
    }

    public void OnHoverSelected(GameObject gameObject, Vector3 inputPosition)
    {
        //do nothing
    }

    public void OnSelectionConfirm(GameObject gameObject, Vector3 inputPosition, List<GameObject> wasSelectedGameObjects)
    {
        //this.gameObject.transform.localScale = new Vector3(1, 1, 0);
        this.gameObject.transform.localScale = _originalScale;

    }

    public void OnSelectionConfirm(GameObject gameObject, Vector3 inputPosition)
    {
        //this.gameObject.transform.localScale = new Vector3(1, 1, 0);
        this.gameObject.transform.localScale = _originalScale;

    }

    public void WasSelected(GameObject gameObject, Vector3 inputPosition)
    {
        //do nothing
    }
}
