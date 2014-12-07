using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{

    #region public variables

    public AudioClip music; //lazy music start hack

    public static PlayerController instance = null;//lazy static accessor

    public Character characterController;

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
        
		characterController.TryMove(dir);
    }

    void TryLook()
    {
		Vector3 characterScreenPosition = Camera.main.WorldToScreenPoint(characterController.transform.position);
		characterController.TryLook(Input.mousePosition - characterScreenPosition);
	}
	
    void TryAction()
    {
        if (Input.GetMouseButtonDown(0))
        {

        }
        if (Input.GetMouseButtonUp(0))
        {

        }
        if (Input.GetMouseButtonDown(1))
        {
            
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            
        }
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
		if (characterController)
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
			transform.position = characterController.transform.position;
		}

		if(!characterController) //FIXME: gameover hack
        {
            Application.LoadLevel(Application.loadedLevel);
        }
	}

    void OnDisable()
    {
		characterController.TryMove(Vector2.zero);
    }

    #endregion
}
