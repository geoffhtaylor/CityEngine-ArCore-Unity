//This script outputs the size of the Collider bounds to the console

using UnityEngine;
using System.Collections;

public class ObtainBounds : MonoBehaviour
{
    float ModelScalingFactor;

    void Start()
    {
        gameObject.AddComponent(typeof(BoxCollider));
        Debug.Log(gameObject.GetComponent<Collider>().bounds.extents.x);
        Debug.Log(gameObject.GetComponent<Collider>().bounds.extents.y);
        Debug.Log(gameObject.GetComponent<Collider>().bounds.extents.z);
        float x = gameObject.GetComponent<Collider>().bounds.extents.x;
        float y = gameObject.GetComponent<Collider>().bounds.extents.y;
        float z = gameObject.GetComponent<Collider>().bounds.extents.z;
        string GOD = x + " " + y + " " + z;
        Debug.Log(GOD);

        // Scales Model object leveraging the ModelScaleFactor user input float value
        float newXScale = (float)(ModelScalingFactor - gameObject.transform.localScale.x);
        float newYScale = (float)(ModelScalingFactor - gameObject.transform.localScale.y);
        float newZScale = (float)(ModelScalingFactor - gameObject.transform.localScale.z);
        gameObject.transform.localScale += new Vector3(newXScale, newYScale, newZScale);
    }
}