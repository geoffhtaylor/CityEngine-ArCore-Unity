using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGlowShader : MonoBehaviour {
    public Renderer rend;
    public Shader myShader;

	// Use this for initialization
	void Start () {
    gameObject.AddComponent(typeof(MeshRenderer));
    rend = GetComponent<Renderer>();
    myShader = Shader.Find("DiffuseOutline");


    }
	
	// Update is called once per frame
	void Update () {
        rend.material.shader = myShader;
    }
}