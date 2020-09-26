using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragableContainer : MonoBehaviour
{

    public string ContainerName;

    private void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
        if (string.IsNullOrWhiteSpace(ContainerName))
            Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
