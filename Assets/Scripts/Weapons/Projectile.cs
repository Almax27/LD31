using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Projectile : MonoBehaviour 
{
	public Killable owner = null;

	public int baseDamage = 1;
	public float damageScale = 1;
	public float speed = 5;
	public float knockbackVelocity = 5;

	public bool destroyOnHit = true;
	public float lifeTime = 1;

	public GameObject[] spawnOnDestroy = new GameObject[0];

	public AudioClip soundOnHitKillable = null;
	public AudioClip soundOnHitNonKillable = null;

	void OnCollisionEnter2D(Collision2D collision)
	{
		Killable killable = collision.collider.GetComponent<Killable>();
		if(owner.CanDamage(killable))
		{
			int realDamage = (int)(baseDamage * damageScale);
			Vector2 knockback = (killable.transform.position - this.transform.position).normalized * knockbackVelocity;
			
			DamagePacket damagePacket = new DamagePacket(owner, realDamage, knockback);
			
			killable.BroadcastMessage("OnDamage", damagePacket, SendMessageOptions.DontRequireReceiver);

			FAFAudio.Instance.PlayOnce2D(soundOnHitKillable, this.transform.position, 0.3f);
		}
		else
		{
			FAFAudio.Instance.PlayOnce2D(soundOnHitNonKillable, this.transform.position, 0.3f);
		}
		if(destroyOnHit)
		{
			for(int i = 0; i < spawnOnDestroy.Length; i++)
			{
				GameObject gobj = GameObject.Instantiate(spawnOnDestroy[i]) as GameObject;
				gobj.transform.position = transform.position;
			}
			Destroy(gameObject);
		}
	}
	
	void Update()
	{
		lifeTime -= Time.deltaTime;
		if(lifeTime <= 0)
		{
			Destroy(gameObject);
		}
		rigidbody2D.velocity = transform.up * speed;
	}
}
