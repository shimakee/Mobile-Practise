using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WordOnlyLetterSelectionResponse : WordSelectionResponse, IWordSelectionResponse
{
    private SpriteRenderer _sprite;
    public int TotalWorldUnits = 10;
    public int imageScale = 10;

    public event Action WordSet;

    public event Action ChildIsSelected;
    public event Action ChildDeselected;


    private void Awake()
    {
        _audioManager = FindObjectOfType<AudioManager>();
        _audioSource = GetComponent<AudioSource>();

        GameOptions.PropertyChanged += OptionChanged;

        if (!_audioManager)
            throw new NullReferenceException("no audio manager comppnent attached.");
        if (!_audioSource)
            throw new NullReferenceException("no audio source comppnent attached.");
        if (!GameOptions)
            throw new NullReferenceException("no game options to load.");
    }

    private void Start()
    {
        var child = transform.Find("background");
        _sprite = child.GetComponent<SpriteRenderer>();

        if (_sprite)
        {
            _sprite.sprite = Word.Sprite;
            _sprite.color = Color.black;
            var scale = CalculateImageScale() * imageScale;
            child.transform.localScale = new Vector2(scale, scale);
        }

        OptionChanged();
    }

    public override void OnChildLetterConfirmed(ILetterSelectionResponse childObjectConfirmed, Vector3 inputPosition, List<GameObject> wasSelectedObjects)
    {
        if (!_audioSource.isPlaying)
        {
            _audioSource.Play();
        }
    }

    public override void OnChildLetterSelected(ILetterSelectionResponse childObjectSelected, Vector3 inputPosition)
    {
        ChildIsSelected();

        base.OnChildLetterSelected(childObjectSelected, inputPosition);

    }

    public void OnChildDeselected()
    {
        ChildDeselected();
    }

    float CalculateImageScale()
    {
        //get aspect ratio
        int pixelPerUnit = Screen.height / TotalWorldUnits;

        return pixelPerUnit;
    }
}
