using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName ="WordScriptable", menuName = "Create word scriptableObject")]
public class Word : ScriptableObject
{
    public Letter[] letters;
    public Sprite Sprite;
    public AudioClip WordAudio;
    public string WordSpelling;
}
