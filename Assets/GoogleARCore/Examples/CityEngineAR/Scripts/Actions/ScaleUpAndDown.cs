using UnityEngine;
 
 public class ScaleUpAndDown : MonoBehaviour
{

    void Update()
    {
        Vector3 vec = new Vector3(0, Mathf.Sin(Time.time * 2), 0);
        transform.localScale += vec;

    }
}
