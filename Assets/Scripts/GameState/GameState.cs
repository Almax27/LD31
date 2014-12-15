using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour 
{
	public static GameState Instance
	{
		get
		{
			if(instance == null)
			{
				GameObject gobj = new GameObject("GameState", typeof(GameState));
				instance = gobj.GetComponent<GameState>();
			}
			return instance;
		}
	}
	static GameState instance = null;

	//GameState variables

}
