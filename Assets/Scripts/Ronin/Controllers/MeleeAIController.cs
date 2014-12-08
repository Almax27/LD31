using UnityEngine;
using System.Collections;

public class MeleeAIController : MonoBehaviour {

	#region public types

	public enum State
	{
		Idle,
		Attacking,
		Fleeing
	}

	#endregion

	#region public variables

	public CombatCharacter entity;

	public State currentState = State.Idle;
	
	public float attackRate = 0.5f;
	public float attackDistance = 0.5f;
	public float agressiveness = 0.4f;

	public float fleeHealth = 1;
	public float fleeDistance = 2f;

	public bool CanAttack { get { return Time.time > lastAttackTime + attackRate; } }

	#endregion

	#region private variables

	float stateTick = 0;
	float lastAttackTime = 0;

	Killable target;

	float idleLookTick = 0;

	#endregion

	void SetState(State newState)
	{
		currentState = newState;
		stateTick = 0;
	}

	void OnAttack()
	{
		lastAttackTime = Time.time;
		if(stateTick > 1.0f)
		{
			SetState(State.Idle);
		}
	}

	void OnDamage(DamagePacket _damagePacket)
	{
		if (entity.health <= fleeHealth)
			SetState (State.Fleeing);
	}

	// Use this for initialization
	void Start () 
	{
		entity = GetComponentInChildren<CombatCharacter>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(target == null)
			SetState(State.Idle);

		stateTick += Time.deltaTime;

		switch(currentState)
		{
		case State.Idle:
		{
			//pick closest target
			float enemyDistance;
			entity.GetClosestEnemy(out target, out enemyDistance);

			//choose something to do
			if(target)
			{
				Debug.Log("RUnning Away");
				Vector3 delta = this.entity.transform.position - target.transform.position;
				if(delta.magnitude < 2)
				{
					entity.TryMove(delta);
					entity.TryLook(delta);
				}
				return;
			}
			else if(Random.value < 0.1f)
			{
				if(target && Random.value <= agressiveness)
					SetState(State.Attacking);
			}

			idleLookTick -= Time.deltaTime;
			if(idleLookTick < 0)
			{
				entity.TryLook(new Vector2(Random.Range(-1,1),Random.Range(-1,1)));
				idleLookTick = 0.5f + Random.value*1.5f;
			}

			entity.TryMove(Vector2.zero);

			break;
		}
		case State.Attacking:
		{
			if(target == null || stateTick > 1.0f)
			{
				SetState(State.Idle);
			}

			if(CanAttack)
			{
				Vector3 delta = target.transform.position - this.entity.transform.position;
				entity.TryMove(delta);
				entity.TryLook(delta);
				if(Vector3.Distance(entity.transform.position, target.transform.position) < attackDistance)
				{
					entity.TryMeleeAttack();
                    lastAttackTime = Time.time;
				}
			}

			break;
		}
		case State.Fleeing:
		{
			Vector3 delta = this.entity.transform.position - target.transform.position;
			if(delta.magnitude < fleeDistance)
			{
				entity.TryMove(delta);
				entity.TryLook(delta);
			}
			else
			{
				SetState(State.Idle);
			}

			break;
		}
		}
	}
}
