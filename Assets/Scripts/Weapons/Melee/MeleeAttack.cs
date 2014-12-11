using UnityEngine;
using System.Collections.Generic;

public class MeleeAttack : MonoBehaviour 
{
	#region public variables

	public Killable owner = null; //TODO: change to team alignment
	public string animationTrigger = "";
	public AudioClip soundOnAttack = null;

	public int baseDamage = 1;
	public float damageScale = 1;
	public float knockbackSpeed = 10;

	public float duration = 0.1f;

	public float rateOfUse = 0.1f;
	
	protected float lastUseTime = 0;

	#endregion

	#region private variables

	List<Killable> processed = new List<Killable>();
	float tick = 0;

	#endregion

	#region public methods

	public bool ApplyDamage()
	{
		if(Time.time > lastUseTime + rateOfUse)
		{
			processed.Clear();
			
			this.collider2D.enabled = true;
			tick = 0;

			lastUseTime = Time.time;

			return true;
		}

		return false;
	}

	#endregion

	#region monobehaviour methods

	void Awake()
	{
		if(this.collider2D == null)
		{
			Debug.LogError("Missing Collider2D on AreaDamage");
			enabled = false;
		}
		this.collider2D.enabled = false;
	}

	void Update()
	{
		tick += Time.deltaTime;
		if(tick > duration)
		{
			this.collider2D.enabled = false;
		}
	}

	void OnTriggerEnter2D(Collider2D _other)
	{
		Killable killable = _other.GetComponent<Killable>();
		if(owner.CanDamage(killable) && !processed.Contains(killable))
		{
			if(killable.gameObject.layer != this.gameObject.layer)
			{
				int realDamage = (int)(baseDamage * damageScale);
				Vector2 knockback = (killable.transform.position - transform.position).normalized * knockbackSpeed;

				DamagePacket damagePacket = new DamagePacket(owner, realDamage, knockback);

				killable.BroadcastMessage("OnDamage", damagePacket, SendMessageOptions.DontRequireReceiver);

				processed.Add(killable);

				FAFAudio.Instance.PlayOnce2D(soundOnAttack, this.transform.position, 0.3f);
			}
		}
	}

	#endregion
}
