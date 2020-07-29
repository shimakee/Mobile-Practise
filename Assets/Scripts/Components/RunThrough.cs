using JetBrains.Annotations;
using System;
using UnityEngine;

public class RunThrough : MonoBehaviour, IGameSession
{
    public string TextAssetName = "initialWordList";
    public GameObject WordMangerCanvas;
    public WordManager WordManager { get; private set; }

    //UI Canvas
    public GameObject StartCanvas;
    public GameObject EndCanvas;

    //UI buttons - maybe this should be in the IGameSession
    public GameObject NextButton;
    public GameObject PreviousButton;

    private GameOptions _gameOptions;
    private bool _sessionEnded;
    private bool _sessionStarted;
    private bool hasReachedLastWord;
    private AudioManager _audioManager;
    private void Awake()
    {
        WordManager = WordMangerCanvas.GetComponent<WordManager>() ?? throw new NullReferenceException("no word manager");
        _gameOptions = WordManager.GameOptions;
        _audioManager = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        StartCanvas.SetActive(true);
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
        StartCanvas.SetActive(false);
        EndCanvas.SetActive(false);
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
        EndCanvas.SetActive(true);
    }

    public void SessionPause(GameObject pauseCanvas)
    {
        pauseCanvas.gameObject.SetActive(true);
        StartCanvas.SetActive(false);
        EndCanvas.SetActive(false);
    }

    public void SessionReset(GameObject pauseCanvas)
    {
        pauseCanvas.gameObject.SetActive(false);
        StartCanvas.SetActive(true);
        EndCanvas.SetActive(false);
        _sessionEnded = false;
        _sessionStarted = false;
        if(WordManager.WordObjects != null)
        {
            WordManager.WordObjects[WordManager.CurrentIndex].SetActive(false);
            WordManager.ClearWordList();
        }
    }

    public void SessionResume(GameObject pauseCanvas)
    {
        pauseCanvas.gameObject.SetActive(false);


        if (!_sessionStarted && !_sessionEnded)
        {
            StartCanvas.SetActive(true);
        }else if(_sessionEnded)
        {
            EndCanvas.SetActive(true);
        }
    }

    public void Next()
    {
        if (_sessionEnded || !_sessionStarted)
            return;

        _audioManager.Play("Click");

        if(WordManager.CurrentIndex < WordManager.MaxIndex)
        {
            WordManager.DisableCurrentWord();
            WordManager.NextIndex();
            WordManager.InstantiateWord(new Vector3(0, 0, 0));
        }

        Debug.Log( $"current index {WordManager.CurrentIndex}");
        Debug.Log($"max index {WordManager.MaxIndex}");

        if (hasReachedLastWord)
        {
            SessionEnd();
            NextButton.SetActive(false);
        }
        if (WordManager.CurrentIndex >= WordManager.MaxIndex)
            hasReachedLastWord = true;
        if (WordManager.CurrentIndex > 0)
            PreviousButton.SetActive(true);
    }
    public void Previous()
    {
        if (_sessionEnded || !_sessionStarted)
            return;

        _audioManager.Play("Click");

        WordManager.DisableCurrentWord();
        WordManager.PreviousIndex();
        WordManager.InstantiateWord(new Vector3(0, 0, 0));

        if (WordManager.CurrentIndex <= 0)
            PreviousButton.SetActive(false);
        if (WordManager.CurrentIndex < WordManager.MaxIndex)
            NextButton.SetActive(true);
    }
}