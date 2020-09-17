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

    public void InitializePicure(string picture) //create overload for offset and margins?
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

        //check that all resources are there
            _spriteRenderer.sprite = Picture.Sprite;
            _audioSource.clip = Picture.Sfx;

    }
    public void Deselected(GameObject gameObject, Vector3 inputPosition)
    {
        this.gameObject.transform.localScale = new Vector3(1, 1, 0);
    }

    public void IsSelected(GameObject gameObject, Vector3 inputPosition)
    {
        this.gameObject.transform.localScale = new Vector3(1.3f, 1.3f, 0);
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
        this.gameObject.transform.localScale = new Vector3(1, 1, 0);
    }

    public void OnSelectionConfirm(GameObject gameObject, Vector3 inputPosition)
    {
        this.gameObject.transform.localScale = new Vector3(1, 1, 0);
    }

    public void WasSelected(GameObject gameObject, Vector3 inputPosition)
    {
        //do nothing
    }
}
