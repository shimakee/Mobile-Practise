using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWalker : MonoBehaviour
{

    private Rigidbody2D _rigidbody2D;
    private bool _enabledRandomWalk;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableRandomWalk(bool enable)
    {
        _enabledRandomWalk = enable;

        if (enable)
        {
            StartCoroutine(walkRandom());
        }
    }

    private IEnumerator walkRandom()
    {
        System.Random random = new System.Random();
        float sec = 0;

        if (_rigidbody2D)
        {
            Vector2 v = new Vector2(random.Next(2, 3), random.Next(2, 3));

            _rigidbody2D.velocity = v;
        }

        sec = (float)(random.NextDouble() * 7);
        yield return new WaitForSeconds(sec);

        for (int i = 0; i < random.Next(1,4); i++)
        {
            if (_rigidbody2D)
            {
                Vector2 v = new Vector2(random.Next(2, 3), random.Next(2, 3));

                _rigidbody2D.velocity = v;
            }

            sec = (float)(random.NextDouble() * 5);
            yield return new WaitForSeconds(sec);
        }

        if (_rigidbody2D)
            _rigidbody2D.velocity = new Vector2(0, 0);

        sec = (float)((random.NextDouble() * 2) +2);
        yield return new WaitForSeconds(sec);

        if (_enabledRandomWalk)
        {
            StartCoroutine(walkRandom());
        }
    }
}
