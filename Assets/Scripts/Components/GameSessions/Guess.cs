using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guess : MonoBehaviour, IGameSession
{
    public WordManager WordManager { get; private set; }
    public GameObject WordManagerCanvas;
    public string TextAssetName = "initialWordList";
    [Range(1f, 10f)] public float ImageSize = 2;
    [Range(1,5)] public int NumberOfExtraImages = 2;
    public GameObject CorrectMark;
    public GameObject WrongMark;
    public int TotalWorldUnits = 10;

    private GameOptions _gameOptions;
    private AudioManager _audioManager;
    private UiManager _uiManager;

    private List<string> _currentWordsOnSet;
    //private List<GameObject> _currentImageObjectsOnSet;
    private List<int> _indexesOfImagesOnSet;
    private bool _isImageSelected;

    private List<Vector2> _wrongMarkPositions;
    private List<GameObject> _wrongMarks;
    void Awake()
    {
        WordManager = WordManagerCanvas.GetComponent<WordManager>() ?? throw new NullReferenceException("no word manager");
        _gameOptions = WordManager.GameOptions;
        _audioManager = FindObjectOfType<AudioManager>();
        _uiManager = FindObjectOfType<UiManager>();

        PictureSelectionResponse.Selected += OnPictureSelected;
    }

    private void OnPictureSelected(GameObject obj, string name)
    {
        if(WordManager.CurrentWord != name)
        {
            if (_isImageSelected == false)
            {
                _isImageSelected = true;
                ProcessWrongSelection(obj);
            }
        }
        else
        {
            if (_isImageSelected == false)
            {
                _isImageSelected = true;
                StartCoroutine(ProcessCorrectSelection(obj));
            }
        }
    }

    private IEnumerator ProcessCorrectSelection(GameObject obj)
    {
        _audioManager.Play("Confirm");
        var correctMark = GameObject.Instantiate(CorrectMark, WordManager.transform);
        correctMark.transform.position = obj.transform.position;
        var scale = CalculateImageScale();
        correctMark.transform.localScale = new Vector2(scale, scale);

        DestroyWrongMarks();
        yield return new WaitForSeconds(1);

        Destroy(correctMark);
        Next();
        _isImageSelected = false;
    }

    private void ProcessWrongSelection(GameObject obj)
    {
        _audioManager.Play("Back");
        GameObject wrongMark = null;

        if (!_wrongMarkPositions.Contains(obj.transform.position))
        {
            wrongMark = GameObject.Instantiate(WrongMark, WordManager.transform);
            var scale = CalculateImageScale();
            wrongMark.transform.localScale = new Vector2(scale, scale);
            wrongMark.transform.position = obj.transform.position;
            _wrongMarkPositions.Add(obj.transform.position);
            _wrongMarks.Add(wrongMark);
        }

        //yield return new WaitForSeconds(2);

        //Destroy(wrongMark);
        _isImageSelected = false;
    }

    private void DestroyWrongMarks()
    {
        foreach (var item in _wrongMarks)
        {
            Destroy(item);
        }
    }

    public void PlayInstruction()
    {
        WordManager.ImageObjects[WordManager.CurrentIndex].GetComponent<IPictureSelectionResponse>().PlayWordAudio();
    }

    // Start is called before the first frame update
    void Start()
    {
        _currentWordsOnSet = new List<string>();
        _wrongMarkPositions = new List<Vector2>();
        _wrongMarks = new List<GameObject>();

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

        _currentWordsOnSet.Clear();
        _indexesOfImagesOnSet.Clear();
        _wrongMarkPositions.Clear();


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

        _currentWordsOnSet.Clear();
        _indexesOfImagesOnSet.Clear();
        _wrongMarkPositions.Clear(); 

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
        

        DisableCurrentSet();
        _currentWordsOnSet.Clear();
        _indexesOfImagesOnSet.Clear();
        _wrongMarkPositions.Clear();

        if (WordManager.CurrentIndexRunner == WordManager.MaxIndex)
        {
            SessionEnd();
            return;
        }

        WordManager.NextIndex();
        GenerateSet();
    }

    private void DisableCurrentSet()
    {
        if(_indexesOfImagesOnSet !=null)
        foreach (var index in _indexesOfImagesOnSet)
        {
            WordManager.DisableImageOnIndex(index);
        }
    }

    private void GenerateSet()
    {
        Debug.Log("index runner " + WordManager.CurrentIndexRunner + "/" + WordManager.MaxIndex);
        DisableCurrentSet();
        int numberOfImagesToGenerate = NumberOfExtraImages + 1;

        List<Vector2> positions = GeneratePositions(numberOfImagesToGenerate);
        if(_indexesOfImagesOnSet == null)
            _indexesOfImagesOnSet = new List<int>();
        _indexesOfImagesOnSet.Clear();
        _currentWordsOnSet.Clear();
        _wrongMarkPositions.Clear();


        var random = new System.Random();

        int positionOfCorrectWord = random.Next(0, numberOfImagesToGenerate-1);

        for (int i = 0; i < numberOfImagesToGenerate; i++)
        {

        //GameObject imageObject = null;
            if (i == positionOfCorrectWord)
            {
                WordManager.InstantiateCurrentImage(positions[i], ImageSize);
                _currentWordsOnSet.Add(WordManager.CurrentWord);

                _indexesOfImagesOnSet.Add(WordManager.CurrentIndex);
                Debug.Log("a " + WordManager.CurrentWord);
            }
            else
            {

                int randomIndex = 0;
                string imageObjectName = "";
                do
                {
                    randomIndex = random.Next(0, WordManager.WordList.Length - 1);
                    imageObjectName = WordManager.WordList[randomIndex];
                } while (_currentWordsOnSet.Contains(imageObjectName) || WordManager.CurrentWord == imageObjectName);

                _indexesOfImagesOnSet.Add(randomIndex);
                 WordManager.InstantiateImageOnIndex(positions[i], randomIndex, ImageSize);

                string word = WordManager.WordList[randomIndex];
                _currentWordsOnSet.Add(word);
                //_currentWordsOnSet.Add(imageObjectName);
                Debug.Log("b " + word );
            }
        }

        //foreach (var item in _currentWordsOnSet)
        //{
        //    Debug.Log("image name" + item);
        //}
        //Debug.Log("number of images" + numbersOfImage);
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
            if (numberOfPositions <= 5 &&numberOfPositions == 3 && i == 0)
                MaxPositionsPerRow = 1;
            if (numberOfPositions <= 5 &&numberOfPositions > 3 && i == 0)
                MaxPositionsPerRow = 2;

            int numberOfPositionsToGenerate = (numberOfPositions - numberOfPositionsMade) > MaxPositionsPerRow ? MaxPositionsPerRow : (numberOfPositions - numberOfPositionsMade);

            float[] xPositions = GenerateScreenPointsDivision(numberOfPositionsToGenerate);

            numberOfPositionsMade += xPositions.Length;
            //Debug.Log("number of positions" + numberOfPositionsToGenerate, this);
            for (int z = 0; z < xPositions.Length; z++)
            {
                //Debug.Log("position" + z, this);
                Vector2 position = Camera.main.ViewportToWorldPoint(new Vector3(xPositions[z], yPositions[i], 0));
                positions.Add(position);
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

    float CalculateImageScale()
    {
        //get aspect ratio
        int pixelPerUnit = Screen.height / TotalWorldUnits;

        return pixelPerUnit;
    }
}
