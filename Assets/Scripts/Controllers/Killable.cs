using UnityEngine;
using System.Collections;

public class Killable : MonoBehaviour 
{
    public float health = 10;

    public AudioClip deathSound = null;
    public AudioClip damageSound = null;

    public GameObject[] spawnOnDeath = new GameObject[0];

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public virtual void OnDamage(float _damage)
    {
        health -= _damage;
        if (health <= 0)
        {
            OnDeath();
        }
        FAFAudio.Instance.PlayOnce2D(damageSound, transform.position, 0.8f);
    }
    
    public virtual void OnDeath()
    {
        health = 0;
        
        //do some fancy shit here
        FAFAudio.Instance.PlayOnce2D(deathSound, transform.position, 1);

        for (int i = 0; i < spawnOnDeath.Length; i++)
        {
            GameObject gobj = Instantiate(spawnOnDeath[i]) as GameObject;
            gobj.transform.position = this.transform.position;
        }
        
        Destroy(this.gameObject);
    }
}
