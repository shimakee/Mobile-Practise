using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName ="WordScriptable", menuName = "Create word scriptableObject")]
public class Word : ScriptableObject
{
    public Letter[] letters;
    public AudioClip WordAudio;
    public string WordSpelling;
}
