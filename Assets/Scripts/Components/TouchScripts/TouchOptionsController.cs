using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TouchOptionsController : MonoBehaviour, ITouchControlOptions
{
    public TouchControlOption Option;

    public bool enablePassiveSelection { get { return Option.enablePassiveSelection; } }
    public bool enableUnniqueSelection { get { return Option.enableUnniqueSelection; } }
    public bool enableLastTouchConfirm { get { return Option.enableLastTouchConfirm; } }
    public bool enableDiselectOnlyOnTouchOff { get { return Option.enableDiselectOnlyOnTouchOff; } }
}
