using UnityEngine;

public interface ILetterSelectionResponse : ISelectionResponse
{
    Letter Letter { get; set; }
}