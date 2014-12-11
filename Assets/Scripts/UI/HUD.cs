using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour {

	public PlayerController playerController = null;

	public GameObject[] healthBars = new GameObject[0];

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		int playerHealth = playerController.character.health;
		for(int i = 0; i < healthBars.Length; i++)
		{
			healthBars[i].SetActive(i < playerHealth);
		}

	}
}
