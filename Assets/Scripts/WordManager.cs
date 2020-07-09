
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WordManager : MonoBehaviour
{
    public TextAsset WordsListTextFile;
    public string TextAssetName = "wordList";
    public GameObject WordBlockPrefab;
    public float wordSpawnWaitTime = 3f;

    private List<string> _wordList;
    private bool _isCreatingWord = false;
    private string _currentWordString = "";
    private GameObject _currentWordObject;
    private WordController _currentWordObjectController;
    private void Awake()
    {
        //check that wordlist text file exist
        if (!WordsListTextFile)
            WordsListTextFile = Resources.Load<TextAsset>(TextAssetName);
        GenerateWordListFromTxt(WordsListTextFile);
        
        //check that there is a prefab
        if (!WordBlockPrefab)
            throw new NullReferenceException("must have a reference to the wordBlock object to instantiate.");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0)
        {
            foreach (var touch in Input.touches)
            {
                OnTouchTwoTaps(touch);
            }
        }
    }

    private List<string> GenerateWordListFromTxt(TextAsset textAsset)
    {
        //reads text file and generates text array of strings.
        _wordList = WordsListTextFile.ToString().Split('\n').ToList();
        List<string> trimmedWordList = new List<string>();

        foreach (var word in _wordList)
        {
            trimmedWordList.Add(word.Trim());
            Debug.Log($"-{word}- is in list.", this);
        }

        _wordList = trimmedWordList;

        return _wordList;
    }

    private string PickRandomWord(List<string> wordList)
    {
        //check that the new word does not equal to the word already generated.
        int randomNumber = UnityEngine.Random.Range(0, wordList.Count);
        Debug.Log($"Word picked -{wordList[randomNumber]}");
        return wordList[randomNumber];
    }

    private IEnumerator CreateWordObject(string word, Vector3 position)
    {

        if (!_isCreatingWord)
        {
            _isCreatingWord = true;
            _currentWordString = word;

            Debug.Log($"creating word -{word}");
            //prevent duplicates or multiple words
            //if (_currentWordObject != null)
            Destroy(_currentWordObject);

            _currentWordObject = Instantiate(WordBlockPrefab);
            _currentWordObject.transform.position = position;
            int n = _currentWordObject.GetComponent<WordController>().InitializeWord(word);

            //when you redesign initialize word to return -1 when not all assets are loaded or cant be found.
            //create loop to pick another word.
            //if(n == -1)
                //pick another object

            Debug.Log($"the word from list is -{word}- with return value {n}.");
            yield return new WaitForSecondsRealtime(wordSpawnWaitTime);
            _isCreatingWord = false;
        }
    }

    private void OnTouchTwoTaps(Touch touch)
    {
        if(touch.tapCount == 2 && touch.phase == TouchPhase.Ended)
        {
            if (!_isCreatingWord)
            {
                Vector3 position = new Vector3(0, 0, 0);
                string word = PickRandomWord(_wordList);
                StartCoroutine(CreateWordObject(word, position));
            }
        }
    }
}
