using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUD : MonoBehaviour {

	public GameObject[] healthBars = new GameObject[0];

	public Text moneyText = null;
	public Text flareText = null;
	public Text ammoText = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		var playerStats = PlayerController.instance.playerStats;

		for(int i = 0; i < healthBars.Length; i++)
		{
			healthBars[i].SetActive(i < PlayerController.instance.character.health);
		}
		moneyText.text = playerStats.money.ToString();

		flareText.text = playerStats.flares.ToString();
		flareText.color = playerStats.flares <= 0 ? Color.red : Color.white;

		ammoText.text = playerStats.ammo.ToString();
		ammoText.color = playerStats.ammo <= 0 ? Color.red : Color.white;
	}
}
