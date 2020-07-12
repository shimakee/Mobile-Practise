using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleSelectionResponse : MonoBehaviour, ISelectionResponse
{
    public void IsSelected(GameObject gameObject, Vector3 inputPosition)
    {
        //check it exists
        if (gameObject)
        {   //transform scale based on input position
            //StartCoroutine(PopScale(gameObject));
            gameObject.transform.localScale = new Vector3(1.2f, 1.2f, 0);
        }
    }

    public void Deselected(GameObject gameObject, Vector3 inputPosition)
    {
        //return to original scale
        if(gameObject != null)
        {
            gameObject.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void OnSelectionConfirm(GameObject gameObject, Vector3 inputPosition)
    {
        //check it exists
        if (gameObject)
        {
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            //maintain target scale;
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                return;

            if (!audioSource.isPlaying)
                audioSource.Play();
        }
    }

    public void OnHoverSelected(GameObject gameObject, Vector3 inputPosition)
    {
        //do nothing
    }

    public void WasSelected(GameObject gameObject, Vector3 inputPosition)
    {
        //do nothing
    }

    public void OnSelectionConfirm(GameObject gameObject, Vector3 inputPosition, List<GameObject> wasSelectedGameObjects)
    {
        //do nothing
    }

    public void IsSelectedUnique(GameObject gameObject, Vector3 inputPosition)
    {
        //do nothing
    }
}
