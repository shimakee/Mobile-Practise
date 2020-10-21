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
        //if (!_audioSource.isPlaying)
        //{
        //    _audioSource.Play();
        //}
    }

    float CalculateImageScale()
    {
        //get aspect ratio
        int pixelPerUnit = Screen.height / TotalWorldUnits;

        return pixelPerUnit;
    }
}
