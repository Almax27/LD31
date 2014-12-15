using UnityEngine;
using System.Collections;

public class HelpOverlay : MonoBehaviour {

	public GameObject imageGobj = null;

	// Use this for initialization
	void Start () 
	{
		if(imageGobj)
		{
			imageGobj.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Time.timeScale > 0 && Input.GetKeyDown(KeyCode.H) && imageGobj)
		{
			imageGobj.SetActive(!imageGobj.activeSelf);
			if(imageGobj.activeSelf)
			{
				Time.timeScale = 0;
			}
			else
			{
				Time.timeScale = 1;
			}
		}
	}

	void onMusicVolumeChanged(float _value)
	{
	}

	void onEffectsVolumeChanged(float _value)
	{
	} 
}
