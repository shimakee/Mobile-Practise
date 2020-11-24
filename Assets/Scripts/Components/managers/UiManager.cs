using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum UiType
{
    mainMenu,
    gameUI,
    pauseMenu,
    options,
    gameStart,
    gameEnd,
    option
}

public class UiManager : MonoBehaviour
{
    List<UiController> CanvasControllerList;
    UiController lastActiveCanvas;
    private void Awake()
    {
        CanvasControllerList = GetComponentsInChildren<UiController>().ToList();
        CanvasControllerList.ForEach(item => item.gameObject.SetActive(false));

        SwitchCanvas(UiType.mainMenu);
    }

    public void SwitchCanvas(UiType canvasType)
    {
        if (lastActiveCanvas != null)
            lastActiveCanvas.gameObject.SetActive(false);

        UiController desiredCanvas = CanvasControllerList.Find(item => item.CanvasType == canvasType);
        if(desiredCanvas != null)
        {
            lastActiveCanvas = desiredCanvas;
            desiredCanvas.gameObject.SetActive(true);
        }
    }
}
