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

    public void OnSwipe(GameObject gameObject, Vector2 direction)
    {
        throw new System.NotImplementedException();
    }

    public void OnTap(int tapCount, int touchCount, Touch[] touches)
    {
        throw new System.NotImplementedException();
    }


    float GetPinchDifference(Touch firstTouch, Touch secondTouch, float pinchJitterAllowance = 1f)
    {
        firstTouch = Input.GetTouch(0);
        secondTouch = Input.GetTouch(1);

        Vector2 firstTouchPreviousPosition = firstTouch.position - firstTouch.deltaPosition;
        Vector2 secondTouchPreviousPosition = secondTouch.position - secondTouch.deltaPosition;

        float touchDistancePreviousPosition = (firstTouchPreviousPosition - secondTouchPreviousPosition).magnitude;
        float touchDistanceCurrentPosition = (firstTouch.position - secondTouch.position).magnitude;

        float distanceDifferene = touchDistanceCurrentPosition - touchDistancePreviousPosition;

        //pinch allowance is for unexpected finer movement.
        if (distanceDifferene >= 0 - pinchJitterAllowance && distanceDifferene <= 0 + pinchJitterAllowance)
            return 0;

        //difference == 0 = noPinch
        //difference > 0 = pinchOut
        //difference < 0 = pinchIn
        return distanceDifferene;
    }

    Vector2 DetermineSwipeDirection(Vector2 positionStart, Vector2 positionEnd, float swipeJitterAllowance = 1f)
    {
        if (positionStart != null && positionEnd != null)
        {
            Vector2 swipeDirection = positionEnd - positionStart;
            if (swipeDirection.magnitude > swipeJitterAllowance)
                return swipeDirection;
        }

        return new Vector2(0, 0);
    }
}
