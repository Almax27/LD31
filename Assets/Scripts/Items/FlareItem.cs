using UnityEngine;
using System.Collections;

public class FlareItem : MonoBehaviour {

	public LightController lightController = null;

	public float minRadius = 5f;
	public float maxRadius = 6f;
	public float lifeTime = 5.0f;
	public float burnOutDuration = 0.5f;

	public ParticleSystem particleSystem;

	float desiredRadius = 1;
	float radiusVel = 0;
	float timeUntilRescale = 0;

	float deathTick = 0;

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

		if(deathTick < lifeTime)
		{
			deathTick += Time.deltaTime;

			if(deathTick >= lifeTime)
			{
				particleSystem.enableEmission = false;
			}
		}
		else
		{
			deathTick += Time.deltaTime;

			float t = Mathf.Min(1, (deathTick - lifeTime) / burnOutDuration);
			desiredRadius = Random.Range(0.5f, 1.0f) * maxRadius * (1-t);
			timeUntilRescale = 1;

			if(t == 1)
			{
				Destroy(this.gameObject);
			}
		}
	}
}
