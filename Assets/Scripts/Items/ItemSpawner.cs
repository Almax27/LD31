using UnityEngine;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour {

	[System.Serializable]
	public class ItemsToSpawn
	{
		public Item prefab = null;
		public float weighting = 1;
	}

	public ItemsToSpawn[] itemsToSpawn = new ItemsToSpawn[0];

	protected List<Transform> placesToSpawn = new List<Transform>();
	protected List<Transform> availablePlacesToSpawn = new List<Transform>();

	// Use this for initialization
	virtual protected void Start () 
	{
		foreach(Transform child in this.transform)
		{
			placesToSpawn.Add(child);
		}
		availablePlacesToSpawn = new List<Transform>(placesToSpawn);
	}

	public Item SpawnNewItem()
	{
		List<Transform> spawnPoints = new List<Transform>(placesToSpawn);
		
		//pick random objective based on weighting
		Item itemPrefab = null;
		float totalWeight = 0;
		foreach(ItemsToSpawn itemToSpawn in itemsToSpawn)
		{
			totalWeight += itemToSpawn.weighting;
		}
		float randomValue = Random.Range(0, totalWeight);
		foreach(ItemsToSpawn itemToSpawn in itemsToSpawn)
		{
			if(randomValue < itemToSpawn.weighting)
			{
				itemPrefab = itemToSpawn.prefab;
				break;
			}
		}
		if(itemPrefab && availablePlacesToSpawn.Count > 0)
		{
			//pick random location and remove it from the available set
			int index = Random.Range(0, availablePlacesToSpawn.Count);
			Transform spawnLocation = availablePlacesToSpawn[index];
			availablePlacesToSpawn.RemoveAt(index);
			
			//create objective instance
			GameObject gobj = Instantiate(itemPrefab.gameObject) as GameObject;
			Item item = gobj.GetComponent<Item>();

			//add callback for when pickedup so we can add the spawn location back to the available set
			item.onPickedUp += delegate(Item itemPickedUp) {
				if(!availablePlacesToSpawn.Contains(spawnLocation))
				{
					availablePlacesToSpawn.Add(spawnLocation);
				}
			};
			
			//set item to the location of spawner
			item.transform.position = spawnLocation.transform.position;

			return item;
		}

		return null;
	}

}
