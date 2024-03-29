﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class InputPlayer : NetworkBehaviour {


    //Editors Variables
    [SerializeField]
    private float attackSpeed;
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float normalSpeed;
    
    public bool canShoot;
    [SyncVar]
    public bool oracleAttached;
    public float pickUpRange = 1.7f;

    //Private Variables
    private Vector3 mousePosition = Vector3.zero;
    private Vector2 aimVec = Vector2.right;
    private Vector2 moveVecInput;
    private float targetAngle;
    [SyncVar(hook = "FlipXHook")]
    private bool CurrentFlipX;

    private float inverseAttackspeed;
    private float moveSpeedInput;

    //Editor Refrences
    public GameObject[] bulletPrefabs;
    public Sprite[] gunPrefabs;

    //Audio Clips
    private AudioSource aSREF;
    public AudioClip weaponsFire1;
    public AudioClip walking1;

    //Private Refrences
    private Camera playerCamera;
    private GameObject weaponRef;
    private Transform bulletSpawnRef;
    private GameObject OracleRef;
    private MovementPlayer moveRef;
    private Animator aniRef;
    private CustomNetworkAnim weaponAniRef;
    private SpriteRenderer spRef;
    private Entity entiRef;

    [SerializeField]
    private int shakeIntesityFire, shakeAmountFire;


    // Use this for initialization
    void Start () {
        if (GetComponent<IngamePlayer>().typeIngame == 4)
        {
            return;
        }
        moveRef = gameObject.GetComponent<MovementPlayer>();
        aniRef = gameObject.GetComponent<Animator>();
        spRef = gameObject.GetComponent<SpriteRenderer>();
        entiRef = gameObject.GetComponent<Entity>();
        aSREF = gameObject.GetComponent<AudioSource>();
        weaponRef = transform.FindChild("Temp Weapon").gameObject;
        weaponAniRef = gameObject.GetComponent<CustomNetworkAnim>();
        bulletSpawnRef = weaponRef.gameObject.transform.FindChild("Temp Spawn");
        inverseAttackspeed = 1f / attackSpeed;
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }
	
	// Update is called once per frame
	void Update () {

        // Check for Local Player
        if (!isLocalPlayer)
        {
            return;
        }
        else if (entiRef.deathState)
        {
            if(oracleAttached)
            {
                CmdDettachOracle();
            }
            return;
        }

        // Check the aim of the player
        mousePosition = playerCamera.ScreenToWorldPoint(Input.mousePosition);
        aimVec = mousePosition - transform.position;
        targetAngle = Mathf.Atan2(aimVec.y, aimVec.x) * Mathf.Rad2Deg;
        AdjustWeaponRotation();

        if (mousePosition.x < transform.position.x && !spRef.flipX)
        {
            // Debug.Log("Flipped");
            spRef.flipX = true;
            weaponAniRef.CmdDirection(false);
            CmdFlipX(true);
        }
        else if (mousePosition.x > transform.position.x && spRef.flipX)
        {
            // Debug.Log("No Flip");
            spRef.flipX = false;
            weaponAniRef.CmdDirection(true);
            CmdFlipX(false);
        }

        // Check the button input of the player
        moveVecInput.x = Input.GetAxisRaw("Horizontal");
        moveVecInput.y = Input.GetAxisRaw("Vertical");

        if (Input.GetButton("Walk") || oracleAttached)
        {
            moveSpeedInput = walkSpeed;
        }
        else
        {
            moveSpeedInput = normalSpeed;
        }

        // Call the Movement class
        moveRef.Move(moveVecInput.x, moveVecInput.y, moveSpeedInput);

        // Check which animation needs to be played
        if ( moveVecInput.x > 0 || moveVecInput.x < 0 || moveVecInput.y > 0 || moveVecInput.y < 0)
        {
            // Are we running or walking
            if ( moveSpeedInput == walkSpeed)
            {
                aniRef.SetBool("walking", true);
                aniRef.SetBool("running", false);
            }
            else
            {
                aniRef.SetBool("walking", false);
                aniRef.SetBool("running", true);
            }
            // Are we walking forward or backwards
            if (CurrentFlipX == (moveVecInput.x < 0))
            {
                aniRef.SetBool("backwards", false);
            }
            else
            {
                aniRef.SetBool("backwards", true);              
            }
            if (!aSREF.isPlaying)
            {
                aSREF.clip = walking1;
                aSREF.Play();
            }
      
        }
        // Not moving
        else
        {
            aniRef.SetBool("walking", false);
            aniRef.SetBool("running", false);
            if (Input.GetButtonDown("Dance"))
            {
                aniRef.SetTrigger("dancing");
            }
        }



        // Firing bullet
        if (Input.GetButton("Fire") && canShoot && !oracleAttached)
        {
            canShoot = false;
            playerCamera.GetComponent<CameraFollow>().ScreenShake(shakeIntesityFire, shakeAmountFire);
            moveRef.Recoil(aimVec);
            StartCoroutine(ShootDelay());
            CmdShoot(aimVec);
            weaponAniRef.CmdFire(true);
            if (!aSREF.isPlaying)
            {
                aSREF.clip = weaponsFire1;
                aSREF.Play();
            }
        }
        // Pick up and drop the oracle
        if (Input.GetButtonDown("PickUp"))
        {
            if (!OracleRef)
            {
                OracleRef = GameObject.Find("Temp Oracle(Clone)");
            }
            if (!oracleAttached && !OracleRef.GetComponent<MovementOracle>().beingCarried && Vector3.Distance(OracleRef.transform.position, transform.position) <= pickUpRange)
            {
                CmdAttachOracle(netId);
            }
            
        }

        if (Input.GetButtonDown("PutDown"))
        {
            if (oracleAttached)
            {
                CmdDettachOracle();
            }
        }

    }

    // Does what is says
    private void AdjustWeaponRotation()
    {
        weaponRef.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, targetAngle));
    }

    /// <summary>
    /// Makes the player switch out to the desired weapon
    /// </summary>
    /// <param name="weaponIndex">Weapon that it needs to switch to. Look in inspector</param>
    public void WeaponSwitch(int weaponIndex)
    {
        // Set new attackspeed;
        inverseAttackspeed = 1 /  bulletPrefabs[weaponIndex].GetComponent<Bullet>().attackSpeed;
        // Set new spriterender;
        weaponRef.GetComponent<SpriteRenderer>().sprite = gunPrefabs[weaponIndex];
    }

    [Command]
    void CmdShoot(Vector2 direction)
    {
        // Spawn Bullet prefab at the edge fo the gun
        GameObject bullet = (GameObject)Instantiate(bulletPrefabs[0], bulletSpawnRef.position, bulletSpawnRef.rotation);
        bullet.GetComponent<Bullet>().Go(direction, transform.tag);
        NetworkServer.Spawn(bullet);
    }

    [Command]
    void CmdAttachOracle(NetworkInstanceId InputNetworkId)
    {
        weaponAniRef.SetWeaponDisplay(0);
        OracleRef = GameObject.Find("Temp Oracle(Clone)");
        oracleAttached = true;
        OracleRef.GetComponent<MovementOracle>().CmdSetTargetObject(InputNetworkId);
    }

    [Command]
    void CmdDettachOracle()
    {
        weaponAniRef.SetWeaponDisplay(1);
        OracleRef = GameObject.Find("Temp Oracle(Clone)");
        oracleAttached = false;
        OracleRef.GetComponent<MovementOracle>().CmdResetTarget();
    }

    // A Command and a hook to set the flipX for the SpriteRenderer
    [Command]
    void CmdFlipX(bool setTo)
    {
        gameObject.GetComponent<SpriteRenderer>().flipX = setTo;
        CurrentFlipX = setTo;
    }
    // Hook for SyncVar
    private void FlipXHook(bool setTo)
    {
        gameObject.GetComponent<SpriteRenderer>().flipX = setTo;
        CurrentFlipX = setTo;
    }

    // Determines the fire rate of the shooting
    IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(inverseAttackspeed);
        weaponAniRef.CmdFire(false);
        canShoot = true;
    }

}
