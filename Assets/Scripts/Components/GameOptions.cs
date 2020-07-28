using System;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="gameOption", menuName = "Create scriptable game option")]
public class GameOptions : ScriptableObject
{
    //volume
    public float MasterAudioVolume = 0;
    public float VoiceAudioVolume = 0;
    public float MusicAudioVolume = 0;
    public float SfxAudioVolume = 0;
    //audio options
    public ImageAudioOptions ImageAudio = ImageAudioOptions.sfxs;
    public LetterAudioOptions LetterAudio = LetterAudioOptions.phonics;
    public VoicePackage VoicePackage = VoicePackage.default_male;
    public LetterCasingOptions LetterCasingOptions = LetterCasingOptions.lower;
    //display options
    public bool Shuffle = false;
    public bool Repeat = false;
    //casing changed event
    public event Action CasingChanged;

    

    private void Awake()
    {
        Initialize();
    }

    public void Save()
    {
        //volume
        PlayerPrefs.SetFloat("masterVolume", this.MasterAudioVolume);
        PlayerPrefs.SetFloat("voiceVolume", this.VoiceAudioVolume);
        PlayerPrefs.SetFloat("musicVolume", this.MusicAudioVolume);
        PlayerPrefs.SetFloat("sfxVolume", this.SfxAudioVolume);
        //audio options
        PlayerPrefs.SetString("audioPackage", this.VoicePackage.ToString());
        PlayerPrefs.SetString("imageAudioOption", this.ImageAudio.ToString());
        PlayerPrefs.SetString("letterAudioOption", this.LetterAudio.ToString());
        //display options
        PlayerPrefs.SetInt("shuffleOption", Convert.ToInt32(this.Shuffle));
        PlayerPrefs.SetInt("repeatOption", Convert.ToInt32(this.Repeat));
        PlayerPrefs.Save();

        OnCasingChanged();
    }

    public void Initialize()
    {
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            this.MasterAudioVolume = PlayerPrefs.GetFloat("masterVolume");
        }
        if (PlayerPrefs.HasKey("voiceVolume"))
        {
            this.VoiceAudioVolume = PlayerPrefs.GetFloat("voiceVolume");
        }
        if (PlayerPrefs.HasKey("voiceVolume"))
        {
            this.MusicAudioVolume = PlayerPrefs.GetFloat("musicVolume");
        }
        if (PlayerPrefs.HasKey("voiceVolume"))
        {
            this.SfxAudioVolume = PlayerPrefs.GetFloat("sfxVolume");
        }
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

    private void OnCasingChanged()
    {
        CasingChanged?.Invoke();
    }
}
