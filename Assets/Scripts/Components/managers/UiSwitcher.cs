using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UiSwitcher : MonoBehaviour
{
    public UiType desiredUi;

    UiManager UiManager;
    Button Button;

    private void Start()
    {
        Button = GetComponent<Button>();
        Button.onClick.AddListener(OnButtonClicked);
        UiManager = FindObjectOfType<UiManager>();
    }

    void OnButtonClicked()
    {
        UiManager.SwitchCanvas(desiredUi);
    }
}
