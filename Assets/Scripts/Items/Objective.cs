using UnityEngine;
using System.Collections;


public class Objective : Item 
{
	public int minWorth = 10;
	public int maxWorth = 20;

	public GameObject pickupTextPrefab;

	protected override void OnPickedUp (Collider2D _other)
	{
		var stats = PlayerController.instance.playerStats;
		int worth = Random.Range(minWorth, maxWorth + 1); //inclusive range
		stats.money += worth;

		GameObject gobj = Instantiate(pickupTextPrefab) as GameObject;
		gobj.transform.position = this.transform.position;

		TextMesh text = gobj.GetComponentInChildren<TextMesh>();
		text.text = "+" + worth.ToString();

		base.OnPickedUp (_other);
	}
}
