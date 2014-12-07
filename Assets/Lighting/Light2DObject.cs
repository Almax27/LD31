using UnityEngine;
using System.Collections.Generic;

public class Light2DObject : MonoBehaviour {

	//static Light2DObject
	// Use this for initialization
	void Start () 
	{
		renderer.material.SetFloat("_AmbientLight", 0);
	}

	// Update is called once per frame
	void Update () 
	{
	
	}
}
