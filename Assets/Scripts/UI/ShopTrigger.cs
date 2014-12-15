using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class ShopTrigger : MonoBehaviour
{
	public Shop shop = null;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D _other)
	{
		if(shop && _other.tag == "Player")
		{
			shop.OpenShop();
		}
	}

	void OnTriggerExit2D(Collider2D _other)
	{
		if(shop && _other.tag == "Player")
		{
			shop.CloseShop();
		}
	}
}
