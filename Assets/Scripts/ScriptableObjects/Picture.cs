using UnityEngine;

[CreateAssetMenu(fileName = "PictureScriptable", menuName = "Create picture scriptableObject")]
public class Picture : ScriptableObject
{
    public string Name;
    public AudioClip Sfx;
    public AudioClip WordAudio;
    public Sprite Sprite;
}
