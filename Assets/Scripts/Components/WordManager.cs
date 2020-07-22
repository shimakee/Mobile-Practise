
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class WordManager : MonoBehaviour
{
    public string TextAssetName = "wordList"; // to delete
    public TextAsset WordsListTextFile;
    public GameObject WordBlockPrefab;
    public GameObject LetterBlockPrefab;
    public GameObject ImageBlockPrefab;
    public float WordSpawnWaitTime = .5f;

    //aspect ratio
    public int WorldUnitSize = 10;
    //public int WidthAspectRatio = 16; //to delete
    //public int HeightAspectRatio = 9; //to delete

    //Options
    public GameOptions gameOptions;
    //[Range(1, 5)] public int MinWordLength;
    //[Range(1, 5)] public int MaxWordLength;



    //wordList
    private List<string> _completeWordList;
    private GameObject[] _readWordsObject; //object of words already read; (so as not to destroy and instantiate them again and again
    private GameObject[] _readImagesObjects;

    //indexes
    private int[] _wordsListIndex; //list of index for all words arranged or shuffled based on option.
    private int _currentWorListIndex = 0; //this is the index that will run up and down the wordsListIndex.
    private int _maxIndex; //maxIndex before looping/repeating
    private int _wordIndex
    {
        get {
                Debug.Log($" wordlistIndex returned {_wordsListIndex[_currentWorListIndex]}");
                return _wordsListIndex[_currentWorListIndex]; }
    }

    //word
    private bool _isCreatingWord = false;
    private bool _isCreatingImage = false;
    private GameObject _currentWordObject;
    private GameObject _currentImageObject;
    private void Awake()
    {
        //check that wordlist text file exist
        //TextAssetName will be  changed based on mode?
        if (!WordsListTextFile)
            WordsListTextFile = Resources.Load<TextAsset>(TextAssetName);

        //generate list of words.
        _completeWordList = GenerateWordListFromTxt(WordsListTextFile);

        //initialize array to store instantiated objects
        _readWordsObject = new GameObject[_completeWordList.Count];
        _readImagesObjects = new GameObject[_completeWordList.Count];

        //assign max index;
        _maxIndex = _completeWordList.Count - 1;
        _readWordsObject = new GameObject[_completeWordList.Count];

        //check that there is a prefab
        if (!WordBlockPrefab)
            throw new NullReferenceException("must have a reference to the wordBlock object to instantiate.");
    }
    private void Start()
    {
        //int wordListLength = _completeWordList.Count - 1;

        //generate array indexes - should be based on options
        _wordsListIndex = GetArrangedIndex(_completeWordList);
        string word = _completeWordList[_wordIndex];

        //generate first word
        StartCoroutine(CreateWordObject(_completeWordList[_wordIndex], WordSpawnWaitTime, CalculatePosition(word)));
        StartCoroutine(CreateImageObject(_completeWordList[_wordIndex], WordSpawnWaitTime, new Vector2(0, 0)));
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0)
        {
            foreach (var touch in Input.touches)
            {
                OnOneFingerTwoTaps(touch); //get next object
                OnTwoFingerOneap(touch); //get previous object
            }
        }
        //Debug.Log($"x:{Input.mousePosition.x}");
        //Debug.Log($"y:{Input.mousePosition.y}");
    }

    //create list of strings from the word block
    private List<string> GenerateWordListFromTxt(TextAsset textAsset)
    {

        //reads text file and generates text array of strings.
        List<string> wordList = textAsset.ToString().Split('\n').ToList();
        List<string> trimmedWordList = new List<string>();

        foreach (var word in wordList)
        {
            trimmedWordList.Add(word.Trim());
        }

        trimmedWordList.Sort();
        return trimmedWordList;
    }

    void NextIndex()
    {
        if(_currentWorListIndex < _maxIndex)
        {
            _currentWorListIndex++;
        }
        else
        {
            //if repeat is on loop back to the start
            if (gameOptions.Repeat)
            {
                _currentWorListIndex = 0;
            }
            else
            {
                //end scene?... show end game animation?... score?..
            }
        }

        Debug.Log($"wordlist length = {_wordsListIndex.Length}");
        Debug.Log($"currentWordListIndex = {_currentWorListIndex}");
    }

    void PreviousIndex()
    {
        if (_currentWorListIndex > 0)
            _currentWorListIndex--;

        Debug.Log($"currentWordListIndex = {_currentWorListIndex}");
    }

    //int[] GetShuffledIndex(List<string> wordList)
    //{
    //    int[] shuffledIndex = new int[wordList.Count - 1];
    //    //get set of random numbers from 0 to wordList.length-1;

    //    //place them onto shuffleIndex


    //    return shuffledIndex;
    //}

    int[] GetArrangedIndex(List<string> wordList)
    {
        int[] arrangedIndex = new int[wordList.Count];
        Debug.Log($"wordlist length {wordList.Count}");
        Debug.Log($"arranged index length {arrangedIndex.Length}");

        for (int i = 0; i < arrangedIndex.Length; i++)
        {
            arrangedIndex[i] = i;
            Debug.Log($"arrange index number i = {i}");
        }

        return arrangedIndex;
    }

    private IEnumerator CreateWordObject(string word, float waitTime, Vector3 position)
    {

        if (!_isCreatingWord)
        {
            _isCreatingWord = true;

            GameObject instantiatedWord = Instantiate(WordBlockPrefab);
            _currentWordObject = instantiatedWord;
            instantiatedWord.transform.position = position;
            //instantiatedWord.name = word;
            int n = instantiatedWord.GetComponent<IWordSelectionResponse>().Initialize(word);

            yield return new WaitForSecondsRealtime(waitTime);
            _isCreatingWord = false;
        }
    }

    private IEnumerator CreateImageObject(string word, float waitTime, Vector3 position)
    {

        if (!_isCreatingImage)
        {
            _isCreatingImage = true;

            GameObject instantiatedImageObject = Instantiate(ImageBlockPrefab);
            _currentImageObject = instantiatedImageObject;
            instantiatedImageObject.transform.position = position;
            instantiatedImageObject.GetComponent<IPictureSelectionResponse>().InitializePicure(word);

            yield return new WaitForSecondsRealtime(waitTime);
            _isCreatingImage = false;
        }
    }

    void OnTwoFingerOneap(Touch touch)
    {
        if(Input.touchCount == 2)
        {
            Previous();
        }
    }

    void OnOneFingerTwoTaps(Touch touch)
    {
        if(Input.touchCount == 1 && touch.tapCount == 2 && touch.phase == TouchPhase.Ended)
        {
            Next();
        }
    }

    public void Next()
    {
        if (!_isCreatingWord)
        {
            DisableCurrentWordImage();
            NextIndex();
            InstantiateWordImage();
        }
    }

    public void Previous()
    {
        if (!_isCreatingWord)
        {
            DisableCurrentWordImage();
            PreviousIndex();
            InstantiateWordImage();
        }
    }

    private void InstantiateWordImage()
    {
        string word = _completeWordList[_wordIndex];

        //WORD
        if (_readWordsObject[_wordIndex] != null)
        {
            _currentWordObject = _readWordsObject[_wordIndex];
            _currentWordObject.SetActive(true);
        }
        else
        {
            Vector2 position = CalculatePosition(word);
            StartCoroutine(CreateWordObject(word, WordSpawnWaitTime, position));
        }

        //IMAGE
        if (_readImagesObjects[_wordIndex] != null)
        {
            _currentImageObject = _readImagesObjects[_wordIndex];
            _currentImageObject.SetActive(true);
        }
        else
        {
            StartCoroutine(CreateImageObject(word, WordSpawnWaitTime, new Vector2(0, 0)));
        }
    }

    private void DisableCurrentWordImage()
    {
        if (_currentWordObject)
        {
            _readWordsObject[_wordIndex] = _currentWordObject;
            _currentWordObject.SetActive(false);
        }
        if (_currentImageObject)
        {
            _readImagesObjects[_wordIndex] = _currentImageObject;
            _currentImageObject.SetActive(false);
        }
    }

    ////calculates the position to place on screen
    private Vector3 CalculatePosition(string word)
    {
        //float widthCenterPosition = (Screen.width / 2)/Screen.width * (WorldUnitSize * WidthAspectRatio);
        //Debug.Log($"center position X: {widthCenterPosition}");

        //float heightCenterPosition = (Screen.height / 2)/Screen.height * (WorldUnitSize * HeightAspectRatio);
        //Debug.Log($"center position Y: {heightCenterPosition}");

        var letterBlockSpriteRenderer = LetterBlockPrefab.GetComponent<SpriteRenderer>();
        float letterHeight = letterBlockSpriteRenderer.bounds.size.y; // for starting position
        float PositionY = (WorldUnitSize/2 * -1) + letterHeight/2;

        return new Vector2(0, PositionY);
    }
}
