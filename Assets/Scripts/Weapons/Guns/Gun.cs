using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {
	
	public Killable owner = null;
	public Projectile bulletPrefab = null;
	public AudioClip attackSound = null;
	public AudioClip noAmmoSound = null;
	public string animationTrigger = "";

	public float bulletSpread;
	public int bulletDamage = 1;

	public bool isFiring = false;

	public virtual bool BeginFire()
	{
		isFiring = true;
		return isFiring;
	}

	public virtual void EndFire()
	{
		isFiring = false;
	}

	protected virtual void FireProjectile()
	{
		Quaternion spead = Quaternion.Euler(0, 0, Random.Range(-bulletSpread, bulletSpread));
		GameObject gobj = Instantiate(bulletPrefab.gameObject, this.transform.position, this.transform.rotation * spead) as GameObject;
		Projectile projectile = gobj.GetComponent<Projectile>();

		projectile.owner = owner;
		projectile.baseDamage = bulletDamage;
		gobj.layer = this.gameObject.layer;
	}
}
