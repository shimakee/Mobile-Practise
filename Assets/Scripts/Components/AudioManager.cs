using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] Sounds;
    public GameOptions GameOptions;

    public static AudioManager instance;
    private AudioSource _audioSource;

    private void Awake()
    {
        gameObject.AddComponent<AudioSource>();
        _audioSource = GetComponent<AudioSource>();

        if (instance != null && instance != this)
        {
            this.gameObject.SetActive(false);
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }


        foreach (var s in Sounds)
        {
            s.AudioSource = gameObject.AddComponent<AudioSource>();
            s.AudioSource.clip = s.AudioClip;
            s.AudioSource.loop = s.Loop;
            s.AudioSource.playOnAwake = s.PlayOnAwake;
            s.AudioSource.volume = s.Volume;
            s.AudioSource.outputAudioMixerGroup = s.OutputMixer;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Play("Title theme");
    }

    public void Play(string name)
    {
        Sound sound = Sounds.Where(s => s.Name == name).FirstOrDefault();
        if (!sound)
        {
            Debug.Log("No sound available", this);
            return;
        }

        sound.AudioSource.Play();
    }

    //play voice method

    //play oneshot method for none interuptable clips
    public void PlayOneShot(string name)
    {
        Sound sound = Sounds.Where(s => s.Name == name).FirstOrDefault();

        sound.AudioSource.PlayOneShot(sound.AudioClip);
    }

    public void PlayVoice(string name)
    {
        AudioClip resourceWord = null;
        int length = name.Length;
        if (length <= 1)
            return;
        if (length == 2)
        {

            //vv
            if(IsVowel(name[0]) && IsVowel(name[1]))
                resourceWord = Resources.Load<AudioClip>($"Packages/{GameOptions.VoicePackage}/audio/syllables/vv/{name}");
            //vc
            if (IsVowel(name[0]) && !IsVowel(name[1]))
                resourceWord = Resources.Load<AudioClip>($"Packages/{GameOptions.VoicePackage}/audio/syllables/vc/{name}");
            //cv
            if (!IsVowel(name[0]) && IsVowel(name[1]))
                resourceWord = Resources.Load<AudioClip>($"Packages/{GameOptions.VoicePackage}/audio/syllables/cv/{name}");
            //cc
            if (!IsVowel(name[0]) && !IsVowel(name[1]))
                resourceWord = Resources.Load<AudioClip>($"Packages/{GameOptions.VoicePackage}/audio/syllables/vv/{name}");
        }
        else
        {
            resourceWord = Resources.Load<AudioClip>($"Packages/{GameOptions.VoicePackage}/audio/words/{name}");
        }

        if (resourceWord && _audioSource)
        {
            _audioSource.clip = resourceWord;
            _audioSource.Play();
        }
    }

    private bool IsVowel(char c)
    {
        Debug.Log($"lowercase {char.ToLower(c)}");
        Debug.Log($"normal {c}");
        Debug.Log($"uppercase {char.ToUpper(c)}");
        Debug.Log($"normal {c}");

        char character = char.ToLower(c);
        if (character == 'a' || character == 'e' || character == 'i' || character == 'o' || character == 'u')
            return true;

        return false;
    }

}
