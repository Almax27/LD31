using UnityEngine;
using System.Collections;

public class Character : Killable {
	
    public Transform moveBody;
    public Transform lookBody;

	public float movementSpeed = 1f;
	public float movementRotSpeed = 360f / 0.5f; //360 degrees in half a second
    public float lookRotationSpeed = 360f;

    public bool useSteering = false;

    public Vector2 Heading { get { return heading; } }
    public Vector2 LookDirection { get { return lookDirection; } }

	protected Animator animator;

    bool isMoving = false, wasMoving = false;
    Vector2 heading = Vector2.zero;
    Vector2 lookDirection = Vector2.zero;

    #region Bot methods

    public void TryMove(Vector2 _heading)
    {
        if (_heading != Vector2.zero)
        {
            heading = _heading.normalized;
            isMoving = true;
        }
        else
		{
            isMoving = false;
		}
    }

    public void TryTurn(Vector2 _heading)
    {
        if (_heading != Vector2.zero)
        {
            heading = _heading.normalized;
        }
    }

    public void TryLook(Vector2 _lookDirection)
    {
        lookDirection = _lookDirection.normalized;
    }

    public virtual void TryStartAction()
    {

    }

    public virtual void TryEndAction()
    {
        
    }

	public override void OnDamage(DamagePacket _damagePacket)
	{
		base.OnDamage(_damagePacket);
		animator.SetTrigger("OnDamage");
	}

    #endregion

    #region Monobehaviour methods

	// Use this for initialization
	protected virtual void Start () 
	{
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
    protected virtual void FixedUpdate () 
	{

	}

    protected virtual void Update()
	{
		if(wasMoving != isMoving)
		{
			if(isMoving)
				animator.SetTrigger("Walk");
			else
				animator.SetTrigger("Idle");
			wasMoving = isMoving;
		}

        if (isMoving)
        {

            Vector3 dir = heading;

            //update movement facing
            if (moveBody && heading != Vector2.zero)
            {
                Quaternion currentFacing = moveBody.transform.localRotation;
                float desiredAngle = Mathf.Atan2(heading.y, heading.x) * Mathf.Rad2Deg;
                Quaternion desiredFacing = Quaternion.Euler(new Vector3(0, 0, desiredAngle - 90));
                
                moveBody.transform.localRotation = Quaternion.RotateTowards(currentFacing, desiredFacing, movementRotSpeed * Time.deltaTime);

                if(useSteering)
                {
                    dir = moveBody.transform.up;
                }
            }

            rigidbody2D.velocity = dir * movementSpeed;
        } 
        else
        {
            rigidbody2D.velocity = Vector3.zero;
        }

        if (lookBody && lookDirection != Vector2.zero)
        {
            Quaternion currentFacing = lookBody.transform.localRotation;
            float desiredAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
            Quaternion desiredFacing = Quaternion.Euler(new Vector3(0, 0, desiredAngle - 90));
            
            lookBody.transform.localRotation = Quaternion.RotateTowards(currentFacing, desiredFacing, lookRotationSpeed * Time.deltaTime);
        }
	}

    #endregion
}
