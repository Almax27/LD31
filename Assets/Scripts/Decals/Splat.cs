using UnityEngine;
using System.Collections;

public class Splat : MonoBehaviour {

	public float radius = 1;

	public Easing.Helper scaleEase;

	// Use this for initialization
	void Start () 
	{
		Vector2 offset = new Vector2(Random.Range(-1.0f,1.0f), Random.Range(-1.0f,1.0f));
		offset.Normalize();
		this.transform.position = (Vector2)this.transform.position + offset * radius;
	}
	
	// Update is called once per frame
	void Update () 
	{
		float scale = 0;
		if(scaleEase.Update(Time.smoothDeltaTime, 0, 1, out scale))
		{
			this.transform.localScale = new Vector3(scale,scale,scale);
		}
		else
		{
			this.transform.localScale = Vector3.one;
			Destroy(this); //remove splat script
		}
	}
}
