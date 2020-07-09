using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName ="WordScriptable", menuName = "Create word scriptableObject")]
public class Word : ScriptableObject
{
    public List<Letter> Letters = new List<Letter>();
    public Sprite Sprite;
    public AudioClip WordAudio;
    public AudioClip Sfx; // sfx of the word when they tap on the sprite picture.
    public string WordSpelling;


    //public Word(string word)
    //{
    //    WordSpelling = word;
    //}

    //public Word(string word, AudioClip audio)
    //    :this(word)
    //{
    //    WordAudio = audio;
    //}

    //public Word(string word, AudioClip audio, List<Letter> letterArray)
    //    :this(word, audio)
    //{
    //    Letters = letterArray;
    //}
}
