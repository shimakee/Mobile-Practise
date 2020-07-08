using System;
using UnityEngine;

[CreateAssetMenu(fileName ="LetterScriptable", menuName = "Create letter scriptableObject")]
public class Letter : ScriptableObject
{
    public char Symbol;
    public Sprite Sprite;
    public AudioClip PhonicAudio;
    public AudioClip LetterAudio;
    public LetterType LetterType;

}

public enum LetterType
{
    vowel,
    consonant
}