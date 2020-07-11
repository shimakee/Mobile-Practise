using UnityEngine;

public interface ITouchResponse
{
    void OnSwipe(Vector2 direction);
    void OnPinch(float difference);
    void OnTap(int tapCount, int touchCount, Touch[] touches);
}
