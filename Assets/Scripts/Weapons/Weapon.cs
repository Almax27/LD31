using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour 
{

    #region public variables

    public GameObject attackPrefab = null;
    public AudioClip attackSound = null;

    public Transform spawnNode = null; //defaults to self if null

    public float fireDelay = 1; //shots per second
    public float fireTimeOffset = 0; 

    public float fireSpreadDeg = 0;

    #endregion

    #region protected variables

    protected bool isFiring = false;
    protected float fireTick = 0;

	protected bool canFire = true;

    #endregion

	#region public methods

	public virtual void TryUseWeapon()
	{
		OnFire();
	}

	#endregion

    #region protected methods

    protected virtual void OnFire()
    {
        Quaternion spead = Quaternion.Euler(0, 0, Random.Range(-fireSpreadDeg, fireSpreadDeg));
		GameObject gobj = Instantiate(attackPrefab, spawnNode.position, spawnNode.rotation * spead) as GameObject;

		FAFAudio.Instance.PlayOnce2D(attackSound, transform.position, 0.3f);
    }

    #endregion

    #region Monobehaviour methods

    void Awake()
    {
		if(spawnNode != null)
		{
        	spawnNode = transform;
		}
    }

    void Update()
    {
        fireTick += Time.deltaTime;
        if(fireTick + fireTimeOffset > fireDelay)
        {
            fireTick -= fireDelay;
            OnFire();
        }
    }

    void OnDisable()
    {
        //FIXME: animation hack
        Vector3 pos = transform.parent.localPosition;
        pos.y = 0;
        transform.parent.localPosition = pos;
    }
    
    #endregion
}
