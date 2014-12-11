using UnityEngine;
using System.Collections;

public class Killable : MonoBehaviour 
{
	#region public variables
	[System.Serializable]
	public class ObjectToSpawn
	{
		public GameObject prefab = null;
		public int count = 1;
		public bool clonePosition = true;
		public bool cloneRotation = true;
		public bool cloneScale = false;
	}

    public int health = 10;

    public AudioClip deathSound = null;
    public AudioClip damageSound = null;

	public ObjectToSpawn[] spawnOnSpawn = new ObjectToSpawn[0];
	public ObjectToSpawn[] spawnOnDeath = new ObjectToSpawn[0];
	public ObjectToSpawn[] spawnOnHit = new ObjectToSpawn[0];

	public GameObject damageTextPrefab = null;

	#endregion

	#region protected variables

	protected CombatCharacter lastAggressor = null;

	#endregion

	#region public methods

	public virtual void SpawnObject(ObjectToSpawn spawnItem, Quaternion rotation)
	{
		for(int i = 0; i < spawnItem.count; i++)
		{
			GameObject gobj = Instantiate(spawnItem.prefab) as GameObject;
			if(spawnItem.clonePosition)
				gobj.transform.position = this.transform.position;
			if(spawnItem.cloneRotation)
				gobj.transform.rotation = rotation;
			if(spawnItem.cloneScale)
				gobj.transform.localScale = this.transform.localScale;
		}
	}
	
	public virtual void OnDamage(DamagePacket _damagePacket)
	{
		health -= _damagePacket.damageAmount;
		if (health <= 0)
		{
			OnDeath();
		}

		FAFAudio.Instance.PlayOnce2D(damageSound, this.transform.position, 0.8f);

		Vector2 normal = this.transform.position - _damagePacket.sender.transform.position;
		Vector2 up = Vector2.up;
		float dz = normal.x * up.y - normal.y * up.x;
		float angle = Mathf.Atan2(Mathf.Abs(dz) + float.Epsilon, Vector2.Dot(normal, up));

		Quaternion normalRot = Quaternion.AngleAxis(Mathf.Rad2Deg * -angle, Vector3.forward);
		
		for (int i = 0; i < spawnOnHit.Length; i++)
		{
			SpawnObject(spawnOnHit[i], normalRot);
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
			SpawnObject(spawnOnDeath[i], GetRotationForObjectSpawn());
		}
		
		Destroy(this.gameObject);
	}

	public virtual bool CanDamage(Killable _killable)
	{
		return _killable && _killable != this;
	}

	public virtual Quaternion GetRotationForObjectSpawn()
	{
		return this.transform.rotation;
	}
	
	#endregion

	#region monobehaviuor methods

	// Use this for initialization
	void Start () 
	{
		for (int i = 0; i < spawnOnSpawn.Length; i++)
		{
			SpawnObject(spawnOnSpawn[i], GetRotationForObjectSpawn());
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#endregion
	
}
