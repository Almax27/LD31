using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour 
{

    #region public variables

    public AudioClip music; //lazy music start hack

    public static PlayerController instance = null;//lazy static accessor

    public CombatCharacter character;

	public bool inputLocked = false;

	public FlareItem flareItemPrefab;

	public int currentGun = 0;
	public List<Gun> availableGuns = new List<Gun>();

	public int currentMelee = 0;
	public List<MeleeAttack> availableMelee = new List<MeleeAttack>();

	public bool isMeleeSelected = false;

    #endregion

    #region private variables

    float controlLockTick = 0;

    #endregion

    #region public methods

    #endregion

    #region private methods

    void TryMove()
    {
        //Handle input 
        Vector2 dir = Vector2.zero;
        if(Input.GetKey(KeyCode.W))
        {
            dir.y = 1;
        }
        else if (Input.GetKey (KeyCode.S)) 
        {
            dir.y = -1;
        }
        if (Input.GetKey (KeyCode.A))
        {
            dir.x = -1;
        }
        else if (Input.GetKey (KeyCode.D))
        {
            dir.x = 1;
        }
        
		character.TryMove(dir);
    }

    void TryLook()
    {
		Vector3 characterScreenPosition = Camera.main.WorldToScreenPoint(character.transform.position);
		character.TryLook(Input.mousePosition - characterScreenPosition);
	}
	
    void TryAction()
    {
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			
		}
		
		if (Input.GetMouseButtonDown(0))
        {
			Gun gun = GetCurrentGun();
			if(gun)
			{
				gun.owner = character;
				bool hasAttacked = gun.BeginFire();

				if(hasAttacked && !string.IsNullOrEmpty(gun.animationTrigger))
				{
					character.animator.SetTrigger(gun.animationTrigger);
				}
			}
        }
        if (Input.GetMouseButtonUp(0))
        {
			Gun gun = GetCurrentGun();
			if(gun && gun.isFiring)
			{
				gun.owner = character;
				gun.EndFire();
			}
        }

		if (Input.GetMouseButtonDown(1))
		{
			MeleeAttack melee = GetCurrentMelee();
			if(melee)
			{
				bool hasAttacked = melee.ApplyDamage();
				if(hasAttacked && !string.IsNullOrEmpty(melee.animationTrigger))
				{
					character.animator.SetTrigger(melee.animationTrigger);
				}
			}
		}
			
			//drop flare
		if (Input.GetKeyDown(KeyCode.F))
		{
			DropFlare();
		}

		//cycle guns
		if (Input.GetKeyDown(KeyCode.Q))
		{
			if(isMeleeSelected == false)
			{
				SelectGun(currentGun + 1);
			}
			else
			{
				SelectGun(currentGun);
			}
		}

		//select gun
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			SelectGun(0);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			SelectGun(1);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			SelectGun(2);
		}

		//select melee
		if (Input.GetKeyDown(KeyCode.E))
		{
			SelectMelee();
		}

    }

	void DropFlare()
	{
		GameObject gobj = Instantiate(flareItemPrefab.gameObject) as GameObject;
		gobj.transform.position = this.transform.position;
	}

	Gun GetCurrentGun()
	{
		if(currentGun < availableGuns.Count)
		{
			return availableGuns[currentGun];
		}
		else
		{
			return null;
		}
	}

	void SelectGun(int _gunIndex)
	{
		Gun previousGun = GetCurrentGun();
		if(previousGun && previousGun.isFiring)
		{
			previousGun.EndFire();
		}
		//TODO: check if it has been purchased
		currentGun = _gunIndex;
		if(currentGun >= availableGuns.Count)
		{
			currentGun = 0;
		}
		isMeleeSelected = false;

		Gun gun = GetCurrentGun();
		if(gun && !string.IsNullOrEmpty(gun.animationTrigger))
		{
			character.animator.SetTrigger(gun.animationTrigger);
		}
	}

	MeleeAttack GetCurrentMelee()
	{
		if(currentMelee < availableMelee.Count)
		{
			return availableMelee[currentMelee];
		}
		else
		{
			return null;
		}
	}

	void SelectMelee()
	{
		isMeleeSelected = true;
	}

    #endregion

    #region monobehaviour methods

	// Use this for initialization
	void Awake () 
    {
        instance = this;

        FAFAudio.Instance.PlayMusic(music);
	}
	
	// Update is called once per frame
	void Update () 
    {
		if (character)
		{
			if(controlLockTick <= 0)
            {
                TryMove();
                TryLook();
                TryAction();
            }
            else
            {
                controlLockTick -= Time.deltaTime;
            }

            //follow bot so audio can locate correctly
			transform.position = character.transform.position;
			transform.rotation = character.lookBody.rotation;
		}

		if(!character) //FIXME: gameover hack
        {
            Application.LoadLevel(Application.loadedLevel);
        }
	}

    void OnDisable()
    {
		character.TryMove(Vector2.zero);
    }

    #endregion
}
