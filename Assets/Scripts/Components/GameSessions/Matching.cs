using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matching : MonoBehaviour, IGameSession
{
    public string TextAssetName = "initialWordList";
    public GameObject WordMangerCanvas;
    public WordManager WordManager { get; private set; }
    [Range(1f, 10f)] public float ImageSize = 4;

    public GameObject DragableContainer;
    public int numberOfWords = 3;
    private int _numberOfWordsCreated = 0;
    private bool isLastSet = false;

    private GameOptions _gameOptions;
    private UiManager _uiManager;
    private AudioManager _audioManager;

    private Vector2[] _coordinatesImage;
    private Vector2[] _coordinatesWords;
    private Vector2[] _coordinatesContainer;

    private List<int> _objectIndexesCurrentlyShown = new List<int>();
    private int _numberOfWordsMatched = 0;

    //TODO
    //create word object where letter does not respond but word does.

    //create another gameplay where you assemble letters.
    //have them bounce around?


    private void Awake()
    {
        WordManager = WordMangerCanvas.GetComponent<WordManager>() ?? throw new NullReferenceException("no word manager");
        _gameOptions = WordManager.GameOptions;
        _audioManager = FindObjectOfType<AudioManager>();
        _uiManager = FindObjectOfType<UiManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SessionPause()
    {
        throw new System.NotImplementedException();
    }

    public void SessionResume()
    {
        throw new System.NotImplementedException();
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

        WordManager.ResetIndex();

        CreateSet();
    }

    public void SessionEnd()
    {
        _audioManager.Play("Success");

        SessionReset();
        _uiManager.SwitchCanvas(UiType.gameEnd);
        Debug.Log("Session has ended.");
    }

    public void SessionReset()
    {
        if (WordManager)
        {
            if (WordManager.WordObjects != null)
            {
                if (WordManager.WordObjects[WordManager.CurrentIndex] != null)
                    WordManager.WordObjects[WordManager.CurrentIndex].SetActive(false);
                WordManager.ClearWordList();
            }
        }

        ClearSetVariables();
        isLastSet = false;
    }

    //private void CreateDragableContainer(string name, Vector3 position)
    //{
    //    var container = GameObject.Instantiate(DragableContainer, position, Quaternion.identity);
    //    container.GetComponent<DragableContainer>().ContainerName = name;
    //    container.name = name + "Container";
    //}

    //private void CreateCoordinates(int number)
    //{
    //}

    private void CreateSet()
    {
        _numberOfWordsCreated = 0;

        //CreateCoordinates(numberOfWords);
        _coordinatesImage = new Vector2[numberOfWords];
        _coordinatesWords = new Vector2[numberOfWords];
        _coordinatesContainer = new Vector2[numberOfWords];


        for (int i = 0; i < numberOfWords; i++)
        {
            float distanceIncrement = 1 / (float)(numberOfWords + 1);
            float xPosition = distanceIncrement * (i + 1);

            _coordinatesImage[i] = Camera.main.ViewportToWorldPoint(new Vector3(xPosition, 0.25f, 0));
            _coordinatesWords[i] = Camera.main.ViewportToWorldPoint(new Vector3(xPosition, 0.65f, 0));
            _coordinatesContainer[i] = Camera.main.ViewportToWorldPoint(new Vector3(xPosition, 0.65f, 0));
        }

        ShuffleIndex(_coordinatesImage);
        //ShuffleIndex(_coordinatesWords);
        //ShuffleIndex(_coordinatesContainer);

        for (int i = 0; i < numberOfWords; i++)
        {
            //if(WordManager.CurrentIndexRunner < WordManager.MaxIndex)
            //{
            _objectIndexesCurrentlyShown.Add(WordManager.CurrentIndex);
                Debug.Log($"index added {WordManager.CurrentIndex}");

                

                var wordObject = WordManager.InstantiateCurrentWord(_coordinatesWords[i], .5f);
                if (wordObject == null)
                {
                    Debug.Log("word is null");
                }
                else
                {
                    Debug.Log($"word is {wordObject.name}");
                }
                var wordContainer = wordObject.GetComponent<DragableContainer>();
                var collider = wordObject.GetComponent<CircleCollider2D>();
                if (wordContainer == null)
                {
                    wordContainer = wordObject.AddComponent<DragableContainer>();
                }
                if (collider == null)
                {
                        collider = wordObject.AddComponent<CircleCollider2D>();
                }

                collider.isTrigger = true;
                collider.radius = .5f;
                wordContainer.ContainerName = WordManager.CurrentWord;

                var imageObject = WordManager.InstantiateCurrentImage(_coordinatesImage[i], ImageSize);
                var dragSnapComponent = imageObject.GetComponent<DragSnapSelectionResponse>();
                if (dragSnapComponent != null)
                {
                    dragSnapComponent.MatchingSession = this;
                    dragSnapComponent.isSet = false;
                    dragSnapComponent.isMatch = false;
                    dragSnapComponent.OriginalPosition = _coordinatesImage[i];
                }

            _numberOfWordsCreated++;

            int previousIndex = WordManager.CurrentIndex;
            WordManager.NextIndex();

            if (previousIndex == WordManager.CurrentIndex)
            {
                isLastSet = true;
                Debug.Log("do something to check for end game Match session");
                break;
            }
                //endGame
            //}
        }
    }


    private void ShuffleIndex(Vector2[] intSet)
    {
        if (intSet == null)
            return;

        System.Random random = new System.Random();

        int length = intSet.Length;
        //Debug.Log($"lenght {length}");

        for (int i = 0; i < intSet.Length; i++)
        {
            int rng = random.Next(i, length);

            Vector2 temp = intSet[i];
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
    //public static event Action<string> WordMatched;

    //public static void OnWordMatched(string word)
    //{
    //    WordMatched?.Invoke(word);
    //}

    public void WordMatched(string word)
    {
        _numberOfWordsMatched++;

        if(_numberOfWordsMatched >= _numberOfWordsCreated || _numberOfWordsMatched >= numberOfWords)
        {
            if (isLastSet)
            {
                SessionEnd();
            }
            else
            {
                StartCoroutine(NextSet());
            }
        }
    }

    private IEnumerator NextSet()
    {
        //play affirmation audio
        _audioManager.Play("Open");

        yield return new WaitForSeconds(2);

        ClearSetVariables();
        CreateSet();
    }

    private void ClearSetVariables()
    {


        foreach (var item in _objectIndexesCurrentlyShown)
        {
            WordManager.DisableImageOnIndex(item);
            WordManager.DisableWordOnIndex(item);
        }
        _numberOfWordsMatched = 0;
        _objectIndexesCurrentlyShown.Clear();
    }
}
