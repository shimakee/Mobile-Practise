using System;
using System.Collections.Generic;
using UnityEngine;

public interface IWordSelectionResponse : ISelectionResponse
{
    Word Word { get; set; }
    int Initialize(string wordString);
    void OnChildLetterConfirmed(ILetterSelectionResponse childObjectConfirmed, Vector3 inputPosition, List<GameObject> wasSelectedObjects);
    void OnChildLetterSelected(ILetterSelectionResponse childObjectSelected, Vector3 inputPosition);

    void ToUpper();
    void ToLower();
    void ToStandard();
}