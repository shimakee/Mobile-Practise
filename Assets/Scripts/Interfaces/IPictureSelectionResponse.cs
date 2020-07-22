using UnityEngine;

public interface IPictureSelectionResponse : ISelectionResponse
{
    Picture Picture { get; set; }
    void InitializePicure(string picture);
}