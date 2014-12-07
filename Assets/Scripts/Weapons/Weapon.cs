using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour 
{

    #region public variables

    public Projectile projectilePrefab = null;
    public AudioClip fireSound = null;

    public Transform spawnNode = null; //defaults to self if null

    public float fireDelay = 1; //shots per second
    public float fireTimeOffset = 0; 

    public float fireSpreadDeg = 0;

    #endregion

    #region protected variables

    protected bool isFiring = false;
    protected float fireTick = 0;

    #endregion

    #region protected methods

    protected virtual void OnFire()
    {
        Quaternion spead = Quaternion.Euler(0, 0, Random.Range(-fireSpreadDeg, fireSpreadDeg));
        GameObject gobj = Instantiate(projectilePrefab, spawnNode.position, spawnNode.rotation * spead) as GameObject;

        FAFAudio.Instance.PlayOnce2D(fireSound, transform.position, 0.3f);
    }

    #endregion

    #region Monobehaviour methods

    void Awake()
    {
        spawnNode = transform;
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
