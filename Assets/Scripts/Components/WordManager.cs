
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WordManager : MonoBehaviour
{
    public string TextAssetName = "wordList";
    public TextAsset WordsListTextFile;
    public GameObject WordBlockPrefab;
    public float WordSpawnWaitTime = 3f;

    //Options
    public bool Shuffle;
    [Range(0, 3)] public int Repeat;
    //[Range(1, 5)] public int MinWordLength;
    //[Range(1, 5)] public int MaxWordLength;

    //wordList
    private List<string> _completeWordList;
    private GameObject[] _readWordsObject; //object of words already read; (so as not to destroy and instantiate them again and again

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
    private GameObject _currentWordObject;
    private void Awake()
    {
        //check that wordlist text file exist
        //TextAssetName will be  changed based on mode?
        if (!WordsListTextFile)
            WordsListTextFile = Resources.Load<TextAsset>(TextAssetName);

        //generate list of words.
        _completeWordList = GenerateWordListFromTxt(WordsListTextFile);

        Debug.Log($"complete wordlist length {_completeWordList.Count}");

        foreach (var item in _completeWordList)
        {
            Debug.Log($"{item} is in complete word list");
        }

        //assign max index;
        _maxIndex = _completeWordList.Count - 1;
        _readWordsObject = new GameObject[_completeWordList.Count];

        Debug.Log($"max index {_maxIndex}");

        //check that there is a prefab
        if (!WordBlockPrefab)
            throw new NullReferenceException("must have a reference to the wordBlock object to instantiate.");
    }
    private void Start()
    {
        //int wordListLength = _completeWordList.Count - 1;

        //generate array indexes - should be based on options
        _wordsListIndex = GetArrangedIndex(_completeWordList);

        //generate first word
        StartCoroutine(CreateWordObject(_completeWordList[_wordIndex], WordSpawnWaitTime, new Vector2(0, 0)));
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

    void NextWord()
    {
        if(_currentWorListIndex < _maxIndex)
        {
            _currentWorListIndex++;
        }
        else
        {
            //if repeat is on loop back to the start
            if (Repeat > 0)
                _currentWorListIndex = 0;
        }

        Debug.Log($"wordlist length = {_wordsListIndex.Length}");
        Debug.Log($"currentWordListIndex = {_currentWorListIndex}");
    }

    void PreviousWord()
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
            instantiatedWord.name = word;
            int n = instantiatedWord.GetComponent<IWordSelectionResponse>().InitializeWord(word);

            yield return new WaitForSecondsRealtime(waitTime);
            _isCreatingWord = false;
        }
    }

    private void OnTwoFingerOneap(Touch touch)
    {
        if(Input.touchCount == 2)
        {
            if (!_isCreatingWord)
            {
                //hide previous object
                //assign it to the _readWordsObject[WordIndex];

                //for now destroy object
                Destroy(_currentWordObject);

                PreviousWord();
                Vector3 position = new Vector3(0, 0, 0); // could be set as global or on a larger scope


                ////check readList if it contains that object
                //if (_readWordsObject[WordIndex] != null)
                //{
                //    //re-enableCollider
                //    //unhide object
                //    //re-assign as current object
                //}
                //else
                //{
                //    //instantiate/create new object
                //    //assign as current object
                string word = _completeWordList[_wordIndex];
                Debug.Log($"word generated = {word}");

                StartCoroutine(CreateWordObject(word, WordSpawnWaitTime, position));
                //}
            }
        }
    }

    private void OnOneFingerTwoTaps(Touch touch)
    {
        if(Input.touchCount == 1 && touch.tapCount == 2 && touch.phase == TouchPhase.Ended)
        {
            if (!_isCreatingWord)
            {
                //hide previous object
                //assign it to the _readWordsObject[WordIndex];

                //for now destroy object
                if(_currentWordObject)
                    Destroy(_currentWordObject);

                NextWord();
                Vector3 position = new Vector3(0, 0, 0); // could be set as global or on a larger scope


                ////check readList if it contains that object
                //if (_readWordsObject[WordIndex] != null)
                //{
                //    //re-enableCollider
                //    //unhide object
                //    //re-assign as current object
                //}
                //else
                //{
                //    //instantiate/create new object
                //    //assign as current object
                        StartCoroutine(CreateWordObject(_completeWordList[_wordIndex], WordSpawnWaitTime, position));
                //}
            }
        }
    }

    ////calculates the position to place on screen
    //private void CalculatePosition()
    //{
    //    float width = Screen.width;
    //    float height = Screen.height;
    //}
}
