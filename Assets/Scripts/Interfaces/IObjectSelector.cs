using UnityEngine;

public interface IObjectSelector
{
    GameObject DetermineSelection(Vector3 inputPosition);
}