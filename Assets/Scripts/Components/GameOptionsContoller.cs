using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameOptionsContoller : MonoBehaviour
{
    public AudioMixer MasterMixer;
    public AudioMixer VoiceMixer;
    public AudioMixer MusicMixer;
    public AudioMixer SfxMixer;

    public Slider[] Sliders;

    public GameOptions GameOptions;

    public TMP_Dropdown LetterCasingDropDown;

    public GameObject ShuffleToggle;
    public GameObject RepeatToggle;
    public GameObject ImageAudioToggle;
    public GameObject LetterAudioToggle;
    public GameObject CasingToggle;

    //colors
    private Color32 _activeColor = new Color32(45, 146, 231, 255);
    //private Color32 _alternateColor = new Color32(77, 221, 74, 255);
    private Color32 _alternateColor = new Color32(162, 247, 154, 255);
    private Color32 _inActiveColor = Color.white;

    private void Awake()
    {
        this.SetActiveColor(ShuffleToggle, GameOptions.Shuffle);
        this.SetActiveColor(RepeatToggle, GameOptions.Repeat);
        this.SetActiveColor(ImageAudioToggle, GameOptions.ImageAudio == ImageAudioOptions.sfxs);
        this.SetActiveColor(LetterAudioToggle, GameOptions.LetterAudio == LetterAudioOptions.letters);
        this.ChangeUIOnCasingColor(CasingToggle);
    }

    // Start is called before the first frame update
    void Start()
    {
        LetterCasingDropDown.value = (int)GameOptions.LetterCasingOptions;
        SetSliderValues();
        PopulateDropDown<LetterCasingOptions>(LetterCasingDropDown);
    }

    private void SetSliderValues()
    {
        if(Sliders.Length == 4)
        {
            Sliders[0].value = GameOptions.MasterAudioVolume;
            Sliders[1].value = GameOptions.VoiceAudioVolume;
            Sliders[2].value = GameOptions.MusicAudioVolume;
            Sliders[3].value = GameOptions.SfxAudioVolume;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ActivateOptionsCanvas(GameObject optionsCanvas)
    {
        optionsCanvas.SetActive(true);
    }

    public void DeActivateOptionsCanvas(GameObject optionsCanvas)
    {
        optionsCanvas.SetActive(false);
    }

    public void SaveOptions()
    {
        GameOptions.Save();
    }
    public void SetVolumeMaster(float volume)
    {
        MasterMixer.SetFloat("volume", Mathf.Log10(volume)  * 20);
        GameOptions.MasterAudioVolume = volume;
    }
    public void SetVolumeVoice(float volume)
    {
        VoiceMixer.SetFloat("volume", Mathf.Log10(volume) * 20);
        GameOptions.VoiceAudioVolume = volume;

    }
    public void SetVolumeMusic(float volume)
    {
        MusicMixer.SetFloat("volume", Mathf.Log10(volume) * 20);
        GameOptions.MusicAudioVolume = volume;
    }
    public void SetVolumeSfx(float volume)
    {
        SfxMixer.SetFloat("volume", Mathf.Log10(volume) * 20);
        GameOptions.SfxAudioVolume = volume;
    }

    public void LetterCasingDropdownChanged(int index)
    {
        GameOptions.LetterCasingOptions = (LetterCasingOptions)index;

        Debug.Log($"options changed to {GameOptions.LetterCasingOptions}");
        GameOptions.Save();
    }

    public void ToggleShuffle(GameObject gameObject)
    {
        GameOptions.Shuffle = !GameOptions.Shuffle;

        SetActiveColor(gameObject, GameOptions.Shuffle);
        GameOptions.Save();
    }

    public void ToggleRepeat(GameObject gameObject)
    {
        GameOptions.Repeat = !GameOptions.Repeat;

        SetActiveColor(gameObject, GameOptions.Repeat);
        GameOptions.Save();
    }

    public void ToggleImageAudio(GameObject gameObject)
    {
        if (GameOptions.ImageAudio == ImageAudioOptions.sfxs)
        {
            GameOptions.ImageAudio = ImageAudioOptions.words;
        }
        else
        {
            GameOptions.ImageAudio = ImageAudioOptions.sfxs;
        }

        SetActiveColor(gameObject, GameOptions.ImageAudio == ImageAudioOptions.sfxs);
        GameOptions.Save();
    }

    public void ToggleLetterAudio(GameObject gameObject)
    {
        if (GameOptions.LetterAudio == LetterAudioOptions.letters)
        {
            GameOptions.LetterAudio = LetterAudioOptions.phonics;
        }
        else
        {
            GameOptions.LetterAudio = LetterAudioOptions.letters;
        }

        SetActiveColor(gameObject, GameOptions.LetterAudio == LetterAudioOptions.letters);
        GameOptions.Save();
    }

    public void ToggleLetterCasing(GameObject gameObject)
    {
        int enumMaxIndex = Enum.GetNames(typeof(LetterCasingOptions)).Length - 1;

        if ((int)GameOptions.LetterCasingOptions > enumMaxIndex - 1)
        {
            GameOptions.LetterCasingOptions = 0;
        }
        else
        {
            GameOptions.LetterCasingOptions++;
        }

        ChangeUIOnCasingColor(gameObject);
        GameOptions.Save();
    }

    public void SetActiveColor(GameObject gameObject, bool isActive)
    {
        Image spriteRenderer = gameObject.GetComponent<Image>();


        if (isActive)
        {
            spriteRenderer.color = _alternateColor;
        }
        else
        {
            spriteRenderer.color = _inActiveColor;
        }
    }

    public void ChangeUIOnCasingColor(GameObject gameObject)
    {
        Image imageRenderer = gameObject.GetComponent<Image>();

        if (GameOptions.LetterCasingOptions == LetterCasingOptions.lower)
            imageRenderer.color = _alternateColor;
        if (GameOptions.LetterCasingOptions == LetterCasingOptions.upper)
            imageRenderer.color = _activeColor;
        if (GameOptions.LetterCasingOptions == LetterCasingOptions.standard)
            imageRenderer.color = _inActiveColor;
    }

    private void PopulateDropDown<T>(TMP_Dropdown dropdown) where T : Enum 
    {
        
        List<string> names = Enum.GetNames(typeof(T)).ToList();
        //uppercase first letter
        for (int i = 0; i < names.Count; i++)
        {
            char[] temp = names[i].ToCharArray();
            temp[0] = Char.ToUpper(temp[0]);
            names[i] = new string(temp);
        }
        if(dropdown)
            dropdown.AddOptions(names);
    }
}
