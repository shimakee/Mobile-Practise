using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoResponseLetterSelectionResponse : LetterSelectionResponse, ILetterSelectionResponse
{
    protected override void PlayAudio()
    {
        //do nothing
    }

    public override void Deselected(GameObject gameObject, Vector3 inputPosition)
    {
        //return to original scale
        this.gameObject.transform.localScale = _originalScale;
        _textMeshPro.color = _ColorDeselect;
    }
}
