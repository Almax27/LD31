using UnityEngine;
using System.Collections;

public class Shotgun : Gun {

	public float rateOfFire = 0.5f;
	public int bulletsPerShot = 5;

	protected float lastFireTime = 0;
	
	public override bool BeginFire()
	{
		if(Time.time > lastFireTime + rateOfFire)
		{
			FireProjectile();
			lastFireTime = Time.time;
			base.BeginFire();

			FAFAudio.Instance.PlayOnce2D(attackSound, transform.position, 0.3f);

			return true;
		}
		return false;
	}

	protected override void FireProjectile()
	{
		for(int i = 0; i < bulletsPerShot; i++)
		{
			base.FireProjectile();
		}
	}
}
