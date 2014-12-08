using UnityEngine;
using System.Collections;

public class FlareItem : MonoBehaviour {

	public LightController lightController = null;

	public float minRadius = 5f;
	public float maxRadius = 6f;

	float desiredRadius = 1;
	float radiusVel = 0;
	float timeUntilRescale = 0;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		lightController.radius = Mathf.SmoothDamp(lightController.radius, desiredRadius, ref radiusVel, 0.1f);

		timeUntilRescale -= Time.smoothDeltaTime;
		if(timeUntilRescale <= 0)
		{
			desiredRadius = Random.Range(minRadius,maxRadius);
			timeUntilRescale = Random.Range(0.1f,0.3f);
		}
	}
}
