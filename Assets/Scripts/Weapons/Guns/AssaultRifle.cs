using UnityEngine;
using System.Collections;

public class AssaultRifle : Gun {

	public float rateOfFire = 0.05f;
	
	protected float lastFireTime = 0;

	public void Update()
	{
		if(isFiring && Time.time > lastFireTime + rateOfFire)
		{
			FireProjectile();
			lastFireTime = Time.time;

			FAFAudio.Instance.PlayOnce2D(attackSound, transform.position, 0.3f);
		}

	}
}
