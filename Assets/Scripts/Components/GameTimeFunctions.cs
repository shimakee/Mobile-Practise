using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class GameTimeFunctions : MonoBehaviour
{
    public Canvas PauseCanvas;

    private void Start()
    {
    }

    public void PauseGame()
    {
        //set time.timescale to 0?..
        PauseCanvas.gameObject.SetActive(true);
        
    }

    public void ResumeGame()
    {
        //resume time scale
        PauseCanvas.gameObject.SetActive(false);

    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
