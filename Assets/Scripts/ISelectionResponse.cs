using UnityEngine;

public interface ISelectionResponse
{
    GameObject DetermineSelection(Vector3 selectionPosition);
    void OnDeselect(GameObject gameObject);
    void OnSelected(GameObject gameObject, Vector3 inputPosition);
    void OnSelectionConfirm(GameObject gameObject, Vector3 inputPosition);
}
