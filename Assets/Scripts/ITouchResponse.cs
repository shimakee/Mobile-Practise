using UnityEngine;

public interface ITouchResponse
{
    GameObject DetermineSelection(Vector3 selectionPosition);
    void IsSelected(GameObject gameObject, Vector3 inputPosition);
    void WasSelected(GameObject gameObject, Vector3 inputPosition);
    void OnHoverSelected(GameObject gameObject, Vector3 inputPosition);
    void Deselected(GameObject gameObject, Vector3 inputPosition);
    void SelectionConfirmed(GameObject gameObject, Vector3 inputPositoin);
    void IsSelectedUnique(GameObject gameObject, Vector3 inputPosition);
}
