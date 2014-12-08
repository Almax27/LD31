using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteRandomiser : MonoBehaviour {

	[System.Serializable]
	public class PossibleState
	{
		public Sprite texture = null;
		public Color color = Color.white;
	};

	public PossibleState[] possibleStates = new PossibleState[0];

	public void RandomiseSprite()
	{
		if(possibleStates.Length > 0)
		{
			PossibleState state = possibleStates[Random.Range(0,possibleStates.Length)];
			SpriteRenderer spriteRenderer = renderer as SpriteRenderer;
			spriteRenderer.sprite = state.texture;
			spriteRenderer.color = state.color;
		}
	}

	// Use this for initialization
	void Start () 
	{
		RandomiseSprite();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
