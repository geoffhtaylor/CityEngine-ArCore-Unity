using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformObj : MonoBehaviour
/*
{
    void Start()
    {
        // Initializations here...
    }

    void Update()
    {
        if 
        // warn: the scale will grow constantly with no end
        transform.localScale += new Vector3(0, 0.1f, 0);
    }
}
*/

{
    float speed = 6f;
    float depth = 1f;

    void Update()
    {
        float scaleInOut = Mathf.Sin(Time.time * speed);
        transform.localScale = new Vector3(1, scaleInOut, 1) * depth;
    }
}

