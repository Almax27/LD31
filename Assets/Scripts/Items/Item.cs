using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
public class Item : MonoBehaviour {

	public delegate void OnPickedUpDelegate(Item _itemPickedUp);
	public OnPickedUpDelegate onPickedUp;

	public bool destroyOnPickup = true;

	void OnTriggerEnter2D(Collider2D _other)
	{
		if(_other.tag == "Player")
		{
			OnPickedUp(_other);
		}
	}

	//method called when picked up - can be overriden by deriving classes to extend functionality 
	protected virtual void OnPickedUp(Collider2D _other)
	{
		onPickedUp(this);
		if(destroyOnPickup)
		{
			Destroy(this.gameObject);
		}
	}
}
