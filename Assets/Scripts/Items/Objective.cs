using UnityEngine;
using System.Collections;


public class Objective : MonoBehaviour 
{
	public delegate void OnPickedUpDelegate(Objective objective);

	public OnPickedUpDelegate onPickedUp;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnTriggerEnter2D(Collider2D _other)
	{
		if(_other.tag == "Player")
		{
			onPickedUp(this);
			Destroy(this.gameObject);
		}
	}
}
