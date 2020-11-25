using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LetterSpellingResponse : LetterSelectionResponse, ILetterSelectionResponse
{
    private AudioManager _audioManager;
    private Rigidbody2D _rigidbody2D;

    //private Vector3 _originalScale;
    //private Vector3 _originalPosition;
    private Vector2 _targetPosition;
    private Vector2 _setPosition;

    private bool isSelected = false;

    private GameObject _container;

    public bool isSet = false;
    public bool onTrigger = false;
    public bool isMatch = false;

    public Spelling GameSession;
    //private bool _enabledRandomWalk;


    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        //_spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _audioManager = FindObjectOfType<AudioManager>();
        _textMeshPro = GetComponent<TextMeshProUGUI>();
        _parentWord = transform.parent.GetComponent<IWordSelectionResponse>();

        if (!_audioSource)
            throw new NullReferenceException("no audio source comppnent attached.");
        if (!_textMeshPro)
            throw new NullReferenceException("no text mesh pro comppnent attached.");
        if (!GameOptions)
            throw new NullReferenceException("no options");

        //if (!_spriteRenderer)
        //    throw new NullReferenceException("no sprite renderer comppnent attached.");
        //if (!GameOptions)
        //    throw new NullReferenceException("no options.");
        //_originalPosition = transform.position;
        //_originalScale = transform.localScale;
        _originalPosition = transform.position;
        _originalScale = transform.localScale;
    }

    void Start()
    {
        //_originalScale = this.gameObject.transform.localScale;
        //Debug.Log($"scale : {_originalScale.x}, {_originalScale.y}");
        //_originalPosition = this.gameObject.transform.position;

        _growScale = new Vector2((float)(_originalScale.x * 1.3), (float)(_originalScale.x * 1.3));
    }

    public override void Deselected(GameObject gameObject, Vector3 inputPosition)
    {
        isSelected = false;
        if (!isSet)
        {
            if (onTrigger)
            {
                if (isMatch)
                {
                    //this.transform.position = _setPosition;
                    //this.isSet = true;
                    SetPosition(_setPosition);

                    if (_container)
                    {
                        _container.GetComponent<TextMeshProUGUI>().enabled = true;
                        //_container.GetComponent<Collider2D>().isTrigger = false;

                        gameObject.SetActive(false);
                    }

                    _audioManager.Play("Confirm");

                    LetterMatched();

                }
                else
                {
                    //this.transform.position = _originalPosition;
                    _audioManager.Play("Back");

                    if (_rigidbody2D)
                    {
                        System.Random random = new System.Random();
                        _rigidbody2D.velocity = new Vector2(random.Next(1, 4), random.Next(1, 4));
                        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                    }
                }
            }
            else
            {
                //this.transform.position = _originalPosition;

                if (_rigidbody2D)
                {
                    System.Random random = new System.Random();
                    _rigidbody2D.velocity = new Vector2(random.Next(1, 4), random.Next(1, 4));
                    _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                }
            }
        }

        this.gameObject.transform.localScale = _originalScale;
    }

    public override void IsSelected(GameObject gameObject, Vector3 inputPosition)
    {
        if (!isSet)
        {
            _targetPosition = Camera.main.ScreenToWorldPoint(inputPosition);
            gameObject.transform.position = _targetPosition;
            this.GetComponent<SpriteRenderer>().color = Color.white;

            if (_rigidbody2D)
            {
                _rigidbody2D.velocity = new Vector2(0,0);
                _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            }
        }

        var scale = (float)(_originalScale.x * 1.3);
        this.gameObject.transform.localScale = new Vector3(scale, scale, scale);

        if (!isSelected)
        {
            PlayAudio();
        }

        isSelected = true;
    }

    public void SetPosition(Vector3 position)
    {
        isSet = true;
        this.gameObject.transform.position = position;
        //this.GetComponent<SpriteRenderer>().color = Color.white;
    }
    public void UnSetPosition(Vector3 position)
    {
        isSet = false;
        this.gameObject.transform.position = position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        onTrigger = true;

        var dragable = collision.gameObject.GetComponent<DragableContainer>();

        if (dragable)
        {
            if (dragable.ContainerName == this.Letter.Symbol.ToString())
            {
                //dragable.SetPosition(this.gameObject.transform.position);
                isMatch = true;
                _setPosition = collision.gameObject.transform.position;
                _container = collision.gameObject;
            }
            else
            {
                isMatch = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        onTrigger = false;
        isMatch = false;
        _setPosition = _originalPosition;
        _container = null;
    }

    public static event Action LetterMatched;

    //public void EnableRandomWalk(bool enable)
    //{
    //    _enabledRandomWalk = enable;

    //    if(enable)
    //        StartCoroutine(walkRandom());
    //}

    //private IEnumerator walkRandom()
    //{
    //    System.Random random = new System.Random();

    //    Vector2 v = new Vector2(random.Next(0, 3), random.Next(0, 3));
    //    if(_rigidbody2D)
    //        _rigidbody2D.velocity = v;

    //    yield return new WaitForSeconds(random.Next(1,4));

    //    if (_enabledRandomWalk)
    //        StartCoroutine(walkRandom());
    //}
}
