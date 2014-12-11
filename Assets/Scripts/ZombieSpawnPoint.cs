using UnityEngine;
using System.Collections.Generic;

public class ZombieSpawnPoint : MonoBehaviour {

	public ZombieAI zombiePrefab = null;
	public float spawnRate = 3f;
	public int maxSpawnCount = 5;
	public float lastSpawnTime = 0;

	protected List<ZombieAI> activeZombies = new List<ZombieAI>();

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Time.time > lastSpawnTime + spawnRate && activeZombies.Count < maxSpawnCount)
		{
			GameObject gobj = Instantiate(zombiePrefab.gameObject,this.transform.position, this.transform.rotation) as GameObject;
			ZombieAI zombie = gobj.GetComponent<ZombieAI>();
			activeZombies.Add(zombie);
			lastSpawnTime = Time.time;
		}

		for(int i = 0; i < activeZombies.Count; i++)
		{
			ZombieAI zombie = activeZombies[i];
			if(zombie == null)
			{
				activeZombies.RemoveAt(i--);
				continue;
			}
		}
	}
}
