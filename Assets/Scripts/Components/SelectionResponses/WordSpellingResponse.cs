using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WordSpellingResponse : WordSelectionResponse, IWordSelectionResponse
{

    public GameObject MissingLetterBlockPrefab;
    public GameObject DragableContainer;

    private GameObject[] letterChildrenObjects;
    private GameObject _deactivatedLetter;
    private char _deactivatedChar;
    private GameObject _replacementLetter;
    private DragableContainer _container;

    private void Awake()
    {
        _audioManager = FindObjectOfType<AudioManager>();
        _audioSource = GetComponent<AudioSource>();

        LetterSpellingResponse.LetterMatched += LetterMattched;

        if (!_audioManager)
            throw new NullReferenceException("no audio manager comppnent attached.");
        if (!_audioSource)
            throw new NullReferenceException("no audio source comppnent attached.");
        if (!GameOptions)
            throw new NullReferenceException("no game options to load.");
    }

    protected override void AssembleLetters(string word) // create overload for offset and margins?
    {
        var letterBlockSpriteRenderer = LetterBlockPrefab.GetComponent<RectTransform>();
        int length = word.Length;
        _letterChildren = new ILetterSelectionResponse[length];
        float scale = CalculateScale(word);
        letterChildrenObjects = new GameObject[length];

       

        //float letterWidth = letterBlockSpriteRenderer.rect.width * scale; // for starting position
        float letterWidth = ((letterBlockSpriteRenderer.rect.width / Screen.width) * WorldUnitSize) * scale;
        float initialAllowanceToCenterPosition = ((letterWidth * length) + (PerLetterMargin * length - 1)) / 2 - (letterWidth / 2); //less half since pivot point is at the center.

        //assemble the word using the letters
        int lettersInstantiated = 0;
        for (int i = 0; i < length; i++)
        {
            Vector3 objectPosition = new Vector3(transform.position.x + (letterWidth * lettersInstantiated) - initialAllowanceToCenterPosition, transform.position.y, transform.position.z);
            objectPosition.x += PerLetterMargin * lettersInstantiated;
            GameObject letterGameObject = Instantiate(LetterBlockPrefab, transform);
            letterChildrenObjects[i] = letterGameObject;
            letterGameObject.transform.position = objectPosition;
            letterGameObject.transform.localScale = new Vector2(scale, scale);
            ILetterSelectionResponse letterComponent = letterGameObject.GetComponent<ILetterSelectionResponse>();
            if (letterComponent != null)
            {
                letterComponent.Initialize(word[i]);
                _letterChildren[i] = letterComponent;
            }
            lettersInstantiated++;
        }

        //deactivate missing letter
        DeactivateRandomLetter(word);
        SwapToDragableLetter(word);
        AddVelocityToLetter();
        AddContainer(word);
        //AssignContainerName(_deactivatedChar.ToString());
    }

    private void DeactivateRandomLetter(string word)
    {
        int length = word.Length;
        System.Random random = new System.Random();
        int randomMissingLetterIndex = random.Next(0, length);

        _deactivatedLetter = letterChildrenObjects[randomMissingLetterIndex];
        if (_deactivatedLetter)
        {
            _deactivatedChar = _deactivatedLetter.GetComponent<ILetterSelectionResponse>().Letter.Symbol;
            Debug.Log(_deactivatedChar);
            //_deactivatedLetter.gameObject.SetActive(false);

            //_deactivatedLetter.GetComponent<TextMeshProUGUI>().enabled = false;
            _deactivatedLetter.GetComponent<TextMeshProUGUI>().text = "_";
            //_deactivatedLetter.GetComponent<Collider2D>().isTrigger = true;
        }
    }

    private void SwapToDragableLetter(string word)
    {
        float scale = CalculateScale(word);
        System.Random random = new System.Random();

        GameObject letterGameObject = Instantiate(MissingLetterBlockPrefab, transform);
        _replacementLetter = letterGameObject;
        letterGameObject.transform.localScale = new Vector2(scale, scale);
        letterGameObject.transform.position = new Vector2(1, 1);

        ILetterSelectionResponse letterComponent = letterGameObject.GetComponent<ILetterSelectionResponse>();
        if (letterComponent != null)
            letterComponent.Initialize(_deactivatedChar);
    }

    private void AddVelocityToLetter()
    {
        _replacementLetter.GetComponent<Rigidbody2D>().velocity = new Vector2(3, 3);
    }

    private void AddContainer(string word)
    {
        //float scale = CalculateScale(word);


        //GameObject container = Instantiate(DragableContainer, transform);
        _container = _deactivatedLetter.AddComponent<DragableContainer>();
        _container.ContainerName = _deactivatedChar.ToString();
        var collider = _deactivatedLetter.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 100;

        //_deactivatedLetter.gameObject.SetActive(true);
        //container.transform.position = _deactivatedLetter.transform.position;
        //container.transform.localScale = new Vector2(scale, scale);

        //_deactivatedLetter.gameObject.SetActive(false);

    }

    //private void AssignContainerName(string name)
    //{
    //    if (_container)
    //    {
    //        _container.GetComponent<DragableContainer>().name = name;
    //    }
    //}

    private void LetterMattched()
    {
        if(_deactivatedLetter)
            _deactivatedLetter.GetComponent<TextMeshProUGUI>().text = _deactivatedChar.ToString();
    }
}
