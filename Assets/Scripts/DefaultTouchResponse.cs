using UnityEngine;

public class DefaultTouchResponse : MonoBehaviour, ITouchResponse
{
    public void OnPinch(float difference)
    {
        throw new System.NotImplementedException();
    }

    public void OnSwipe(Vector2 direction)
    {
        throw new System.NotImplementedException();
    }

    public void OnTap(int tapCount, int touchCount, Touch[] touches)
    {
        throw new System.NotImplementedException();
    }
}
