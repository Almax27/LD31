using UnityEngine;
using System.Collections.Generic;
using System;

public class Light2D: MonoBehaviour 
{
	public float radius = 100;
	public Mesh lightMesh = null;

	public class LightMeshPoint : IComparable<LightMeshPoint>
	{
		public LightMeshPoint(Vector2 _point)
		{
			point = _point;
			angle = Mathf.Atan2(point.y, point.x);
		}
		public Vector2 point;
		public float angle;

		public int CompareTo(LightMeshPoint comparePart)
		{
			if(this.angle == comparePart.angle) return 0;
			return this.angle < comparePart.angle ? 1 : -1;
		}
	}
	protected List<LightMeshPoint> lightMeshPoints = new List<LightMeshPoint>(1000);
	protected LayerMask lightingMask;

	// Use this for initialization
	void Start () 
	{
		lightMesh = new Mesh();
		MeshFilter filter = GetComponent<MeshFilter>();
		filter.mesh = lightMesh;

		lightingMask = 1 << LayerMask.NameToLayer("Lighting2DColliders");
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
	
	void TryAddRayCast(ref Vector2 dir, float distance)
	{		
		Vector2 pos2D = this.transform.position;
		RaycastHit2D raycastHit = Physics2D.Raycast(pos2D, dir, distance, lightingMask.value);

		if(raycastHit.collider != null)
		{
			lightMeshPoints.Add(new LightMeshPoint(raycastHit.point - pos2D));
		}
		else
		{
			lightMeshPoints.Add(new LightMeshPoint(dir * distance));
		}
	}

	Quaternion rayRotLeft = Quaternion.AngleAxis(Mathf.Rad2Deg * 0.00001f, Vector3.forward);
	Quaternion rayRotRight = Quaternion.AngleAxis(Mathf.Rad2Deg * -0.00001f, Vector3.forward);
	void ProcessPoint(Vector2 point)
	{
		Vector2 relativePosition = point - (Vector2)this.transform.position;
		boundPointToArea(ref relativePosition);

		Vector2 dir = relativePosition.normalized;

		TryAddRayCast(ref dir, relativePosition.magnitude);

		Vector2 dirOff = rayRotLeft * dir;
		TryAddRayCast(ref dirOff, distanceToBound(dirOff));

		dirOff = rayRotRight * dir;
		TryAddRayCast(ref dirOff, distanceToBound(dirOff));
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

		Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(this.transform.position, radius, lightingMask.value);

		//Dictionary<Type, ProcessColliderMethod> procDict = new Dictionary<Type, ProcessColliderMethod>();
		//procDict[typeof(PolygonCollider2D)] = (c) => ProcessPolygon(c);
		//procDict[typeof(CircleCollider2D)] = (c) => ProcessCircle(c);

		//build points
		for(int i = 0; i < collidersInRange.Length; i++)
		{
			Collider2D collider = collidersInRange[i];
			ProcessPolygon(collider);
			//procDict[collider.GetType()](collider);
		}

		Vector2[] corners = {new Vector2(-radius,-radius),new Vector2(-radius,radius),new Vector2(radius,radius),new Vector2(radius,-radius)};
		for(int i = 0; i < corners.Length; i++)
		{
			Vector2 relativePosition = corners[i];
			Vector2 dir = relativePosition.normalized;
			float dist = relativePosition.magnitude;
			
			TryAddRayCast(ref dir, dist);
		}

		lightMeshPoints.Sort();

		lightMeshPoints.Add(new LightMeshPoint(Vector2.zero));

		BuildLightMesh();

		this.renderer.material.SetFloat("_Radius", radius);
	}

	void BuildLightMesh()
	{
		int vertexCount = lightMeshPoints.Count;

		Vector3[] newVertices = new Vector3[vertexCount];
		Vector2[] newUVs = new Vector2[vertexCount];
		for(int i=0; i< vertexCount; i++)
		{
			Vector3 v = lightMeshPoints[i].point;
			newVertices[i] = v;
			newUVs[i] = v/(radius*2);
		}

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

		lightMesh.Clear();
		lightMesh.vertices = newVertices;
		lightMesh.triangles = newTriangles;
		lightMesh.uv = newUVs;

		lightMesh.RecalculateNormals();
		lightMesh.RecalculateBounds();
	}
}
