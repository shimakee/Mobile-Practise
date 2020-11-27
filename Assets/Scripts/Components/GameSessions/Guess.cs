using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guess : MonoBehaviour, IGameSession
{
    public WordManager WordManager { get; private set; }
    public GameObject WordManagerCanvas;
    public string TextAssetName = "initialWordList";

    [Range(1,5)] public int NumberOfExtraImages = 2;

    private GameOptions _gameOptions;
    private AudioManager _audioManager;
    private UiManager _uiManager;

    private List<string> _currentWordsOnSet;
    //private List<GameObject> _currentImageObjectsOnSet;
    private int[] _indexesOfImagesOnSet;


    void Awake()
    {
        WordManager = WordManagerCanvas.GetComponent<WordManager>() ?? throw new NullReferenceException("no word manager");
        _gameOptions = WordManager.GameOptions;
        _audioManager = FindObjectOfType<AudioManager>();
        _uiManager = FindObjectOfType<UiManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _currentWordsOnSet = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {

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

        GenerateSet();
    }

    public void SessionPause()
    {
        throw new System.NotImplementedException();
    }
    public void SessionResume()
    {
        throw new System.NotImplementedException();
    }

    public void SessionReset()
    {
        DisableCurrentSet();

        if (WordManager)
        {
            WordManager.ResetIndex();

            if (WordManager.WordObjects != null)
            {
                if (WordManager.WordObjects[WordManager.CurrentIndex] != null)
                    WordManager.WordObjects[WordManager.CurrentIndex].SetActive(false);
                WordManager.ClearWordList();
            }
        }
    }

    public void SessionEnd()
    {
        _uiManager.SwitchCanvas(UiType.gameEnd);
        _audioManager.Play("Success");

        DisableCurrentSet();
        if (WordManager)
        {
            WordManager.ResetIndex();

            if (WordManager.WordObjects != null)
            {
                if (WordManager.WordObjects[WordManager.CurrentIndex] != null)
                    WordManager.WordObjects[WordManager.CurrentIndex].SetActive(false);
                WordManager.ClearWordList();
            }
        }
    }

    public void Next()
    {
        if (WordManager.CurrentIndex == WordManager.MaxIndex)
            SessionEnd();

        WordManager.NextIndex();
        _currentWordsOnSet.Clear();

        //WordManager.DisableCurrentImage();
        DisableCurrentSet();

        GenerateSet();
    }

    private void DisableCurrentSet()
    {
        foreach (var index in _indexesOfImagesOnSet)
        {
            WordManager.DisableImageOnIndex(index);
        }
    }

    private void GenerateSet()
    {
        int numberOfImagesToGenerate = NumberOfExtraImages + 1;
        List<Vector2> positions = GeneratePositions(numberOfImagesToGenerate);
        _indexesOfImagesOnSet = new int[numberOfImagesToGenerate];

        for (int i = 0; i < numberOfImagesToGenerate; i++)
        {
            var random = new System.Random();
            //var maxValue = positions.Count > 0 ? positions.Count - 1 : 0;
            int number = random.Next(0, positions.Count - 1);

            if (i == 0)
            {
                var currentObject = WordManager.InstantiateCurrentImage(positions[number]);
                _currentWordsOnSet.Add(WordManager.CurrentWord);

                _indexesOfImagesOnSet[i] = WordManager.CurrentIndex;
                //_currentImageObjectsOnSet.Add(currentObject);
            }
            else
            {
                GameObject extraImageObject = null;
                int randomIndex = 0;
                do
                {
                    randomIndex = random.Next(0, WordManager.WordList.Length);
                    extraImageObject = WordManager.InstantiateImageOnIndex(positions[number], randomIndex);
                } while (WordManager.CurrentWord == extraImageObject.name || _currentWordsOnSet.Contains(extraImageObject.name));

                _currentWordsOnSet.Add(extraImageObject.name);
                _indexesOfImagesOnSet[i] = randomIndex;
                //_currentImageObjectsOnSet.Add(extraImageObject);
            }


            positions.RemoveAt(number);

            foreach (var item in _currentWordsOnSet)
            {
                Debug.Log("each item is " + item);
            }
        }

    }

    private List<Vector2> GeneratePositions(int numberOfPositions)
    {
        List<Vector2> positions = new List<Vector2>();

        int numberOfRows = 1;
        int MaxPositionsPerRow = 3;
        int numberOfPositionsMade = 0;

        if (numberOfPositions > 2)
            numberOfRows = 2;

        float[] yPositions = GenerateScreenPointsDivision(numberOfRows);

        for (int i = 0; i < numberOfRows; i++)
        {
            if(numberOfPositions <= 5)
            {
                if (numberOfPositions == 3 && i == 0)
                    MaxPositionsPerRow = 1;
                if (numberOfPositions > 3 && i == 0)
                    MaxPositionsPerRow = 2;
            }
            int numberOfPositionsToGenerate = (numberOfPositions - numberOfPositionsMade) <= MaxPositionsPerRow ? (numberOfPositions - numberOfPositionsMade) : MaxPositionsPerRow;

            float[] xPositions = GenerateScreenPointsDivision(numberOfPositionsToGenerate);

            numberOfPositionsMade += xPositions.Length;
            //Debug.Log("number of positions" + numberOfPositionsToGenerate, this);
            for (int z = 0; z < xPositions.Length; z++)
            {
                //Debug.Log("position" + z, this);
                positions.Add(Camera.main.ViewportToWorldPoint(new Vector3(xPositions[z], yPositions[i], 0)));
            }

            MaxPositionsPerRow = 3;
        }

        return positions;
    }

   private float[] GenerateScreenPointsDivision(int numberOfPositions)
    {

        float[] positions = new float[numberOfPositions];

        for (int i = 0; i < numberOfPositions; i++)
        {
            float distanceIncrement = 1 / (float)(numberOfPositions + 1);
            float xPosition = distanceIncrement * (i + 1);

            positions[i] = xPosition;
        }

        return positions;
    }

}
