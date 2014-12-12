using UnityEngine;
using System.Collections.Generic;

public class ObjectiveSpawner : MonoBehaviour {

	[System.Serializable]
	public class ObjectiveToSpawn
	{
		public Objective objectivePrefab = null;
		public float weighting = 1;
	}
	public ObjectiveToSpawn[] objectivesToSpawn = new ObjectiveToSpawn[0];
	public float spawnInterval = 60;

	protected List<Transform> placesToSpawn = new List<Transform>();
	protected List<Transform> availablePlacesToSpawn = new List<Transform>();

	protected float lastSpawnTime = 0;

	// Use this for initialization
	void Start () 
	{
		foreach(Transform child in this.transform)
		{
			placesToSpawn.Add(child);
		}
		availablePlacesToSpawn = new List<Transform>(placesToSpawn);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Time.time > lastSpawnTime + spawnInterval)
		{
			SpawnNewObjective();
			lastSpawnTime = Time.time;
		}
	}

	void SpawnNewObjective()
	{
		List<Transform> spawnPoints = new List<Transform>(placesToSpawn);

		//pick random objective based on weighting
		Objective objectivePrefab = null;
		float totalWeight = 0;
		foreach(ObjectiveToSpawn objToSpawn in objectivesToSpawn)
		{
			totalWeight += objToSpawn.weighting;
		}
		float randomValue = Random.Range(0, totalWeight);
		foreach(ObjectiveToSpawn objToSpawn in objectivesToSpawn)
		{
			if(randomValue < objToSpawn.weighting)
			{
				objectivePrefab = objToSpawn.objectivePrefab;
				break;
			}
		}
		if(objectivePrefab && availablePlacesToSpawn.Count > 0)
		{
			//pick random location and remove it from the available set
			int index = Random.Range(0, availablePlacesToSpawn.Count);
			Transform spawnLocation = availablePlacesToSpawn[index];
			availablePlacesToSpawn.RemoveAt(index);

			//create objective instance
			GameObject gobj = Instantiate(objectivePrefab.gameObject) as GameObject;
			Objective newObjective = gobj.GetComponent<Objective>();
			newObjective.onPickedUp += delegate(Objective objective) {
				if(!availablePlacesToSpawn.Contains(spawnLocation))
				{
					availablePlacesToSpawn.Add(spawnLocation);
				}
			};

			//set objective to the location of spawner
			newObjective.transform.position = spawnLocation.transform.position;
		}
	}
}
