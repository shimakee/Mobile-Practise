using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName ="TouchControlOption", menuName ="Create scriptable TouchControlOption")]
public class TouchControlOption : ScriptableObject
{
    public bool enablePassiveSelection = false;
    public bool enableUnniqueSelection = false;
    public bool enableLastTouchConfirm = false;
    public bool enableDiselectOnlyOnTouchOff = false;
}
