using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameOptionsContoller : MonoBehaviour
{
    public GameOptions GameOptions;

    public GameObject ShuffleToggle;
    public GameObject RepeatToggle;
    public GameObject ImageAudioToggle;
    public GameObject LetterAudioToggle;
    public GameObject CasingToggle;

    private void Awake()
    {
        GameOptions.SetActiveColor(ShuffleToggle, GameOptions.Shuffle);
        GameOptions.SetActiveColor(RepeatToggle, GameOptions.Repeat);
        GameOptions.SetActiveColor(ImageAudioToggle, GameOptions.ImageAudio == ImageAudioOptions.sfxs);
        GameOptions.SetActiveColor(LetterAudioToggle, GameOptions.LetterAudio == LetterAudioOptions.letters);
        GameOptions.ChangeUIOnCasingColor(CasingToggle);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}
