using UnityEngine;
using System.Collections.Generic;

public class CombatCharacter : Character 
{
	#region static variables

	public static List<CombatCharacter> activeCombatEntities = new List<CombatCharacter>();

	#endregion

	#region public variables

	public float knockbackScale = 1.0f;

	#endregion

	#region protected variables

	protected float damageScale = 1;

	#endregion

	#region private variables
	 
	float knockBackTime = 0;
	Vector3 knockBackVelocity = Vector3.zero;

	#endregion

	#region public methods

	public override void OnDamage (DamagePacket _damagePacket)
	{
		base.OnDamage (_damagePacket);

		knockBackTime = 0.07f;
		knockBackVelocity = _damagePacket.knockback * knockbackScale;
	}

	public virtual bool GetClosestEnemy(out Killable _target, out float _distance)
	{
		_target = null;
		_distance = 0;

		//if an aggressor is within flee radius, move to a random position
		float minDist = float.MaxValue;
		foreach(CombatCharacter aggressor in CombatCharacter.activeCombatEntities)
		{
			if(aggressor != this)
			{
				float dist = Vector2.Distance(this.transform.position, aggressor.transform.position);
				if(dist < minDist)
				{
					_distance = dist;
					_target = aggressor;
				}
			}
		}

		return _target != null;
	}
	
	#endregion

	#region monobehaviour methods

	protected void OnEnable()
	{
		activeCombatEntities.Add (this);
	}

	protected void OnDisable()
	{
		activeCombatEntities.Remove (this);
	}

	protected override void Update ()
	{
		if(knockBackTime < 0)
			base.Update ();
		else
		{
			GetComponent<Rigidbody2D>().velocity = knockBackVelocity;
			knockBackTime -= Time.deltaTime;
		}
	}

	#endregion


}
