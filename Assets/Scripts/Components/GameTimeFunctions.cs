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

    //buttons
    public GameObject NextButton;
    public GameObject PreviousButton;

    private GameOptions _gameOptions;

    private void Start()
    {
        _gameOptions = WordManager.gameOptions;

        SetColor(ShuffleToggle, _gameOptions.Shuffle);
        SetColor(RepeatToggle, _gameOptions.Repeat);
        SetColor(ImageAudioToggle, _gameOptions.ImageAudio == ImageAudioOptions.sfxs);
        SetColor(LetterAudioToggle, _gameOptions.LetterAudio == LetterAudioOptions.letters);
    }

    private void FixedUpdate()
    {
        if (WordManager.CurrentWordListIndex <= 0)
        {
            PreviousButton.SetActive(false);
        }
        else
        {
            PreviousButton.SetActive(true);
        }

        if(WordManager.CurrentWordListIndex >= WordManager.WordListLength-1 && !WordManager.gameOptions.Repeat)
        {
            NextButton.SetActive(false);
        }
        else
        {
            NextButton.SetActive(true);
        }
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
        
        SetColor(ShuffleToggle, _gameOptions.Shuffle);
        _gameOptions.Save();

    }

    public void ToggleRepeat()
    {
        _gameOptions.Repeat = !_gameOptions.Repeat;

        SetColor(RepeatToggle, _gameOptions.Repeat);
        _gameOptions.Save();
    }

    public void ToggleImageAudio()
    {
        if (_gameOptions.ImageAudio == ImageAudioOptions.sfxs)
        {
            _gameOptions.ImageAudio = ImageAudioOptions.words;
        }
        else
        {
            _gameOptions.ImageAudio = ImageAudioOptions.sfxs;
        }

        SetColor(ImageAudioToggle, _gameOptions.ImageAudio == ImageAudioOptions.sfxs);
        _gameOptions.Save();
    }

    public void ToggleLetterAudio()
    {
        if (_gameOptions.LetterAudio == LetterAudioOptions.letters)
        {
            _gameOptions.LetterAudio = LetterAudioOptions.phonics;
        }
        else
        {
            _gameOptions.LetterAudio = LetterAudioOptions.letters;
        }

        SetColor(LetterAudioToggle, _gameOptions.LetterAudio == LetterAudioOptions.letters);
        _gameOptions.Save();
    }


    private void SetColor(GameObject gameObject, bool isActive)
    {
        Image spriteRenderer = gameObject.GetComponent<Image>();


        if (isActive)
        {
            spriteRenderer.color = new Color32(45, 146, 231, 255);
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }
}
