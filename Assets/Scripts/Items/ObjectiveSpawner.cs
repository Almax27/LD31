using UnityEngine;
using System.Collections.Generic;

public class ObjectiveSpawner : ItemSpawner {

	public int objectivesActiveAtOnce = 2;
	protected int activeCount = 0;

	protected override void Start ()
	{
		base.Start ();

		SpawnObjective();
	}

	protected void SpawnObjective()
	{
		while(activeCount < objectivesActiveAtOnce)
		{
			Item newItem = SpawnNewItem();
			if(newItem)
			{
				activeCount++;
				newItem.onPickedUp += delegate(Item _itemPickedUp) 
				{
					activeCount--;
					SpawnObjective();
				};
			}
			else
			{
				Debug.LogError("Failed to create new objective");
				break;
			}
		}
	}


}
