using System;
using UnityEngine;

[CreateAssetMenu(fileName ="LetterScriptable", menuName = "Create letter scriptableObject")]
public class Letter : ScriptableObject
{
    //keep the letters as default since there will only be 26 of them
    //you can just pull difference sprites, audio on resource.load
    public char Symbol;
    public Sprite Sprite;
    //change audio file to only one audio clip, change it on Letter controller
    //using awake method and reasource.load letter or phonic audio depending on user options.
    public AudioClip PhonicAudio;
    public AudioClip LetterAudio;
    public LetterType LetterType;

}

public enum LetterType
{
    vowel,
    consonant
}