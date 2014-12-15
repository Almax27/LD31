using UnityEngine;
using System.Collections;

public class PickupSpawner : ItemSpawner 
{
	public float minSpawnInterval = 0;
	public int maxSpawnCount = 1;

	protected float lastSpawnTime = 0;
	protected int activePickupCount = 0;
	
	// Update is called once per frame
	void Update () 
	{
		if(Time.time > lastSpawnTime + minSpawnInterval)
		{
			if(activePickupCount < maxSpawnCount)
			{
				Item item = SpawnNewItem();
				item.onPickedUp += delegate(Item _itemPickedUp) 
				{
					activePickupCount--;
				};
				activePickupCount++;
			}
			lastSpawnTime = Time.time;
		}
	}
}
