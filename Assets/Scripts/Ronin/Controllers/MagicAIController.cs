using UnityEngine;
using System.Collections;

public class MagicAIController : MonoBehaviour
{
	#region public types

	public enum State
	{
		Idle,
		Attacking,
		Moving
	}

	#endregion

	#region public variables

	public CombatCharacter entity;
	public Vector3 targetPosition;
	public float targetThreshold = 0.15f;

	public float fleeCooldown = 1.0f;
	public float fleeRadius = 0.5f;
	public float distanceToFlee = 1.0f;

	public int consecutiveAttacks = 2;
	public float attackDelay = 0.5f;
	public float attackInterval = 0.5f;

	public float agressiveness = 0.6f;
	public float curiousness = 0.4f;

	#endregion

	#region private variables

	State currentState = State.Idle;
	float stateTick = 0;

	int attackCount = 0;

	Killable targetToAttack = null;

	float lastFleeTime = 0;


	#endregion

	#region private methods

	void SetState(State _newState)
	{
		currentState = _newState;
		stateTick = 0;
	}

	void OnDamage(DamagePacket _damagePacket)
	{
		//rethink on damage
		SetState (State.Idle);
	}

	#endregion

	#region monobehaviour methods
	
	// Use this for initialization
	void Start () 
	{
		//set target as current position to begin
		targetPosition = transform.position; 
	}
	
	// Update is called once per frame
	void Update () 
	{
		stateTick += Time.deltaTime;

		switch(currentState)
		{
		case State.Idle:
		{
			if(Time.time > lastFleeTime + fleeCooldown)
			{
				Killable enemy = null;
				float enemyDistance = 0;
				if(entity.GetClosestEnemy(out enemy, out enemyDistance))
				{
					if(enemyDistance < fleeRadius)
					{
						targetPosition = this.transform.position + (this.transform.position - enemy.transform.position).normalized * fleeRadius;
						SetState(State.Moving);

						lastFleeTime = Time.time;
						return;
					}
				}
			}

			float decision = Random.value*2.0f;
			if(decision < agressiveness)
			{
				SetState(State.Attacking);
			}
			else if (decision >= 1 && decision < curiousness + 1)
			{
				//targetPosition = RoomHelper.RandomPosition();
				SetState(State.Moving);
			}

			entity.TryMove(Vector2.zero);
			entity.TryLook(new Vector2(Random.Range(-1,1),Random.Range(-1,1)));

			break;
		}
		case State.Attacking:
		{
			//try to find target
			if(targetToAttack == null)
			{
				float enemyDistance;
				entity.GetClosestEnemy(out targetToAttack, out enemyDistance);
				attackCount = 0;
			}
			else
			{
				//check if we've finished attacking
				if(attackCount >= consecutiveAttacks)
				{
					targetToAttack = null;
					//targetPosition = RoomHelper.RandomPosition();
					SetState(State.Moving);
					break;
				}
				//try to attack
				else if(stateTick > attackDelay + (attackCount*attackInterval))
				{
					attackCount++;
					entity.TryCastMagic();
				}
                entity.TryTurn(targetToAttack.transform.position - this.transform.position);
				entity.TryLook(targetToAttack.transform.position - this.transform.position);
			}

			entity.TryMove(Vector2.zero);

			break;
		}
		case State.Moving:
		{
			Vector2 distance = targetPosition - entity.transform.position;
			if(distance.magnitude < targetThreshold)
			{
				SetState(State.Idle);
			}
			else
			{
				entity.TryMove(distance);
				entity.TryLook(distance);
			}

			//timeout if blocked
			if(stateTick > 5.0f)
			{
				SetState(State.Idle);
			}
			break;
		}
		}
	}

	#endregion
}
