using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragSnapSelectionResponse : MonoBehaviour, IPictureSelectionResponse
{
    public Picture PictureScriptable;
    public Matching MatchingSession;
    public Picture Picture
    {
        get { return PictureScriptable; }
        set { PictureScriptable = value; }
    }

    public GameOptions GameOptions;

    private AudioSource _audioSource;
    private SpriteRenderer _spriteRenderer;
    private AudioManager _audioManager;
    private Rigidbody2D _rigidbody2D;

    private Vector3 _originalScale;
    private Vector3 _originalPosition;
    private Vector2 _targetPosition;
    private Vector2 _setPosition;
    //public string namers;

    public Vector3 OriginalPosition { get { return _originalPosition; } set { _originalPosition = value; } }

    private bool isSelected = false;

    private GameObject _container;

    public bool isSet = false;
    public bool onTrigger = false;
    public bool isMatch = false;

    //public static event Action<string> WordMatched;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _audioManager = FindObjectOfType<AudioManager>();

        if (!_audioSource)
            throw new NullReferenceException("no audio source comppnent attached.");
        if (!_spriteRenderer)
            throw new NullReferenceException("no sprite renderer comppnent attached.");
        if (!GameOptions)
            throw new NullReferenceException("no options.");
    }
    void Start()
    {
        _originalScale = this.gameObject.transform.localScale;
        _originalPosition = this.gameObject.transform.position;
        //this.GetComponent<SpriteRenderer>().color = Color.grey;
        //Matching.WordMatched += OnWordMatched;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitializePicture(string picture) //create overload for offset and margins?
    {
        if (Picture == null)
            Picture = ScriptableObject.CreateInstance<Picture>();
        if (String.IsNullOrWhiteSpace(Picture.Name))
            Picture.Name = picture;
        //assign assets
        if (Picture.Sfx == null)
            Picture.Sfx = Resources.Load<AudioClip>($"Audio/Sfxs/sfx_{picture}");
        if (Picture.Sprite == null)
            Picture.Sprite = Resources.Load<Sprite>($"Sprites/default/{picture}");
        if (Picture.WordAudio == null)
            Picture.WordAudio = Resources.Load<AudioClip>($"Packages/{GameOptions.VoicePackage}/audio/words/{picture}");

        //name your object
        transform.name = this.Picture.Name;

        //check that all resources are there
        _spriteRenderer.sprite = Picture.Sprite;
        //_audioSource.clip = Picture.Sfx;
        _audioSource.clip = Picture.WordAudio;
    }


    public void Deselected(GameObject gameObject, Vector3 inputPosition)
    {
        isSelected = false;
        if (!isSet )
        {
            if (onTrigger)
            {
                if (isMatch)
                {
                    //this.transform.position = _setPosition;
                    //this.isSet = true;
                    SetPosition(_setPosition);

                    if (_container)
                        _container.SetActive(false);

                    _audioManager.Play("Confirm");

                    //OnWordMatched(this.Picture.Name);
                    //Matching.OnWordMatched(this.Picture.Name);
                    if(MatchingSession != null)
                    {
                        MatchingSession.WordMatched(this.Picture.Name);
                    }
                    
                }
                else
                {
                    this.transform.position = _originalPosition;
                    _audioManager.Play("Back");
                }
            }
            else
            {
                this.transform.position = _originalPosition;
            }
        }

        this.gameObject.transform.localScale = _originalScale;

    }

    public void IsSelected(GameObject gameObject, Vector3 inputPosition)
    {
        if (!isSet)
        {
            if (_rigidbody2D)
            {
                Debug.Log("rigidbody");
                _rigidbody2D.velocity = new Vector2(0, 0);

                _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            }

            _targetPosition = Camera.main.ScreenToWorldPoint(inputPosition);
            gameObject.transform.position = _targetPosition;
            this.GetComponent<SpriteRenderer>().color = Color.white;
        }

        var scale = (float)(_originalScale.x * 1.3);
        this.gameObject.transform.localScale = new Vector3(scale, scale, scale);

        //if (!_audioSource.isPlaying && !isSelected)
        //{
        //    _audioSource.Play();
        //}

        isSelected = true;
    }

    public void IsSelectedUnique(GameObject gameObject, Vector3 inputPosition)
    {
        //gameObject.transform.position = new Vector3(inputPosition.x, inputPosition.y, 0);
    }

    public void OnHoverSelected(GameObject gameObject, Vector3 inputPosition)
    {
    }

    public void OnSelectionConfirm(GameObject gameObject, Vector3 inputPosition, List<GameObject> wasSelectedGameObjects)
    {
    }

    public void OnSelectionConfirm(GameObject gameObject, Vector3 inputPosition)
    {
        //gameObject.transform.position = inputPosition;
    }

    public void WasSelected(GameObject gameObject, Vector3 inputPosition)
    {
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
            if (dragable.ContainerName == this.Picture.Name)
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
        //var dragable = collision.gameObject.GetComponent<DragableContainer>();

        //if (dragable)
        //{
        //    if (dragable.ContainerName == this.Picture.Name)
        //    {
        //        //dragable.SetPosition(this.gameObject.transform.position);
        //        isContactOnSet = false;
                
        //    }
        //}
    }

    //private void OnWordMatched(string word)
    //{
    //    if(word == this.Picture.Name)
    //        Debug.Log(word);
    //}

    public void PlayWordAudio()
    {
        //if (!_audioSource.isPlaying)
        //{
            _audioSource.Play();
        //}
    }
}
