using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] Sounds;

    private void Awake()
    {
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
