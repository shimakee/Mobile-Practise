using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class velocity : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    // Start is called before the first frame update
    private void Awake()
    {
        _rigidbody = transform.GetComponent<Rigidbody2D>();

        if (_rigidbody == null)
            Debug.Log("no rigidbody");

    }
    void Start()
    {
        //tra.GetComponent<Rigidbody2D>().velocity = new Vector2(5, 5);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.touchCount > 0)
        {

            foreach (var touch in Input.touches)
            {

                if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
                {

                    var y = touch.position - _rigidbody.position;

                    if (y.magnitude > 5)
                    {
                        y.Normalize();
                        //y.Scale(new Vector2(5,5));
                    }
                    _rigidbody.velocity = y;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("up");
            _rigidbody.velocity = new Vector2(0, 5);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            _rigidbody.velocity = new Vector2(-5, 0);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            _rigidbody.velocity = new Vector2(0, -5);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            _rigidbody.velocity = new Vector2(5, 0);
        }
    }
}
