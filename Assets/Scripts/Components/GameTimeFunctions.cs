using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class GameTimeFunctions : MonoBehaviour
{
    //public Canvas PauseCanvas;

    private void Start()
    {
    }

    public void PauseGame(GameObject canvas)
    {
        //set time.timescale to 0?..
        canvas.gameObject.SetActive(true);
        
    }

    public void ResumeGame(GameObject canvas)
    {
        //resume time scale
        canvas.gameObject.SetActive(false);

    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
