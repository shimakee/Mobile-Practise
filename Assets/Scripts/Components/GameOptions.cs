using System;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="gameOption", menuName = "Create scriptable game option")]
public class GameOptions : ScriptableObject
{
    public ImageAudioOptions ImageAudio = ImageAudioOptions.sfxs;
    public LetterAudioOptions LetterAudio = LetterAudioOptions.phonics;
    public VoicePackage VoicePackage = VoicePackage.default_male;
    public bool Shuffle = false;
    public bool Repeat = false;

    private void Awake()
    {
        Initialize();
    }

    public void Save()
    {
        PlayerPrefs.SetString("audioPackage", this.VoicePackage.ToString());
        PlayerPrefs.SetString("imageAudioOption", this.ImageAudio.ToString());
        PlayerPrefs.SetString("letterAudioOption", this.LetterAudio.ToString());

        PlayerPrefs.SetInt("shuffleOption", Convert.ToInt32(this.Shuffle));
        PlayerPrefs.SetInt("repeatOption", Convert.ToInt32(this.Repeat));
        PlayerPrefs.Save();
    }

    public void Initialize()
    {
        if (PlayerPrefs.HasKey("audioPackage"))
        {
            Enum.TryParse(PlayerPrefs.GetString("audioPackage"), out VoicePackage audioPackage);
            this.VoicePackage = audioPackage;
        }

        if (PlayerPrefs.HasKey("imageAudioOption"))
        {
            Enum.TryParse(PlayerPrefs.GetString("imageAudioOption"), out ImageAudioOptions imageAudioOptions);
            this.ImageAudio = imageAudioOptions;
        }

        if (PlayerPrefs.HasKey("letterAudioOption"))
        {
            Enum.TryParse(PlayerPrefs.GetString("letterAudioOption"), out LetterAudioOptions letterAudioOptions);
            this.LetterAudio = letterAudioOptions;
        }

        if(PlayerPrefs.HasKey("shuffleOption"))
            this.Shuffle = Convert.ToBoolean(PlayerPrefs.GetInt("shuffleOption"));
        if(PlayerPrefs.HasKey("repeatOption"))
            this.Repeat = Convert.ToBoolean(PlayerPrefs.GetInt("repeatOption"));
    }

    public GameOptions GetGameOptions()
    {
        return this;
    }

    public void ToggleShuffle(GameObject gameObject)
    {
        this.Shuffle = !this.Shuffle;

        SetColor(gameObject, Shuffle);
        this.Save();

    }

    public void ToggleRepeat(GameObject gameObject)
    {
        this.Repeat = !this.Repeat;

        SetColor(gameObject, this.Repeat);
        this.Save();
    }

    public void ToggleImageAudio(GameObject gameObject)
    {
        if (this.ImageAudio == ImageAudioOptions.sfxs)
        {
            this.ImageAudio = ImageAudioOptions.words;
        }
        else
        {
            this.ImageAudio = ImageAudioOptions.sfxs;
        }

        SetColor(gameObject, this.ImageAudio == ImageAudioOptions.sfxs);
        this.Save();
    }

    public void ToggleLetterAudio(GameObject gameObject)
    {
        if (this.LetterAudio == LetterAudioOptions.letters)
        {
            this.LetterAudio = LetterAudioOptions.phonics;
        }
        else
        {
            this.LetterAudio = LetterAudioOptions.letters;
        }

        SetColor(gameObject, this.LetterAudio == LetterAudioOptions.letters);
        this.Save();
    }


    public void SetColor(GameObject gameObject, bool isActive)
    {
        Image spriteRenderer = gameObject.GetComponent<Image>();


        if (isActive)
        {
            spriteRenderer.color = new Color32(45, 146, 231, 255);
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }
}
