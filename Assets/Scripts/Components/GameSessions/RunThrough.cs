using System;
using UnityEngine;

public class RunThrough : MonoBehaviour, IGameSession
{
    public string TextAssetName = "initialWordList";
    public GameObject WordMangerCanvas;
    public WordManager WordManager { get; private set; }
    [Range(1f, 10f)] public float ImageSize = 5;

    //UI Canvas
    //public UiManager UiManager;

    //UI buttons - maybe this should be in the IGameSession
    public GameObject NextButton;
    public GameObject PreviousButton;

    private GameOptions _gameOptions;
    private bool _sessionEnded;
    private bool _sessionStarted;
    private bool hasReachedLastWord;
    private AudioManager _audioManager;
    private bool IsImageActive;

    private UiManager _uiManager;
    private void Awake()
    {
        WordManager = WordMangerCanvas.GetComponent<WordManager>() ?? throw new NullReferenceException("no word manager");
        _gameOptions = WordManager.GameOptions;
        _audioManager = FindObjectOfType<AudioManager>();
        _uiManager = FindObjectOfType<UiManager>();
    }

    private void Start()
    {
        //StartCanvas.SetActive(true);
        //there is a unity bug, if i instantiate it here the word will be complete but if i instantiate it on sessionstart last letter will be missing
        //WordManager.WordObjects[WordManager.CurrentIndex].SetActive(false);
    }

    public void SessionStart()
    {
        SessionStart(TextAssetName);
    }
    public void SessionStart(string textFileName)
    {
        if (String.IsNullOrWhiteSpace(textFileName))
        {
            WordManager.LoadWordsToLoadManager(WordManager.GenerateWordListFromTxt(TextAssetName));
        }
        else
        {
            TextAssetName = textFileName;
            WordManager.LoadWordsToLoadManager(WordManager.GenerateWordListFromTxt(textFileName));
        }



        //WordManager.WordObjects[WordManager.CurrentIndex].SetActive(true);
        WordManager.ResetIndex();
        WordManager.InstantiateWord(new Vector3(0, 0, 0));
        //StartCanvas.SetActive(false);
        //EndCanvas.SetActive(false);
        _sessionStarted = true;
        _sessionEnded = false;
        hasReachedLastWord = false;
        NextButton.SetActive(true);
    }

    public void SessionEnd()
    {
        _audioManager.Play("Success");
        _sessionEnded = true;
        _sessionStarted = false;
        WordManager.WordObjects[WordManager.CurrentIndex].SetActive(false);
        _uiManager.SwitchCanvas(UiType.gameEnd);

        if (WordManager.WordObjects != null)
        {
            WordManager.WordObjects[WordManager.CurrentIndex].SetActive(false);
            WordManager.ClearWordList();
        }
    }

    public void SessionPause(GameObject pauseCanvas)
    {
    }

    public void SessionReset()
    {
        _sessionEnded = false;
        _sessionStarted = false;
        if (WordManager)
        {
            if(WordManager.WordObjects != null)
            {
                if (WordManager.WordObjects[WordManager.CurrentIndex] != null) 
                    WordManager.WordObjects[WordManager.CurrentIndex].SetActive(false);
                WordManager.ClearWordList();
            }
        }
    }

    public void SessionResume(GameObject pauseCanvas)
    {
    }

    public void Next()
    {
        if (_sessionEnded || !_sessionStarted)
            return;

        _audioManager.Play("Click");

        if(WordManager.CurrentIndexRunner < WordManager.MaxIndex)   
        {
            WordManager.DisableCurrentWord();
            WordManager.DisableCurrentImage();
            IsImageActive = false;

            WordManager.NextIndex();
            WordManager.InstantiateWord(new Vector3(0, 0, 0));

        }

        if (hasReachedLastWord)
        {
            SessionEnd();
            NextButton.SetActive(false);
        }
        if (WordManager.CurrentIndexRunner >= WordManager.MaxIndex)
            hasReachedLastWord = true;
        if (WordManager.CurrentIndexRunner > 0)
            PreviousButton.SetActive(true);
    }
    public void Previous()
    {
        if (_sessionEnded || !_sessionStarted)
            return;

        _audioManager.Play("Click");

        WordManager.DisableCurrentImage();
        IsImageActive = false;

        WordManager.DisableCurrentWord();
        WordManager.PreviousIndex();
        WordManager.InstantiateWord(new Vector3(0, 0, 0));

        if (WordManager.CurrentIndexRunner <= 0)
            PreviousButton.SetActive(false);
        if (WordManager.CurrentIndexRunner < WordManager.MaxIndex)
            NextButton.SetActive(true);
    }

    public void FlipImageAndWord()
    {
        if (IsImageActive)
        {
            WordManager.InstantiateWord(new Vector3(0, 0, 0));
            WordManager.DisableCurrentImage();
            IsImageActive = false;
        }
        else
        {
            WordManager.DisableCurrentWord();
            WordManager.InstantiateImage(new Vector3(0,0,0), ImageSize);
            IsImageActive = true;

        }
    }
}