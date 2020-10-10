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

    private GameOptions _gameOptions;
    private UiManager _uiManager;
    private AudioManager _audioManager;

    private Vector2[] _coordinatesImage;
    private Vector2[] _coordinatesWords;
    private Vector2[] _coordinatesContainer;

    //TODO
    //create word object where letter does not respond but word does.
    //shuffle words
    //create options - image to image or word to image matching
    //options before startin gameplay
       // number of words
       //number of images

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

    public void SessionPause(GameObject pauseCanvas)
    {
        throw new System.NotImplementedException();
    }

    public void SessionResume(GameObject pauseCanvas)
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
            var wordObject = WordManager.InstantiateWord(_coordinatesWords[i], .5f);
            wordObject.AddComponent<DragableContainer>();
            wordObject.AddComponent<CircleCollider2D>();
            var collider = wordObject.GetComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = .5f;
            wordObject.GetComponent<DragableContainer>().ContainerName = WordManager.CurrentWord;
            WordManager.InstantiateImage(_coordinatesImage[i], ImageSize);
            //CreateDragableContainer(WordManager.CurrentWord, _coordinatesContainer[i]);
            WordManager.NextIndex();
        }
    }

    public void SessionEnd()
    {
        throw new System.NotImplementedException();
    }

    private void CreateDragableContainer(string name, Vector3 position)
    {
        var container = GameObject.Instantiate(DragableContainer, position, Quaternion.identity);
        container.GetComponent<DragableContainer>().ContainerName = name;
        container.name = name + "Container";
    }

    private void CreateCoordinates(int number)
    {
        _coordinatesImage = new Vector2[numberOfWords];
        _coordinatesWords = new Vector2[numberOfWords];

        for (int i = 0; i < numberOfWords; i++)
        {
            float distanceIncrement = 1 / (float)(numberOfWords + 1);
            float xPosition = distanceIncrement * (i + 1);
            _coordinatesImage[i] = Camera.main.ViewportToWorldPoint(new Vector3(xPosition, 0.25f, 0));
            _coordinatesWords[i] = Camera.main.ViewportToWorldPoint(new Vector3(xPosition, 0.65f, 0));
        }
    }
}
