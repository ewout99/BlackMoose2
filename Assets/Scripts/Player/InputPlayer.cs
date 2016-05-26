using UnityEngine;
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
    public bool oracleAttached;

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
    public AudioClip weaponsFire1;
    public AudioClip weaponsFire2;

    //Private Refrences
    private Camera playerCamera;
    private GameObject weaponRef;
    private Transform bulletSpawnRef;
    private GameObject OracleRef;
    private MovementPlayer moveRef;
    private Animator aniRef;
    private SpriteRenderer spRef;


    // Use this for initialization
    void Start () {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        moveRef = GetComponent<MovementPlayer>();
        aniRef = GetComponent<Animator>();
        spRef = GetComponent<SpriteRenderer>();
        weaponRef = transform.FindChild("Temp Weapon").gameObject;
        bulletSpawnRef = transform.FindChild("Temp Weapon").gameObject.transform.FindChild("Temp Spawn");
        inverseAttackspeed = 1f / attackSpeed;
	}
	
	// Update is called once per frame
	void Update () { 

        // Check for Local Player
        if (!isLocalPlayer)
            return;

        // Check the aim of the player
        mousePosition = playerCamera.ScreenToWorldPoint(Input.mousePosition);
        aimVec = mousePosition - transform.position;
        targetAngle = Mathf.Atan2(aimVec.y, aimVec.x) * Mathf.Rad2Deg;
        AdjustWeaponRotation();

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
            if (moveVecInput.x < 0)
            {
                spRef.flipX = true;
                CmdFlipX(true);
            }
            else if (moveVecInput.x > 0)
            {
                spRef.flipX = false;
                CmdFlipX(false);
            }
        }
        else
        {
            aniRef.SetBool("walking", false);
            aniRef.SetBool("running", false);
        }

        // Firing bullet
        if (Input.GetButton("Fire") && canShoot)
        {
            canShoot = false;
            playerCamera.GetComponent<CameraFollow>().ScreenShake(5, 14);
            moveRef.Recoil(aimVec);
            StartCoroutine(ShootDelay());
            CmdShoot(aimVec);
        }

        // Pick up and drop the oracle
        if (Input.GetButtonDown("PickUp"))
        {
            CmdAttachOracle();
        }

        if (Input.GetButtonDown("PutDown"))
        {
            CmdDettachOracle();
        }

    }

    private void AdjustWeaponRotation()
    {
        // Add weapons to player then enable
        weaponRef.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, targetAngle));
    }

    public void WeaponSwitch(int weaponIndex)
    {
        // Set new attackspeed;
        inverseAttackspeed = 1/  bulletPrefabs[weaponIndex].GetComponent<Bullet>().attackSpeed;
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
    void CmdAttachOracle()
    {

    }

    [Command]
    void CmdDettachOracle()
    {

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
    }

    // Determines the fire rate of the shooting
    IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(inverseAttackspeed);
        canShoot = true;
    }

}
