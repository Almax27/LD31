using UnityEngine;
using System.Collections;

public class Shotgun : Gun {

	public float rateOfFire = 0.5f;
	public int bulletsPerShot = 5;

	protected float lastFireTime = 0;
	
	public override bool BeginFire()
	{
		var playerStats = PlayerController.instance.playerStats;
		if(playerStats.ammo <= 0)
		{
			FAFAudio.Instance.PlayOnce2D(noAmmoSound, transform.position, 0.3f);
			return false;
		}
		else if(Time.time > lastFireTime + rateOfFire)
		{
			base.BeginFire();

			int shotsToFire = Mathf.Min(bulletsPerShot, playerStats.ammo);
			playerStats.ammo -= shotsToFire;

			FireBurst(shotsToFire);
			lastFireTime = Time.time;

			FAFAudio.Instance.PlayOnce2D(attackSound, transform.position, 0.3f);

			return true;
		}
		return false;
	}

	protected void FireBurst(int _projectileCount)
	{
		for(int i = 0; i < _projectileCount; i++)
		{
			FireProjectile();
		}
	}
}
