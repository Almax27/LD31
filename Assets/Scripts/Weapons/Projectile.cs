using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Projectile : MonoBehaviour 
{
    public float damage = 1;
    public float flightSpeed = 1;

    public Transform decalPrefab;

    #region monobehaviour methods

	// Use this for initialization
	void Start () 
    {
        rigidbody2D.velocity = transform.up * flightSpeed;
    }

    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D _collision)
    {
        _collision.gameObject.SendMessage("OnDamage", damage, SendMessageOptions.DontRequireReceiver);

        GameObject gobj = Instantiate(decalPrefab.gameObject) as GameObject;
        gobj.transform.position = _collision.contacts[0].point;

        Destroy(this.gameObject);
    }

    #endregion

}
