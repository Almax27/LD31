using UnityEngine;
using System.Collections;

public class TestFollowMouse : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		p.z = 0;
		this.transform.position = p;
	}
}
