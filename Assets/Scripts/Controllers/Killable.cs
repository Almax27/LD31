using UnityEngine;
using System.Collections;

public class Killable : MonoBehaviour 
{
	#region public variables

    public int health = 10;

    public AudioClip deathSound = null;
    public AudioClip damageSound = null;

	public GameObject[] spawnOnSpawn = new GameObject[0];
    public GameObject[] spawnOnDeath = new GameObject[0];
	public GameObject[] spawnOnHit = new GameObject[0];

	public GameObject damageTextPrefab = null;

	#endregion

	#region protected variables

	protected CombatCharacter lastAggressor = null;

	#endregion

	#region public methods
	
	public virtual void OnDamage(DamagePacket _damagePacket)
	{
		health -= _damagePacket.damageAmount;
		if (health <= 0)
		{
			OnDeath();
		}

		FAFAudio.Instance.PlayOnce2D(damageSound, this.transform.position, 0.8f);
		
		for (int i = 0; i < spawnOnHit.Length; i++)
		{
			GameObject gobj = Instantiate(spawnOnHit[i]) as GameObject;
			gobj.transform.position = this.transform.position;
		}

		if(damageTextPrefab)
		{
			GameObject text = Instantiate(damageTextPrefab) as GameObject;
			text.transform.position = this.transform.position;
			text.GetComponentInChildren<TextMesh>().text = _damagePacket.damageAmount.ToString();
		}
		
		//Debug.Log(name + " took " + _damagePacket.damageAmount + " damage");
	}
	
	public virtual void OnDeath()
	{
		health = 0;
		
		//do some fancy shit here
		FAFAudio.Instance.PlayOnce2D(deathSound, this.transform.position, 1);
		
		for (int i = 0; i < spawnOnDeath.Length; i++)
		{
			GameObject gobj = Instantiate(spawnOnDeath[i]) as GameObject;
			gobj.transform.position = this.transform.position;
		}
		
		Destroy(this.gameObject);
	}

	public virtual bool CanDamage(Killable _killable)
	{
		return _killable && _killable != this;
	}
	
	#endregion

	#region monobehaviuor methods

	// Use this for initialization
	void Start () 
	{
		for (int i = 0; i < spawnOnSpawn.Length; i++)
		{
			GameObject gobj = Instantiate(spawnOnSpawn[i]) as GameObject;
			gobj.transform.position = this.transform.position;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#endregion
	
}
