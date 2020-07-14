using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class GameTimeFunctions : MonoBehaviour
{
    public WordManager WordManager;
    public GameObject ShuffleButton;
    public GameObject RepeatButton;
    public Canvas PauseCanvas;

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

    public void ToggleShuffle()
    {
        WordManager.Shuffle = !WordManager.Shuffle;

        //change sprite based on shuffle toggled
        if (WordManager.Shuffle)
        {
            //active sprite
        }
        else
        {
            //inactive sprite
        }

    }

    public void ToggleRepeat()
    {
        WordManager.Repeat = !WordManager.Repeat;
        //change sprite based on repeat toggled
        if (WordManager.Repeat)
        {
            //active sprite
        }
        else
        {
            //inactive sprite
        }
    }

}
