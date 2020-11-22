using UnityEngine;

public interface IPictureSelectionResponse : ISelectionResponse
{
    Picture Picture { get; set; }
    void InitializePicture(string picture);
    void PlayWordAudio();
}