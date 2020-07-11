using System.Collections;
using UnityEngine;

public class ScaleSelectionResponse : DefaultSelectionResponse, ISelectionResponse
{
    public override void IsSelected(GameObject gameObject, Vector3 inputPosition)
    {
        //check it exists
        if (gameObject)
        {   //transform scale based on input position
            //StartCoroutine(PopScale(gameObject));
            gameObject.transform.localScale = new Vector3(1.2f, 1.2f, 0);
        }
    }


    public override void Deselected(GameObject gameObject, Vector3 inputPosition)
    {
        //return to original scale
        if(gameObject != null)
        {
            gameObject.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public override void OnSelectionConfirm(GameObject gameObject, Vector3 inputPosition)
    {
        //check it exists
        if (gameObject)
        {   //maintain target scale;
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                return;

            if (!audioSource.isPlaying)
                audioSource.Play();
        }
    }

    IEnumerator PopScale(GameObject gameObject)
    {

        Transform transform = gameObject.transform;
        transform.localScale = new Vector3(1.5f, 1.5f, 0);
        yield return new WaitForSeconds(.2f);
        transform.localScale = new Vector3(1.2f, 1.2f, 0);

    } 
}
