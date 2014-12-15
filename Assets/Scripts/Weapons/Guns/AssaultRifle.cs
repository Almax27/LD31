using UnityEngine;
using System.Collections;

public class AssaultRifle : Gun {

	public float rateOfFire = 0.05f;
	
	protected float lastFireTime = 0;

	public override bool BeginFire()
	{
		var playerStats = PlayerController.instance.playerStats;
		if(playerStats.ammo <= 0)
		{
			FAFAudio.Instance.PlayOnce2D(noAmmoSound, transform.position, 0.3f);
			return false;
		}
		else
		{
			return base.BeginFire();
		}
	}
	public void Update()
	{
		var playerStats = PlayerController.instance.playerStats;
		if(playerStats.ammo > 0 && isFiring && Time.time > lastFireTime + rateOfFire)
		{
			FireProjectile();
			lastFireTime = Time.time;

			FAFAudio.Instance.PlayOnce2D(attackSound, transform.position, 0.3f);

			playerStats.ammo -= 1;
		}
	}
}
