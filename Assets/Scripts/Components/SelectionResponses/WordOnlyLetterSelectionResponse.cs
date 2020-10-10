using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WordOnlyLetterSelectionResponse : WordSelectionResponse, IWordSelectionResponse
{
    public override void OnChildLetterConfirmed(ILetterSelectionResponse childObjectConfirmed, Vector3 inputPosition, List<GameObject> wasSelectedObjects)
    {
        if (!_audioSource.isPlaying)
        {
            Debug.Log("playing", this);
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

}
