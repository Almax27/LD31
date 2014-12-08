using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Projectile : MonoBehaviour 
{
	public Killable owner = null; //TODO: change to team alignment

	public int baseDamage = 1;
	public float damageScale = 1;
	public float speed = 5;
	public float knockbackVelocity = 5;

	public bool destroyOnHit = true;
	public float lifeTime = 1;

	public GameObject[] spawnOnDestroy = new GameObject[0];

	void OnTriggerEnter2D(Collider2D _other)
	{
		Killable killable = _other.GetComponent<Killable>();
		if(owner.CanDamage(killable))
		{
			int realDamage = (int)(baseDamage * damageScale);
			Vector2 knockback = (killable.transform.position - this.transform.position).normalized * knockbackVelocity;
			
			DamagePacket damagePacket = new DamagePacket(owner, realDamage, knockback);
			
			killable.BroadcastMessage("OnDamage", damagePacket, SendMessageOptions.DontRequireReceiver);

			if(destroyOnHit)
			{
				Destroy(gameObject);
			}
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

	void OnDestroy()
	{
		for(int i = 0; i < spawnOnDestroy.Length; i++)
		{
			GameObject gobj = GameObject.Instantiate(spawnOnDestroy[i]) as GameObject;
			gobj.transform.position = transform.position;
		}
	}

}
