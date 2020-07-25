using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameSessionManager : MonoBehaviour
{
    public GameObject WordMangerCanvas;
    public GameOptions GameOptions;

    public string TextAssetName = "wordList"; 

    //UI buttons
    public GameObject NextButton;
    public GameObject PreviousButton;
    //Options UI buttons
    public GameObject ShuffleToggle;
    public GameObject RepeatToggle;
    public GameObject ImageAudioToggle;
    public GameObject LetterAudioToggle;

    //wordmanager
    private WordManager _wordManager;

    private void Awake()
    {
        _wordManager = WordMangerCanvas.GetComponent<WordManager>() ?? throw new NullReferenceException("no word manager");
        _wordManager.LoadWordsToLoadManager(_wordManager.GenerateWordListFromTxt(TextAssetName));
        _wordManager.InstantiateWord(new Vector3(0, 0, 0));
    }

    // Start is called before the first frame update
    void Start()
    {
        //options - set UI to reflect options
        GameOptions.SetColor(ShuffleToggle, GameOptions.Shuffle);
        GameOptions.SetColor(RepeatToggle, GameOptions.Repeat);
        GameOptions.SetColor(ImageAudioToggle, GameOptions.ImageAudio == ImageAudioOptions.sfxs);
        GameOptions.SetColor(LetterAudioToggle, GameOptions.LetterAudio == LetterAudioOptions.letters);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Next()
    {
        _wordManager.DisableCurrentWord();
        _wordManager.NextIndex();
        _wordManager.InstantiateWord(new Vector3(0,0,0));

        if (_wordManager.CurrentIndex >= _wordManager.MaxIndex && !GameOptions.Repeat)
            NextButton.SetActive(false);
        if (_wordManager.CurrentIndex > 0)
            PreviousButton.SetActive(true);
    }

    public void Previous()
    {
        _wordManager.DisableCurrentWord();
        _wordManager.PreviousIndex();
        _wordManager.InstantiateWord(new Vector3(0,0,0));

        if (_wordManager.CurrentIndex <= 0)
            PreviousButton.SetActive(false);
        if (_wordManager.CurrentIndex < _wordManager.MaxIndex)
            NextButton.SetActive(true);
    }

  
}
