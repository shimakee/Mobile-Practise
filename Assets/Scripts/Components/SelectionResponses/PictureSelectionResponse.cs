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
    }

    public void InitializePicure(Picture picture) //create overload for offset and margins?
    {
        if(picture != null)
        {
            Picture = picture;
            //check that all resources are there
            if (Picture.Sfx == null)
                Picture.Sfx = Resources.Load<AudioClip>($"Audio/Sfxs/sfx_{Picture.Name}");
            if (Picture.Sprite == null)
                Picture.Sprite = Resources.Load<Sprite>($"Sprites/{Picture}");

            if (Picture.Sprite != null)
                _spriteRenderer.sprite = picture.Sprite;

            if (Picture.Sfx != null)
                _audioSource.clip = picture.Sfx;
        }

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
            _audioSource.Play();
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
