using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class GameTimeFunctions : MonoBehaviour
{
    public WordManager WordManager;
    public GameObject ShuffleToggle;
    public GameObject RepeatToggle;
    public GameObject ImageAudioToggle;
    public GameObject LetterAudioToggle;
    public Canvas PauseCanvas;

    private GameOptions _gameOptions;

    private void Start()
    {
        _gameOptions = WordManager.gameOptions;
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

    public void ToggleShuffle()
    {
        _gameOptions.Shuffle = !_gameOptions.Shuffle;

        //change sprite based on shuffle toggled
        if (_gameOptions.Shuffle)
        {
            //active sprite
            ShuffleToggle.GetComponent<Image>().color = new Color32(45, 146, 231, 255);
        }
        else
        {
            //inactive sprite
            ShuffleToggle.GetComponent<Image>().color = Color.white;
        }

        _gameOptions.Save();

    }

    public void ToggleRepeat()
    {
        _gameOptions.Repeat = !_gameOptions.Repeat;
        //change sprite based on repeat toggled
        if (_gameOptions.Repeat)
        {
            //active sprite
            RepeatToggle.GetComponent<Image>().color = new Color32(45, 146, 231, 255);
        }
        else
        {
            //inactive sprite
            RepeatToggle.GetComponent<Image>().color = Color.white;
        }

        _gameOptions.Save();
    }

    public void ToggleImageAudio()
    {
         Image spriteRenderer = ImageAudioToggle.GetComponent<Image>();

        if (_gameOptions.ImageAudio == ImageAudioOptions.sfxs)
        {
            _gameOptions.ImageAudio = ImageAudioOptions.words;
            //change sprite
            spriteRenderer.color = Color.white;
        }
        else
        {
            _gameOptions.ImageAudio = ImageAudioOptions.sfxs;
            //change sprite
            spriteRenderer.color = Color.blue;
        }
        _gameOptions.Save();
    }

    public void ToggleLetterAudio()
    {
        Image spriteRenderer = LetterAudioToggle.GetComponent<Image>();


        if (_gameOptions.LetterAudio == LetterAudioOptions.letters)
        {
            _gameOptions.LetterAudio = LetterAudioOptions.phonics;
            //change sprite
            spriteRenderer.color = Color.white;
        }
        else
        {
            _gameOptions.LetterAudio = LetterAudioOptions.letters;
            //change sprite
            spriteRenderer.color = Color.blue;
        }

        _gameOptions.Save();
    }

}
