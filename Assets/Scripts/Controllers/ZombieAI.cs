using UnityEngine;
using System.Collections.Generic;

public class ZombieAI : MonoBehaviour {
	
	public CombatCharacter character = null;
	public MeleeAttack meleeAttack = null;
	public float attackDistance = 1.0f;

	public string[] targetTags = new string[0];
	public float targetSearchRadius = 20;
	public float maxTargetPathDistance = 20;
	public float targetUpdateRate = 1.0f;

	public float chasingSpeed = 2;
	public float wanderingSpeed = 1;

	protected Transform target = null;
	protected NavMeshPath path = null;
	protected float pathThresholdSq = 0.5f*0.5f;
	protected int targetCorner = 1;
	protected float lastTargetUpdateTime = 0;

	protected bool isWandering = false;
	protected float nextWanderTime = 0;
	protected Vector3 wanderTargetPosition = Vector3.zero;

	protected Vector3 lastSeenPosition = Vector3.zero;
	protected bool chasingTarget = false;

	// Use this for initialization
	void Start ()
	{
		character = GetComponentInChildren<CombatCharacter>();

		path = new NavMeshPath();

		lastTargetUpdateTime = Time.time + Random.value;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Time.timeScale == 0)
		{
			return;
		}

		TryUpdateTarget();

		Vector2 targetPosition;
		if(target)
		{
			character.movementSpeed = chasingSpeed;
			lastSeenPosition = target.position;
			targetPosition = lastSeenPosition;
			chasingTarget = true;
		}
		else if(chasingTarget)
		{
			float dist = (this.transform.position - lastSeenPosition).sqrMagnitude;
			if(dist > 0.5f)
			{
				targetPosition = lastSeenPosition;
			}
			else
			{
				chasingTarget = false;
				character.TryMove(Vector2.zero);
				return;
			}
		}
		else
		{
			character.movementSpeed = wanderingSpeed;
			if(Time.time > nextWanderTime)
			{
				isWandering = Random.value > 0.5f;
				wanderTargetPosition = this.transform.position + Random.insideUnitSphere * 5;
				nextWanderTime = Time.time + Random.Range(3.0f,5.0f);
			}
			if(isWandering)
			{
				targetPosition = wanderTargetPosition;
			}
			else
			{
				character.TryMove(Vector2.zero);
				return;
			}
		}

		Vector3 thisInNavSpace = ConvertPoint(this.transform.position);
		Vector3 targetInNavSpace = ConvertPoint(targetPosition);
		float distanceSq = (targetInNavSpace - thisInNavSpace).sqrMagnitude;
		//path to target position
		if(distanceSq > 0.5f && NavMesh.CalculatePath(thisInNavSpace, targetInNavSpace, -1, path))
		{
			//draw path
			for(int i = 0; i < path.corners.Length-1; i++)
			{
				Vector3 a = path.corners[i];
				Vector3 b = path.corners[i+1];
				
				Debug.DrawLine(a,b);
			}

			if(path.corners.Length > 1)
			{
				Vector3 pos = ConvertPoint(path.corners[1]);
				Vector3 dir = pos - this.transform.position;
				dir.z = 0;
				dir.Normalize();
				character.TryMove(dir);
				character.TryLook(dir);
			}
			else
			{
				character.TryMove(Vector2.zero);
			}
		}
		else
		{
			character.TryMove(Vector2.zero);
		}

		//try attack
		if(target)
		{
			Vector2 dirToTarget = targetPosition - (Vector2)this.transform.position;
			float angleToTarget = Vector2.Angle(character.lookBody.rotation * Vector2.up, dirToTarget);
			if(meleeAttack && dirToTarget.sqrMagnitude < attackDistance * attackDistance && angleToTarget < 45)
			{
				meleeAttack.owner = character;
				meleeAttack.ApplyDamage();
			}
		}
	}

	void TryUpdateTarget()
	{
		int worldLayerMask = 1 << LayerMask.NameToLayer("World");

		if(Time.time > lastTargetUpdateTime + targetUpdateRate)
		{
			List<GameObject> targets = new List<GameObject>();

			GameObject desiredTarget = null;

			foreach(string tag in targetTags)
			{
				targets.AddRange(GameObject.FindGameObjectsWithTag(tag));

				float closestLengthSq = float.MaxValue;
				GameObject closestTarget = null;
				
				for(int i = 0; i < targets.Count; i++)
				{
					GameObject nearbyTarget = targets[i];

					//do line of sight check
					Vector2 dir = (nearbyTarget.transform.position - this.transform.position);
					float angle = Vector2.Angle(this.character.LookDirection, dir);

					if(angle > 80)
					{
						continue;
					}
					else if(dir.sqrMagnitude > targetSearchRadius*targetSearchRadius)
					{
						continue;
					}
					else if(Physics2D.Raycast(this.transform.position, dir.normalized, dir.magnitude, worldLayerMask))
					{
						continue;
					}

					//do pathing distance check
					Vector3 thisInNavSpace = ConvertPoint(this.transform.position);
					Vector3 targetInNavSpace = ConvertPoint(nearbyTarget.transform.position);
					
					if(NavMesh.CalculatePath(thisInNavSpace, targetInNavSpace, -1, path))
					{
						float lenSq = 0;
						for(int j = 0; j < path.corners.Length-1; j++)
						{
							lenSq += (path.corners[j] - path.corners[j+1]).sqrMagnitude;
						}
						if(lenSq < closestLengthSq)
						{
							closestLengthSq = lenSq;
							closestTarget = nearbyTarget;
						}
					}
				}

				if(closestTarget != null && closestLengthSq < maxTargetPathDistance * maxTargetPathDistance)
				{
					desiredTarget = closestTarget;
					break;
				}
			}

			if(desiredTarget)
			{
				target = desiredTarget.transform;
			}
			else
			{
				target = null;
			}

			lastTargetUpdateTime = Time.time;
		}
	}

	Vector3 ConvertPoint(Vector3 p)
	{
		Vector3 newP;
		newP.x = p.x;
		newP.y = p.z;
		newP.z = p.y;
		return newP;
	}

	Vector3 ClosestPoint(Vector3 p1, Vector3[] points, float minDistSq)
	{
		Vector3 closestPoint = p1;
		float closestDistSq = float.MaxValue;
		
		for(int i = 0; i < points.Length; i++)
		{
			Vector3 p2 = points[i];
			float distSq = (p1 - p2).sqrMagnitude;
			if(distSq < closestDistSq && distSq > minDistSq)
			{
				closestDistSq = distSq;
				closestPoint = p2;
			}
		}
		return closestPoint;
	}

	public void OnDamage (DamagePacket _damagePacket)
	{
		target = _damagePacket.sender.transform;
		lastTargetUpdateTime = Time.time;
	}

	//Vect

}
