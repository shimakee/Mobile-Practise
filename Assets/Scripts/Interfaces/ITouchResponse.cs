using UnityEngine;

public interface ITouchResponse
{
    void OnSwipe(GameObject gameObject, Vector2 direction);
    //void OnSwipe(GameObject gameObject, Vector2 startPosition, Vector2 endPosition);
    void OnPinch(float difference);
    //void OnPinch(Touch touchFirst, Touch touchSecond);
    void OnTap(int tapCount, int touchCount, Touch[] touches);
}
