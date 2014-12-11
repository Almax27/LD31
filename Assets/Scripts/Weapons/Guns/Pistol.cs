using UnityEngine;
using System.Collections;

public class Pistol : Gun
{
	public float rateOfFire = 0.1f;

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
}
