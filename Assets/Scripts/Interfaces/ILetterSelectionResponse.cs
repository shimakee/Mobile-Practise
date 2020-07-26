using UnityEngine;

public interface ILetterSelectionResponse : ISelectionResponse
{
    Letter Letter { get; set; }
    void Initialize(char letter);
    void ToUpper();
    void ToLower();
}