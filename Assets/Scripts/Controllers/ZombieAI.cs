using UnityEngine;
using System.Collections;

public class ZombieAI : MonoBehaviour {

	public NavMeshAgent navAgent = null;
	public CombatCharacter character = null;

	protected Killable target = null;
	protected NavMeshPath path = null;
	protected float pathThresholdSq = 0.5f*0.5f;
	protected int targetCorner = 1;

	// Use this for initialization
	void Start ()
	{
		character = GetComponentInChildren<CombatCharacter>();

		GameObject playerGObj = GameObject.FindWithTag("Player");
		target = playerGObj.GetComponent<CombatCharacter>();

		path = new NavMeshPath();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(target)
		{
			//path.ClearCorners();

			Vector3 thisInNavSpace = ConvertPoint(this.transform.position);
			Vector3 targetInNavSpace = ConvertPoint(target.transform.position);

			if(NavMesh.CalculatePath(thisInNavSpace, targetInNavSpace, -1, path))
			{
				//draw path
				for(int i = 0; i < path.corners.Length-1; i++)
				{
					Vector3 a = path.corners[i];
					Vector3 b = path.corners[i+1];

					Debug.DrawLine(a,b);
				}

				Vector3 targetPosition = ConvertPoint(path.corners[1]);

				Vector3 dir = targetPosition - this.transform.position;
				dir.z = 0;
				dir.Normalize();
				character.TryMove(dir);
				character.TryLook(dir);

				//character.TryMove(
			}

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

	//Vect

}
