using UnityEngine;
using System.Collections.Generic;
using System;

public class Light2D: MonoBehaviour 
{
	public float radius = 100;
	public Mesh lightMesh = null;
	protected List<Vector3> lightMeshPoints = new List<Vector3>();

	// Use this for initialization
	void Start () 
	{
		lightMesh = new Mesh();
		MeshFilter filter = GetComponent<MeshFilter>();
		filter.mesh = lightMesh;
	}

	void boundPointToArea(ref Vector2 point)
	{
		point.x = Mathf.Clamp(point.x, -radius, radius);
		point.y = Mathf.Clamp(point.y, -radius, radius);
	}
	float distanceToBound(Vector2 dirFromCenter)
	{
		float ratio = dirFromCenter.x / dirFromCenter.y;
		//pick smallest and move to bounds
		if(Mathf.Abs(dirFromCenter.x) < Mathf.Abs(dirFromCenter.y))
		{
			dirFromCenter.x = radius;
			dirFromCenter.y = dirFromCenter.x * ratio;
		}
		else
		{
			dirFromCenter.y = radius;
			dirFromCenter.x = dirFromCenter.y / ratio;
		}
		return dirFromCenter.magnitude;
	}

	void TryAddRayCast(Vector2 dir, float distance)
	{		
		RaycastHit2D hit = Physics2D.Raycast(this.transform.position, dir, distance);
		
		if(hit.collider != null)
		{
			lightMeshPoints.Add(hit.point - (Vector2)this.transform.position);
		}
		else
		{
			lightMeshPoints.Add(dir * distance);
		}
	}

	void ProcessPoint(Vector2 point)
	{
		Vector2 relativePosition = point - (Vector2)this.transform.position;
		boundPointToArea(ref relativePosition);

		Vector2 dir = relativePosition.normalized;

		TryAddRayCast(dir, relativePosition.magnitude);

		Vector2 dirOff = Quaternion.AngleAxis(Mathf.Rad2Deg * 0.00001f, Vector3.forward) * dir;
		TryAddRayCast(dirOff, distanceToBound(dirOff));

		dirOff = Quaternion.AngleAxis(Mathf.Rad2Deg * -0.00001f, Vector3.forward) * dir;
		TryAddRayCast(dirOff, distanceToBound(dirOff));
	}

	delegate void ProcessColliderMethod(Collider2D x);

	void ProcessPolygon(Collider2D collider)
	{
		PolygonCollider2D polygon = (PolygonCollider2D)collider;
		for(int j = 0; j < polygon.points.Length; j++)
		{
			this.ProcessPoint(polygon.transform.position + (Vector3)polygon.points[j]);
		}
	}

	void ProcessCircle(Collider2D collider)
	{
		CircleCollider2D circle = (CircleCollider2D)collider;

		Vector2 toCircle = circle.transform.position - this.transform.position;

		float theta = Mathf.Sin (circle.radius / toCircle.magnitude);
		float DistToTangent = Mathf.Sqrt(circle.radius*radius - toCircle.sqrMagnitude);
		Vector2 tangentNormal = (Quaternion.AngleAxis(Mathf.Rad2Deg * theta, Vector3.forward) * toCircle.normalized) * DistToTangent;

		this.ProcessPoint((Vector2)circle.transform.position + tangentNormal);
		this.ProcessPoint((Vector2)circle.transform.position - tangentNormal);

		Debug.DrawLine(this.transform.position, (Vector2)circle.transform.position + tangentNormal);
		Debug.DrawLine(this.transform.position, (Vector2)circle.transform.position - tangentNormal);
		Debug.DrawLine(this.transform.position, (Vector2)circle.transform.position);
	}

	// Update is called once per frame
	void Update () 
	{
		lightMeshPoints.Clear();

		Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(this.transform.position, radius, LayerMask.NameToLayer("Lighting2DColliders"));

		Dictionary<Type, ProcessColliderMethod> procDict = new Dictionary<Type, ProcessColliderMethod>();
		procDict[typeof(PolygonCollider2D)] = (c) => ProcessPolygon(c);
		//procDict[typeof(CircleCollider2D)] = (c) => ProcessCircle(c);

		//build points
		for(int i = 0; i < collidersInRange.Length; i++)
		{
			Collider2D collider = collidersInRange[i];
			procDict[collider.GetType()](collider);
		}

		Vector2[] corners = {new Vector2(-radius,-radius),new Vector2(-radius,radius),new Vector2(radius,radius),new Vector2(radius,-radius)};
		for(int i = 0; i < corners.Length; i++)
		{
			Vector2 relativePosition = corners[i];
			Vector2 dir = relativePosition.normalized;
			float dist = relativePosition.magnitude;
			
			TryAddRayCast(dir, dist);
		}

		//sort by angle from vertical axis
		lightMeshPoints.Sort(delegate(Vector3 a, Vector3 b)
		{
			float angleA = Vector3.Angle(Vector3.up, a);
			if(a.x < 0)
			{
				angleA = 360 - angleA;
			}
			float angleB = Vector3.Angle(Vector3.up, b);
			if(b.x < 0)
			{
				angleB = 360 - angleB;
			}
			return angleA.CompareTo(angleB);
		});

		lightMeshPoints.Add(Vector2.zero);

		BuildLightMesh();

		this.renderer.material.SetFloat("_Radius", radius);
	}

	void BuildLightMesh()
	{
		int vertexCount = lightMeshPoints.Count;

		Vector3[] newVertices = lightMeshPoints.ToArray();

		int triangleCount = vertexCount-1;
		int[] newTriangles = new int[triangleCount * 3];

		int t = 0;
		for(int i = 0; i < newTriangles.Length; i += 3)
		{
			newTriangles[i] = vertexCount-1;
			newTriangles[i+1] = t;
			newTriangles[i+2] = t+1;
			t++;
		}
		//correct final index
		newTriangles[newTriangles.Length-1] = 0;
		
		Vector2[] newUVs = new Vector2[vertexCount];
		for(int i=0; i<vertexCount; i++)
		{
			newUVs[i] = newVertices[i]/(radius*2);
		}

		lightMesh.Clear();
		lightMesh.vertices = newVertices;
		lightMesh.triangles = newTriangles;
		lightMesh.uv = newUVs;

		lightMesh.RecalculateNormals();
		lightMesh.RecalculateBounds();
	}
}
