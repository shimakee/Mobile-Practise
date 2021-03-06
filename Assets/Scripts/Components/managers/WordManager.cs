﻿
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class WordManager : MonoBehaviour
{
  
    public GameObject WordBlockPrefab;
    public GameObject LetterBlockPrefab;
    public GameObject ImageBlockPrefab;
    public GameOptions GameOptions;
    public int TotalWorldUnits = 10;

    //worldsize
    //public int WorldUnitSize = 10;
    public GameObject[] WordObjects { get { return _wordsObject; } }
    public GameObject[] ImageObjects { get { return _imagesObjects; } }
    public string[] WordList { get { return _completeWordList; } }
    public string CurrentWord { get { return _completeWordList[CurrentIndex]; } }
    //public GameObject CurrentWordObject { get { return WordObjects[CurrentIndex]; } }
    public int CurrentIndex { get { return _wordsListIndex[_currentWordListIndex]; } }
    public int CurrentIndexRunner { get { return _currentWordListIndex;  } }
    public int MaxIndex; //maxIndex before looping/repeating

    //list of words
    private string[] _completeWordList;
    //list of words as word/image game object
    private GameObject[] _wordsObject; //object of words already read; (so as not to destroy and instantiate them again and again
    private GameObject[] _imagesObjects;
    //list index
    private int[] _wordsListIndex; //list of index for all words arranged or shuffled based on option.
    private int _currentWordListIndex = 0; //this is the index that will run up and down the wordsListIndex.
    
    //current image or word
    private GameObject _currentWordObject;
    private GameObject _currentImageObject;
    private void Awake()
    {
        //check that there is a prefab
        if (!WordBlockPrefab)
            throw new NullReferenceException("must have a reference to the wordBlock object to instantiate.");

        GameOptions.PropertyChanged += OptionPropertyChanged;
    }
    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public List<string> GenerateWordListFromTxt(string textFileName)
    {
        TextAsset textFile = Resources.Load<TextAsset>($"wordList/{textFileName}") ?? throw new NullReferenceException("no txt asset with word list:");

        //reads text file and generates text array of strings.
        List<string> wordList = textFile.ToString().Split('\n').ToList();
        List<string> trimmedWordList = new List<string>();

        foreach (var word in wordList)
        {
            trimmedWordList.Add(word.Trim().ToLower());
        }

        trimmedWordList.Sort();
        return trimmedWordList;
    }

    public void LoadWordsToLoadManager(List<string> wordList)
    {
        int length = wordList.Count;
        _completeWordList = wordList.ToArray();
        _wordsObject = new GameObject[length];
        _imagesObjects = new GameObject[length];
        _wordsListIndex = GetIndexSet(length);

        if (GameOptions.Shuffle)
            ShuffleIndex(_wordsListIndex);
        MaxIndex = length - 1;
    }
    public void ResetIndex()
    {
        _currentWordListIndex = 0;
    }

    private int[] GetIndexSet(int length)
    {
        return Enumerable.Range(0, length).ToArray();
    }
    private void ShuffleIndex(int[] intSet)
    {
        if (intSet == null)
            return;

        System.Random random = new System.Random();

        int length = intSet.Length;
        //Debug.Log($"lenght {length}");

        for (int i = _currentWordListIndex ; i < intSet.Length; i++)
        {
            int rng = random.Next(i, length);

            int temp = intSet[i];
            intSet[i] = intSet[rng];
            intSet[rng] = temp;

            //Debug.Log($"index rng {rng}");
            //Debug.Log($"index i {i}");
            //Debug.Log($"temp value {temp}");
            //Debug.Log($"i value {intSet[i]}");
            //Debug.Log($"rng value {intSet[rng]}");
        }

        //return intSet;
    }

    private void SortIndex(int[] intSet)
    {
        if (intSet == null)
            return;

        int[] tempArray =  new int[intSet.Length - (_currentWordListIndex + 1)];

        Array.Copy(intSet, _currentWordListIndex + 1, tempArray, 0, tempArray.Length);

        Array.Sort(tempArray);

        Array.Copy(tempArray, 0, intSet, _currentWordListIndex + 1, tempArray.Length);
    }
    private GameObject CreateWordObject(string word, Vector3 position, float scale)
    {
        GameObject instantiatedWord = Instantiate(WordBlockPrefab, transform);
        instantiatedWord.transform.position = position;
        instantiatedWord.GetComponent<IWordSelectionResponse>().Initialize(word);
        instantiatedWord.transform.localScale = new Vector2(scale, scale);
        //_wordsObject[CurrentIndex] = instantiatedWord;
        return instantiatedWord;
    }

    //private IEnumerator CreateImageObject(string word, float waitTime, Vector3 position)
    private GameObject CreateImageObject(string word, Vector3 position, float imageScale)
    {

        GameObject instantiatedImageObject = Instantiate(ImageBlockPrefab, transform);
        instantiatedImageObject.transform.position = position;

        float scale = CalculateImageScale() * imageScale;
        instantiatedImageObject.transform.localScale = new Vector2(scale, scale);
        instantiatedImageObject.GetComponent<IPictureSelectionResponse>().InitializePicture(word);
        //_imagesObjects[CurrentIndex] = instantiatedImageObject;

        return instantiatedImageObject;
    }

    public void NextIndex()
    {
        //Debug.Log($"current index {_currentWordListIndex}", this);
        if (_currentWordListIndex < MaxIndex)
        {
            _currentWordListIndex++;
        }
        else
        {
            //if repeat is on loop back to the start
            if (GameOptions.Repeat)
            {
                _currentWordListIndex = 0;
            }
        }
        //Debug.Log($"current index ++ {_currentWordListIndex}", this);

    }

    public void PreviousIndex()
    {
        //Debug.Log($"current index {_currentWordListIndex}", this);

        if (_currentWordListIndex > 0)
            _currentWordListIndex--;

        //Debug.Log($"current index ++ {_currentWordListIndex}", this);

    }
    public void ClearWordList()
    {
        for (int i = 0; i < _wordsObject.Length; i++)
        {
            if (_wordsObject[i] != null)
                Destroy(_wordsObject[i]);
        }

        for (int i = 0; i < _imagesObjects.Length; i++)
        {
            if (_imagesObjects[i] != null)
                Destroy(_imagesObjects[i]);
        }
    }


    public GameObject InstantiateCurrentWord(Vector3 position, float scale = 1)
    {
        _currentWordObject =  InstantiateWordOnIndex(position, CurrentIndex, scale);
        return _currentWordObject;
    }
    public GameObject InstantiateWordOnIndex(Vector3 position, int index, float scale = 1)
    {
        string word = _completeWordList[index];

        GameObject wordOject = null;
        //WORD
        if (_wordsObject[index] != null)
        {
            //Debug.Log($"word is instantiated {_wordsObject[CurrentIndex].name}", _wordsObject[CurrentIndex]);
            wordOject = _wordsObject[index];
            wordOject.SetActive(true);
            wordOject.transform.position = position;
        }
        else
        {
            //Vector2 position = CalculatePosition(word);
            //StartCoroutine(CreateWordObject(word, WordSpawnWaitTime, position));

            wordOject = CreateWordObject(word, position, scale);
            _wordsObject[index] = wordOject;
        }

        //OptionPropertyChanged();
        return wordOject;
    }

    public GameObject InstantiateCurrentImage(Vector3 position, float scale = 1)
    {
        _currentImageObject = InstantiateImageOnIndex(position, CurrentIndex, scale);
        return _currentImageObject;
    }

    public GameObject InstantiateImageOnIndex(Vector3 position, int index, float scale = 1)
    {
        string word = _completeWordList[index];
        GameObject imageObject = null;
        //IMAGE
        if (_imagesObjects[index] != null)
        {
            imageObject = _imagesObjects[index];
            imageObject.SetActive(true);
            imageObject.transform.position = position;
        }
        else
        {
            //StartCoroutine(CreateImageObject(word, WordSpawnWaitTime, new Vector2(0, 0)));
            imageObject = CreateImageObject(word, position, scale);
            _imagesObjects[index] = imageObject;
        }

        return imageObject;
    }

    public void DisableCurrentWord()
    {
        DisableWordOnIndex(CurrentIndex);
    }

    public void DisableWordOnIndex(int i)
    {
        //for word
        if(_wordsObject != null)
        {
            if (_wordsObject[i])
            {
                Debug.Log($"disabled {_wordsObject[i]} word");
                _wordsObject[i].SetActive(false);
            }
            else
            {
                Debug.Log("no word object on that index.");
            }
        }
        else
        {
            Debug.Log("words object is null.");
        }
    }

    public void DisableCurrentImage()
    {
        DisableImageOnIndex(CurrentIndex);
    }

    public void DisableImageOnIndex(int i)
    {
        //for word
        if (_imagesObjects != null)
        {
            if (_imagesObjects[i])
            {
                Debug.Log($"disabled {_imagesObjects[i]} image");
                _imagesObjects[i].SetActive(false);
            }
            else
            {
                Debug.Log("no image object on that index.");
            }
        }
        else
        {
            Debug.Log("image object is null.");
        }
    }

    public void OptionPropertyChanged()
    {

        if (GameOptions.Shuffle)
        {
            ShuffleIndex(_wordsListIndex);
        }
        else
        {
            SortIndex(_wordsListIndex);
        }

        if (!_currentWordObject)
            return;

        var component = _currentWordObject.GetComponent<IWordSelectionResponse>();
        if (GameOptions.LetterCasingOptions == LetterCasingOptions.lower)
            component.ToLower();
        if (GameOptions.LetterCasingOptions == LetterCasingOptions.upper)
            component.ToUpper();
        if (GameOptions.LetterCasingOptions == LetterCasingOptions.standard)
            component.ToStandard();
    }

    float CalculateImageScale()
    {
        //get aspect ratio
        int pixelPerUnit = Screen.height/TotalWorldUnits;

        return pixelPerUnit;
    }
}
