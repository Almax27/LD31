using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour 
{
	#region public types

	[System.Serializable]
	public class PlayerStats
	{
		public int money = 0;
		public int flares = 0;
		public int ammo = 0;
		public int currentGun = 0;
		public int currentMelee = 0;
	}

	#endregion


    #region public variables

	public static PlayerController instance = null;//lazy static accessor

	public AudioClip music = null; //lazy music hack

	public PlayerStats playerStats = new PlayerStats();

    public CombatCharacter character;

	public FlareItem flareItemPrefab;
	
	public List<Gun> availableGuns = new List<Gun>();
	public List<MeleeAttack> availableMelee = new List<MeleeAttack>();
	public bool isMeleeSelected = false;

    #endregion

    #region private variables

    float controlLockTick = 0; //set this to time in seconds for input to lock
	float timeOfDeath = 0;

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
				SelectGun(playerStats.currentGun + 1);
			}
			else
			{
				SelectGun(playerStats.currentGun);
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
		if(playerStats.flares > 0)
		{
			GameObject gobj = Instantiate(flareItemPrefab.gameObject) as GameObject;
			gobj.transform.position = this.transform.position;

			playerStats.flares--;
		}
	}

	Gun GetCurrentGun()
	{
		if(Shop.instance == null || !Shop.instance.IsGunPurchased(playerStats.currentGun))
		{
			return null; //not purchased
		}

		if(playerStats.currentGun < availableGuns.Count)
		{
			return availableGuns[playerStats.currentGun];
		}
		else
		{
			return null;
		}
	}

	void SelectGun(int _gunIndex)
	{
		if(Shop.instance == null || !Shop.instance.IsGunPurchased(_gunIndex))
		{
			return; //not purchased
		}

		Gun previousGun = GetCurrentGun();
		if(previousGun && previousGun.isFiring)
		{
			previousGun.EndFire();
		}

		playerStats.currentGun = _gunIndex;
		if(playerStats.currentGun >= availableGuns.Count)
		{
			playerStats.currentGun = 0;
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
		if(Shop.instance == null || !Shop.instance.IsMeleePurchased(playerStats.currentMelee))
		{
			return null; //not purchased
		}
		if(playerStats.currentMelee < availableMelee.Count)
		{
			return availableMelee[playerStats.currentMelee];
		}
		else
		{
			return null;
		}
	}

	void SelectMelee()
	{
		if(Shop.instance == null || !Shop.instance.IsMeleePurchased(playerStats.currentMelee))
		{
			return; //not purchased
		}
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
		if(Time.timeScale == 0)
		{
			return;
		}

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            playerStats.money += 100;
        }
#endif

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
		else //handle player death
        {
			if(timeOfDeath <= 0)
			{
				timeOfDeath = Time.time;
			}
			if(Time.time - timeOfDeath > 0.5f && Input.anyKeyDown)
			{
				Respawn();
			}
        }
	}

	void Respawn()
	{
		Application.LoadLevel(Application.loadedLevel);
	}

    void OnDisable()
    {
		character.TryMove(Vector2.zero);
    }

#endregion
}
