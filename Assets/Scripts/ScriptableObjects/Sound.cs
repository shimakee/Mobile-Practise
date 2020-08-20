using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
[CreateAssetMenu(fileName ="Sound", menuName = "create scriptable sound")]
public class Sound : ScriptableObject
{
    public string Name;
    public AudioClip AudioClip;
    public bool Loop;
    public bool PlayOnAwake;
    public AudioMixerGroup OutputMixer;
    [Range(0f, 1f)] public float Volume;

    [HideInInspector]
    public AudioSource AudioSource;
}
