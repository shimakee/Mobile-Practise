using System.Collections.Generic;
using UnityEngine;

public interface ISelectionResponse
{
    GameObject DetermineSelection(Vector3 selectionPosition);
    void IsSelected(GameObject gameObject, Vector3 inputPosition);
    void OnHoverSelected(GameObject gameObject, Vector3 inputPosition);

    void WasSelected(GameObject gameObject, Vector3 inputPosition);
    void Deselected(GameObject gameObject, Vector3 inputPosition);

    void OnSelectionConfirm(GameObject gameObject, Vector3 inputPosition);
    void IsSelectedUnique(GameObject gameObject, Vector3 inputPosition);
}
