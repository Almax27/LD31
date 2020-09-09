using UnityEngine;
using System.Collections;

public class LightController : MonoBehaviour 
{
	public Light2D lightPrefab;
	public int lightCount = 3;
	public float lightOffset = 0.2f;

	public float radius = 10;
	public Color color = Color.white;

	protected Light2D[] lights = null;

	// Use this for initialization
	void Awake () 
	{
		Vector2 offset = Vector2.up * lightOffset;
		float angleStep = 360 / lightCount;

		lights = new Light2D[lightCount];
		for(int i = 0; i < lights.Length; i++)
		{
			offset = Quaternion.AngleAxis(angleStep, Vector3.forward) * offset;

			GameObject gobj = Instantiate(lightPrefab.gameObject) as GameObject;
			gobj.transform.parent = this.transform;
			Light2D light2d = gobj.GetComponent<Light2D>();

			light2d.transform.localPosition = offset;

			lights[i] = light2d;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		for(int i = 0; i < lights.Length; i++)
		{
			Light2D light2d = lights[i];

			light2d.radius = radius;
			Color c = color;
			c.a /= lights.Length;
			light2d.GetComponent<Renderer>().material.color = c;
		}
	}
}
