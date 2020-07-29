using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] Sounds;

    public static AudioManager instance;

    private void Awake()
    {
        //if (instance == null)
        //{
        //    instance = this;
        //    DontDestroyOnLoad(this);
        //}
        //else
        //{
        //    if(this != instance)
        //    {
        //        Destroy(instance.gameObject);
        //        instance = this;
        //        DontDestroyOnLoad(this.gameObject);
        //        //this.gameObject.SetActive(false);
        //        //Destroy(this.gameObject);
        //        //return;
        //    }
        //}

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
    //public void PlayOneShot(string name)
    //{
    //    Sound sound = Sounds.Where(sound => sound.Name == name).FirstOrDefault();

    //    sound.AudioSource.PlayOneShot(sound.AudioClip);
    //}
}
