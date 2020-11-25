using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NoResponseLetterSelectionResponse : LetterSelectionResponse, ILetterSelectionResponse
{

    private WordOnlyLetterSelectionResponse _parent;
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _textMeshPro = GetComponent<TextMeshProUGUI>();
        //_parentWord = transform.parent.GetComponent<WordOnlyLetterSelectionResponse>();
        _parent = transform.parent.GetComponent<WordOnlyLetterSelectionResponse>();
        //_parentWord.chi += SelectedAction;
        //_parentWord.ChildDeselected += DeselectedAction;
        _parent.ChildIsSelected += SelectedAction;
        _parent.ChildDeselected += DeselectedAction;

        if (!_audioSource)
            throw new NullReferenceException("no audio source comppnent attached.");
        if (!_textMeshPro)
            throw new NullReferenceException("no text mesh pro comppnent attached.");
        if (!GameOptions)
            throw new NullReferenceException("no options");

        _originalPosition = transform.position;
        _originalScale = transform.localScale;
    }
    protected override void PlayAudio()
    {
        //do nothing
    }

    public override void Deselected(GameObject gameObject, Vector3 inputPosition)
    {
        Debug.Log("is deselected chold to parent call 1 ");

        //return to original scale
        DeselectedAction();

        if (_parent != null)
        {
            _parent.OnChildDeselected();

        }
    }

    public override void IsSelected(GameObject gameObject, Vector3 inputPosition)
    {
        SelectedAction();

        if (_parent != null)
        {
            Debug.Log("is selected chold to parent call");
            _parent.OnChildLetterSelected(this, inputPosition);
        }
    }

    public override void OnSelectionConfirm(GameObject gameObject, Vector3 inputPosition, List<GameObject> wasSelectedGameObjects)
    {
        DeselectedAction();
        if (_parent != null)
            _parent.OnChildLetterConfirmed(this, inputPosition, wasSelectedGameObjects);
        if(_parent)
            _parent.OnChildDeselected();
    }

    private void OnDestroy()
    {
        _parent.ChildIsSelected -= SelectedAction;
        _parent.ChildDeselected -= DeselectedAction;
    }
}
